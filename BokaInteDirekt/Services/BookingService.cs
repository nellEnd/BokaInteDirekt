using BokaInteDirekt.Context;
using BokaInteDirekt.DTO;
using BokaInteDirekt.Interfaces;
using BokaInteDirekt.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace BokaInteDirekt.Services
{
	public class BookingService(BokaInteDirektContext context) : IBookingService
	{
		private List<Booking>? _bookings;
		private readonly BokaInteDirektContext _context = context;

        public async Task<Booking> CreateAppointment(BookingRequest request)
        {
			Booking booking = new()
			{
				Day = request.Day,
				Date = DateTime.ParseExact(request.Day, "yyyy-MM-dd", null),
				StartTime = request.StartTime,
				EndTime = request.EndTime,
				IsAvailable = false
			};
			 _context.Add(booking);
			await _context.SaveChangesAsync();
			return booking;
        }

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

        public async Task<List<Booking>> GetBookings()
        {
            var bookings = await _context.Bookings.ToListAsync();

			if (bookings == null)
				return null;

			return bookings;
        }
    }
}
