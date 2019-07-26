using System.Linq;
using BookingMachine.Auth;
using BookingMachine.Interfaces;
using BookingMachine.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingMachine.Repositories
{
	public class QueueRepository : IQueueRepository
	{
		private readonly AppDbContext _db;
		private readonly IComputerRepository _computerRepository;

		public QueueRepository(AppDbContext db, IComputerRepository computerRepository)
		{
			_db = db;
			_computerRepository = computerRepository;
		}
		
		public int GetIndex(ApplicationUser user)
		{
			return _db.Queues.First(q => q.User == user).Index;
		}

		public void PutInQueue(ApplicationUser user, Room preferredRoom, int amount)
		{
			int index;
			if (!_db.Queues.Any())
				index = 1;
			else if (_db.Queues.Any(q => q.User == user))
				index = _db.Queues.First(q => q.User == user).Index;
			else
				index = _db.Queues.Max(q => q.Index) + 1;
			for (int i = 0; i < amount; i++)
			{
				var queue = new Queue
				{
					Index = index,
					User = user,
					PreferredRoom = preferredRoom
				};
				_db.Queues.Add(queue);
				_db.SaveChanges();
			}
		}

		public void LeaveQueue(string userId)
		{
			var queues = _db.Queues.Where(q => q.User.Id == userId);
			_db.Queues.RemoveRange(queues);
			_db.SaveChanges();
		}

		public void RemovePlaceFromQueue(int queueId)
		{
			var queue = _db.Queues.Find(queueId);
			_db.Queues.Remove(queue);
			_db.SaveChanges();
		}

		public void MoveQueue()
		{
			while (_db.Queues.Any() && _db.Queues.Min(q => q.Index) != 1)
			{
				foreach (var queue in _db.Queues)
				{
					queue.Index--;
				}

				_db.SaveChanges();
			}
		}

		public async void BookFromQueue()
		{
			while (_db.Queues.Any() && _db.Computers.Any(c => !c.IsTaken && !c.IsBooked))
			{
				var queue = _db.Queues.Include(q => q.User).First(q => q.Index == _db.Queues.Min(qu => qu.Index));
				await _computerRepository.Book(1, queue.PreferredRoom, queue.User.Id);
				_db.Queues.Remove(queue);
				_db.SaveChanges();
			}
		}
	}
}