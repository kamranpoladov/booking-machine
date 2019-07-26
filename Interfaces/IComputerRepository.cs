using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingMachine.Models;

namespace BookingMachine.Interfaces
{
    public interface IComputerRepository
    {
        int GetNumberOfFreeCommons();
        int GetNumberOfFreeVips();
        Task<int> Book(int amount, Room room, string userId);
        void RemoveBooking(string userId, int amount);
        void RemoveOverdue();
        void DeleteBooking(int compId, string userId);
        Room GetRoomForUser(string userId);
        int GetAmountOfPlacesForUser(string userId);
        DateTime GetExpireDateTimeForUser(string userId);
        IEnumerable<Computer> GetBooksForUser(string userId);
    }
}