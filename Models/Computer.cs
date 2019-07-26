using System;
using BookingMachine.Auth;

namespace BookingMachine.Models
{
    public class Computer
    {
        public short Id { get; set; }
        public bool IsTaken { get; set; }
        public bool IsBooked { get; set; }
        public DateTime? TimeBooked { get; set; }
        public DateTime? TimeTaken { get; set; }
        public Room Room { get; set; }
        public virtual ApplicationUser BookedBy { get; set; }
    }
}