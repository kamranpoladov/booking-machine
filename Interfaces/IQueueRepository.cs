using BookingMachine.Auth;
using BookingMachine.Models;

namespace BookingMachine.Interfaces
{
	public interface IQueueRepository
	{
		int GetIndex(ApplicationUser user);
		void PutInQueue(ApplicationUser user, Room preferredRoom, int amount);
		void LeaveQueue(string userId);
		void RemovePlaceFromQueue(int queueId);
		void MoveQueue();
		void BookFromQueue();
	}
}