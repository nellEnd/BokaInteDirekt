using BokaInteDirekt.DTO;
using BokaInteDirekt.Interfaces;
using BokaInteDirekt.Models;

namespace BokaInteDirekt.Services
{
	public class BookingService : IBookingService
	{
		private List<Booking>? _bookings;

		public Task<List<Booking>> GetAll()
		{
			if (_bookings == null)
			{
				populateList();
			}
			return Task.FromResult(_bookings);
		}

		public Task<Booking>? SaveAppointment(BookingRequest request)
		{
			if (request == null)
				return null;

			var newBooking = new Booking
			{
				Day = request.Day,
				Date = DateTime.ParseExact(request.Day, "yyyy-MM-dd", null),
				StartTime = request.StartTime,
				EndTime = request.EndTime,
				IsAvailable = false
			};
			populateList();
			if (_bookings != null)
			{
				foreach (var _booking in _bookings)
				{
					if (_booking.Date.Equals(newBooking.Date))
					{
						if (_booking.StartTime == newBooking.StartTime)
						{
							if (_booking.EndTime == newBooking.EndTime)
							{
								_booking.IsAvailable = false;
								break;
							}
						}
					}
				}
			}
			return Task.FromResult(newBooking);
		}
	

	private void populateList()
		{
			_bookings = new List<Booking>();
			var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

			foreach (var day in days)
			{
				var date = DateTime.Today.AddDays(Array.IndexOf(days, day)); // Lägger till rätt dag
				for (int i = 0; i < 10; i++)
				{
					// Skapar tider för bokningarna, ex: 08:00 - 09:00, 09:00 - 10:00, etc.
					var startHour = 8 + i;
					var startTime = $"{startHour:00}:00";
					var endTime = $"{startHour + 1:00}:00";

					// Lägg till bokningen i listan
					_bookings.Add(new Booking(day, date, startTime, endTime, true));
				}
			}
		}
	}
}
