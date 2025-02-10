using BokaInteDirekt.DTO;
using BokaInteDirekt.Models;

namespace BokaInteDirekt.Interfaces
{
	public interface IBookingService
	{
		Task<List<Booking>> GetAll();
		Task<List<Booking>?> GetAvailableAppointments(string bookingType);
		Task<Booking?> BookAppointment(int id, string bookingType, BookAppointmentRequest request);
		Task<Booking> CreateAppointment(BookingRequest request);
		Task<List<Booking>?> GetBookings();
		Task <bool> DeleteAppointment(int id);
	}
}
