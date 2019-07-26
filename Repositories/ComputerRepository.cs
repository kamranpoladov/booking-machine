using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingMachine.Auth;
using BookingMachine.Interfaces;
using BookingMachine.Models;
using Microsoft.AspNetCore.Identity;

namespace BookingMachine.Repositories
{
    public class ComputerRepository : IComputerRepository
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int ExpireTimeMinutes = 1;

        public ComputerRepository(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<int> Book(int amount, Room room, string userId)
        {
            int i;
            var user = await _userManager.FindByIdAsync(userId);
            for (i = 0; i < amount; i++)
            {
                var computer = room == Room.Any
                    ? _db.Computers.FirstOrDefault(c => !c.IsTaken && !c.IsBooked)
                    : _db.Computers.FirstOrDefault(c => !c.IsTaken && !c.IsBooked && c.Room == room);

                if (computer == null)
                    break;
                computer.IsBooked = true;
                computer.BookedBy = user;
                computer.TimeBooked = _db.Computers.Any(c => c.BookedBy == user && c.IsBooked)
                    ? _db.Computers.First(c => c.BookedBy == user && c.IsBooked).TimeBooked
                    : DateTime.Now;
                _db.SaveChanges();
            }
                Console.WriteLine("{0} booked {1} computers at {2}", user.UserName, i, DateTime.Now);
            return i;
        }

        public void RemoveBooking(string userId, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var computer = _db.Computers.FirstOrDefault(c => c.IsBooked && c.BookedBy.Id == userId);
                if (computer == null)
                    break;
                computer.IsBooked = false;
                computer.TimeBooked = DateTime.MinValue;
                _db.SaveChanges();
            }
        }

        public void RemoveOverdue()
        {
            foreach (var computer in _db.Computers)
            {
                if (computer.TimeBooked.HasValue && DateTime.Now.Subtract((DateTime) computer.TimeBooked).TotalMinutes > ExpireTimeMinutes)
                {
                    computer.IsBooked = false;
                    computer.TimeBooked = DateTime.MinValue;
                }
            }

            _db.SaveChanges();
        }

        public void DeleteBooking(int compId, string userId)
        {
            var computer = _db.Computers.FirstOrDefault(c =>
                c.Id == compId && c.BookedBy.Id ==
                userId);
            if (computer != null)
            {
                computer.IsBooked = false;
                computer.TimeBooked = DateTime.MinValue;
            }

            _db.SaveChanges();
        }
        
        public int GetNumberOfFreeCommons()
        {
            return _db.Computers.Count(c => 
                c.Room == Room.Common && !c.IsTaken && !c.IsBooked);
        }

        public int GetNumberOfFreeVips()
        {
            return _db.Computers.Count(c =>
                c.Room == Room.Vip && !c.IsTaken && !c.IsBooked);
        }

        public Room GetRoomForUser(string userId)
        {
            return _db.Computers.First(c =>
                c.IsBooked && c.BookedBy.Id == userId).Room;
        }

        public int GetAmountOfPlacesForUser(string userId)
        {
            return _db.Computers.Count(c =>
                c.IsBooked && c.BookedBy.Id == userId);
        }

        public DateTime GetExpireDateTimeForUser(string userId)
        {
            var timeBooked = _db.Computers.First(c =>
                c.BookedBy.Id == userId && c.IsBooked).TimeBooked;
            return timeBooked?.AddMinutes(ExpireTimeMinutes) ?? DateTime.MinValue;
        }

        public IEnumerable<Computer> GetBooksForUser(string userId)
        {
            return _db.Computers.Where(c => c.IsBooked && c.BookedBy.Id == userId);
        }
    }
}
