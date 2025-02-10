namespace BokaInteDirekt.Models
{
	public class Booking
	{
		public int Id { get; set; }
		public required string Day { get; set; }
		public DateTime Date { get; set; }
		public required string StartTime { get; set; }
		public required string EndTime { get; set; }
		public bool IsAvailable { get; set; }
		public required string BookingType { get; set; }
		public string? CustomerEmail { get; set; }
		public string? CancelId { get; set; }
	}
}
