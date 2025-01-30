namespace BokaInteDirekt.DTO
{
	public class BookingRequest
	{
		public required string Day { get; set; } //2025-01-26
		public required string StartTime { get; set; } //10:00
		public required string EndTime { get; set; } //11:00
		public required string BookingType { get; set; }
	}
}
