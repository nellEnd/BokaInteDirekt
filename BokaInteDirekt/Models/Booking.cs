namespace BokaInteDirekt.Models
{
	public class Booking
	{
		public string Day { get; set; } //monday
		public DateTime Date { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public bool IsAvailable { get; set; }

		public Booking() { }

		public Booking(string day, DateTime date, string startTime, string endTime, bool isAvailable)
		{
			Day = day;
			Date = date;
			StartTime = startTime;
			EndTime = endTime;
			IsAvailable = isAvailable;
		}
	}
}
