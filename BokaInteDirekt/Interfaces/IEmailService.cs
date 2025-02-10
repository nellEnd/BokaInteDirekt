using BokaInteDirekt.DTO;
using BokaInteDirekt.Models;

namespace BokaInteDirekt.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string receiver, string subject, string body);
        Task SetBookingEmail(User user, Booking booking, string bookingType);
        Task SetAdminEmail(Booking booking, BookAppointmentRequest request, string bookingType);
        string GenerateGoogleCalendarLink(string title, DateTime startTime, DateTime endTime, string location, string details);
    }
}
