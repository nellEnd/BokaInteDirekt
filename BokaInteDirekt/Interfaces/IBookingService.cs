using BokaInteDirekt.DTO;
using BokaInteDirekt.Models;

namespace BokaInteDirekt.Interfaces
{
	public interface IBookingService
	{
		Task<List<Booking>?> GetBookedAppointments();
		Task<List<string>?> GetAvailableAppointments(string bookingType);
		Task<Booking?> BookAppointment(int id, string bookingType, BookAppointmentRequest request);
        Task<Booking> UpdateAppointment(Booking appointment, BookAppointmentRequest request, string? newEndTime = null);
		List<Booking> FindContinuousSlots(string startTime, string endTime, List<Booking> slots);
        Task<Booking> CreateAppointment(BookingRequest request);
		Task<List<Booking>?> GetBookings();
		Task <bool> DeleteAppointment(int id);
		Task<Booking?> CancelAppointment(int id, string cancelCode);
	}
}
