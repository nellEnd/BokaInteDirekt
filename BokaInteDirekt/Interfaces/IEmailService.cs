using BokaInteDirekt.DTO;
using BokaInteDirekt.Models;

namespace BokaInteDirekt.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string receiver, string subject, string body);
        Task SetBookingEmail(User user, Booking booking, string bookingType, int id, string cancelCode);
        Task SetAdminEmail(Booking booking, BookAppointmentRequest request, string bookingType);
      //  Task SetCancelEmail()
        string GenerateGoogleCalendarLink(string title, DateTime startTime, DateTime endTime, string location, string details);
    }
}
