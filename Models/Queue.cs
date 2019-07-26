using BookingMachine.Auth;

namespace BookingMachine.Models
{
	public class Queue
	{
		public int QueueId { get; set; }
		public int Index { get; set; }
		public Room PreferredRoom { get; set; }
		public virtual ApplicationUser User { get; set; }
	}
}