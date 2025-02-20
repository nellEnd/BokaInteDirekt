using BokaInteDirekt.Context;
using BokaInteDirekt.DTO;
using BokaInteDirekt.Interfaces;
using BokaInteDirekt.Models;
using Microsoft.EntityFrameworkCore;

namespace BokaInteDirekt.Services
{
	public class BookingService(BokaInteDirektContext context) : IBookingService
	{
		// 1h = 12st slottar á 5 min

		private readonly BokaInteDirektContext _context = context;

		private readonly Dictionary<string, int> _bookingDurations = new()
		{
			{"CHECK UP", 20 },
			{"FIRST VISIT", 40 },
			{ "FIRST VISIT BABY", 30 },
			{ "BABY CHECK UP", 15 },
			{ "CRANIO-SACRAL THERAPY", 40 },
			{ "PARENT-BABY CHECK UP", 30 }
		};

		public async Task<Booking> CreateAppointment(BookingRequest request)
		{
			Booking booking = new()
			{
				Day = request.Day,
				Date = DateTime.ParseExact(request.Day, "yyyy-MM-dd", null),
				StartTime = request.StartTime,
				EndTime = request.EndTime,
				BookingType = request.BookingType,
				IsAvailable = true
			};
			_context.Add(booking);
			await _context.SaveChangesAsync();
			return booking;
		}

		public async Task<List<Booking>?> GetBookedAppointments()
		{
			var bookings = await _context.Bookings
				.Where(b => b.IsAvailable == false)
				.OrderBy(b => b.Date)
				.ThenBy(b => b.StartTime)
				.ToListAsync();

			if (bookings == null || bookings.Count == 0)
				return null;

			return bookings;
		}

		public List<string>? GetAvailableAppointments(string bookingType)
		{
			var appointments = _context.Bookings.Where(b => b.IsAvailable).ToList();
			var availableTimes = GetAvailableSlots(bookingType, appointments);
			Console.WriteLine("Tillgängliga tider:");
			foreach (var time in availableTimes)
			{
				Console.WriteLine(time);
			}

			return availableTimes;





			/*            if (!_bookingDurations.TryGetValue(bookingType.ToUpper(), out int duration))
							return new List<string>();

						var availableSlots = await _context.Bookings
							.Where(b => b.IsAvailable)
							.OrderBy(b => b.StartTime)
							.ToListAsync();

						var possibleStartTimes = new List<string>();

						for (int i = 0; i< availableSlots.Count; i++)
						{
							var slot = availableSlots[i];
							var newEndTime = TimeSpan.Parse(slot.StartTime).Add(TimeSpan.FromMinutes(duration)).ToString(@"hh\:mm");
							var continuosSlots = FindContinuousSlots(slot.StartTime, newEndTime, availableSlots);

							if (continuosSlots.Count > 0)
								possibleStartTimes.Add(slot.StartTime);
						}

						return possibleStartTimes;*/


			/*            var allSlots = await _context.Bookings
                            .Where(b => b.IsAvailable)
                            .OrderBy(b => b.Date)
                            .ThenBy(b => b.StartTime)
                            .ToListAsync();

                        var availableSlots = new List<Booking>();
                        // var duration = bookingType.ToUpper() == "Nybesök".ToUpper() ? 40 : 20;
                        _bookingDurations.TryGetValue(bookingType.ToUpper(), out int duration);

                        for (int i = 0; i < allSlots.Count; i++)
                        {
                            var slot = allSlots[i];
                            TimeSpan.TryParse(slot.StartTime, out TimeSpan startTime);
                            var requiredEndTime = startTime.Add(TimeSpan.FromMinutes(duration));

                            if ( i < allSlots.Count - 1)
                            {
                                var nextSlot = allSlots[i + 1];
                                TimeSpan.TryParse(nextSlot.StartTime, out TimeSpan nextStartTime);

                                if (slot.Date == nextSlot.Date && nextStartTime == startTime.Add(TimeSpan.FromMinutes(5)))
                                    availableSlots.Add(slot);
                            }
                        }
                        return availableSlots;*/
		}

		public List<string> GetAvailableSlots(string bookingType, List<Booking> appointments)
		{
			int slotDuration = new Dictionary<string, int>
			{
			{"CHECK UP", 20},
			{"FIRST VISIT", 40},
			{"FIRST VISIT BABY", 30},
			{"BABY CHECK UP", 15},
			{"CRANIO-SACRAL THERAPY", 40},
			{"PARENT-BABY CHECK UP", 30}
			}[bookingType.ToUpper()];

			List<string> availableSlots = new List<string>();
			/*12:00
			12:05
			12:10
			12:15
			12:20
			12:25*/
			foreach (var appointment in appointments)
			{
				TimeSpan startTime = TimeSpan.Parse(appointment.StartTime); // 12:00
				TimeSpan endTime = startTime.Add(TimeSpan.FromMinutes(slotDuration)); // 12:20

				// Kontrollera om hela sloten ryms inom tillgängliga tider
				bool slotFits = true;
				for (TimeSpan t = startTime; t < endTime; t = t.Add(TimeSpan.FromMinutes(5)))
				{
					if (!appointments.Any(a => TimeSpan.Parse(a.StartTime) == t && a.IsAvailable))
					{
						slotFits = false;
						break;
					}
				}

				if (slotFits)
				{
					availableSlots.Add(startTime.ToString(@"hh\:mm"));
				}
			}

			return availableSlots;

			/*
						TimeSpan currentSlot = startTime;
						Console.WriteLine("While loop: " + TimeSpan.FromMinutes(slotDuration));
						while (currentSlot.Add(TimeSpan.FromMinutes(slotDuration)) <= endTime)
						{
							availableSlots.Add(currentSlot.ToString(@"hh\:mm"));
							currentSlot = currentSlot.Add(TimeSpan.FromMinutes(5));
						}

						return availableSlots;*/
		}

		public async Task<Booking?> BookAppointment(int id, string bookingType, BookAppointmentRequest request)
		{
			if (!_bookingDurations.TryGetValue(bookingType.ToUpper(), out int duration))
				return null;

			var appointment = await _context.Bookings
				.FirstOrDefaultAsync(b => b.Id == id && b.IsAvailable);

			if (appointment == null || !TimeSpan.TryParse(appointment.StartTime, out TimeSpan startTime))
				return null;

			var newEndTime = startTime.Add(TimeSpan.FromMinutes(duration)).ToString(@"hh\:mm");

			if (appointment.EndTime == newEndTime)
				return await UpdateAppointment(appointment, request);

			var potentialSlots = await _context.Bookings
				.Where(b => b.Day == appointment.Day && b.IsAvailable)
				.OrderBy(b => b.StartTime)
				.ToListAsync();

			var continuousSlots = FindContinuousSlots(appointment.StartTime, newEndTime, potentialSlots);

			if (continuousSlots.Count == 0)
				return null;

			foreach (var slot in continuousSlots)
			{
				slot.IsAvailable = false;
				slot.CustomerEmail = request.User.Email;
				slot.CancelId = appointment.CancelId;
			}

			return await UpdateAppointment(appointment, request, newEndTime);


			////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

			/*if (!_bookingDurations.TryGetValue(bookingType.ToUpper(), out int duration))
                return null;

            var appointment = await _context.Bookings
                .Where(b => b.Id == id && b.IsAvailable)
                .FirstOrDefaultAsync();

            if (appointment == null || !TimeSpan.TryParse(appointment.StartTime, out TimeSpan startTime))
                return null;

            var newEndTime = startTime.Add(TimeSpan.FromMinutes(duration));

            if (appointment.EndTime == newEndTime.ToString(@"hh\:mm"))
                return await UpdateAppointment(appointment, request);

            var potentialSlots = await _context.Bookings
                .Where(b => b.Day == appointment.Day && b.IsAvailable)
                .OrderBy(b => b.StartTime)
                .ToListAsync();

            var followingSlot = potentialSlots
                .Where(b => b.StartTime == appointment.EndTime).FirstOrDefault();

            if (followingSlot == null)
                return null;

            var nextSlots = potentialSlots
                .Where(b =>
                TimeSpan.TryParse(b.EndTime, out TimeSpan bEnd) &&
                bEnd <= newEndTime)
                .ToList();

            var test = nextSlots
                .Where(b =>
                b.EndTime == newEndTime.ToString(@"hh\:mm")).FirstOrDefault();

            nextSlots.Add(test);

            if (nextSlots.Count == 0)
                return null;

            foreach (var slot in nextSlots)
            {
                slot.IsAvailable = false;
                slot.CustomerEmail = request.User.Email;
                slot.CancelId = appointment.CancelId;
            }
            return await UpdateAppointment(appointment, request, newEndTime.ToString(@"hh\:mm"));*/
		}

		public async Task<List<Booking>?> GetBookings()
		{
			var bookings = await _context.Bookings
				.OrderBy(b => b.Date)
				.ThenBy(b => b.StartTime)
				.ToListAsync();

			if (bookings == null)
				return null;

			return bookings;
		}

		public async Task<bool> DeleteAppointment(int id)
		{
			var appointment = await _context.Bookings.Where(x => x.Id == id).FirstOrDefaultAsync();

			if (appointment == null)
				return false;

			_context.Bookings.Remove(appointment);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<Booking?> CancelAppointment(int id, string cancelCode)
		{
			var appointment = await _context.Bookings
				.Where(b => b.Id == id && b.CancelId == cancelCode)
				.FirstOrDefaultAsync();

			if (appointment == null)
				return null;

			appointment.CancelId = null;
			appointment.CustomerEmail = null;
			appointment.IsAvailable = true;
			await _context.SaveChangesAsync();
			return appointment;
		}

		public async Task<Booking> UpdateAppointment(Booking appointment, BookAppointmentRequest request, string? newEndTime = null)
		{
			appointment.EndTime = newEndTime ?? appointment.EndTime;  // Om newEndTime inte är null, sätt det som ny sluttid annars behåll nuvarande.
			appointment.IsAvailable = false;
			appointment.CustomerEmail = request.User.Email;
			appointment.CancelId = Guid.NewGuid().ToString("N")[..6].ToUpper();

			await _context.SaveChangesAsync();
			return appointment;
		}

		public List<Booking> FindContinuousSlots(string startTime, string endTime, List<Booking> slots)
		{
			var result = new List<Booking>();
			string currentEndTime = startTime;

			foreach (var slot in slots)
			{
				if (slot.StartTime != currentEndTime)
					break; // Sluta om vi inte hittar en direkt anslutande slot

				result.Add(slot);
				currentEndTime = slot.EndTime;

				if (currentEndTime == endTime)
					return result; // Vi har hittat alla nödvändiga slots
			}

			return new List<Booking>();
		}
	}
}
