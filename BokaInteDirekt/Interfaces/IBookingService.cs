using BokaInteDirekt.DTO;
using BokaInteDirekt.Models;

namespace BokaInteDirekt.Interfaces
{
	public interface IBookingService
	{
		Task<List<Booking>> GetAll();
		Task<Booking>? SaveAppointment(BookingRequest request);
	}
}
