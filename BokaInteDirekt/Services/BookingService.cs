using BokaInteDirekt.Context;
using BokaInteDirekt.DTO;
using BokaInteDirekt.Interfaces;
using BokaInteDirekt.Models;
using Microsoft.EntityFrameworkCore;

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
                BookingType = request.BookingType,
                IsAvailable = true
            };
            _context.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<List<Booking>> GetAll()
        {
            var bookings = await _context.Bookings.ToListAsync();
            return bookings;
            /*if (_bookings == null)
			{
				populateList();
			}
			return Task.FromResult(_bookings);*/
        }

        public async Task<List<Booking>?> GetAvailableAppointments(string bookingType)
        {
            var allSlots = await _context.Bookings
                .Where(b => b.IsAvailable)
                .OrderBy(b => b.Date)
                .ThenBy(b => b.StartTime)
                .ToListAsync();
            //allSlots.OrderBy(b => b.Date);

            var availableSlots = new List<Booking>();
            var duration = bookingType.ToUpper() == "Nybesök".ToUpper() ? 40 : 20;

            for (int i = 0; i < allSlots.Count; i++)
            {
                var slot = allSlots[i];
                TimeSpan.TryParse(slot.StartTime, out TimeSpan startTime);
                var requiredEndTime = startTime.Add(TimeSpan.FromMinutes(duration));

                if(duration == 20)
                    availableSlots.Add(slot);
                else if(duration == 40 && i < allSlots.Count - 1)
                {
                    var nextSlot = allSlots[i + 1];
                    TimeSpan.TryParse(nextSlot.StartTime, out TimeSpan nextStartTime);

                    if(slot.Date == nextSlot.Date && nextStartTime == startTime.Add(TimeSpan.FromMinutes(20)))
                        availableSlots.Add(slot);
                }
            }
            return availableSlots;
        }

        public async Task<Booking?> BookAppointment(int id, string bookingType, BookAppointmentRequest request)
        {
            var appointment = await _context.Bookings.Where(b => b.Id == id).FirstOrDefaultAsync();

            if (appointment == null || !appointment.IsAvailable)
                return null;

            var duration = bookingType.ToUpper() == "Nybesök".ToUpper() ? 40 : 20;
            TimeSpan.TryParse(appointment.StartTime, out TimeSpan startTime);
            var newEndTime = startTime.Add(TimeSpan.FromMinutes(duration));

            appointment.EndTime = newEndTime.ToString(@"hh\:mm");
            appointment.IsAvailable = false;
            appointment.CustomerEmail = request.User.Email;

            var nextSlot = await _context.Bookings
                .Where(b => 
                b.Day == appointment.Day && 
                b.EndTime == appointment.EndTime && 
                b.IsAvailable)
                .FirstOrDefaultAsync();

            if (nextSlot != null)
                nextSlot.IsAvailable = false;

            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<List<Booking>> GetBookings()
        {
            var bookings = await _context.Bookings.ToListAsync();

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
                    //_bookings.Add(new Booking(day, date, startTime, endTime, true));
                }
            }
        }

    }
}
