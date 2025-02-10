using BokaInteDirekt.DTO;
using BokaInteDirekt.Interfaces;
using BokaInteDirekt.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace BokaInteDirekt.Services
{
    public class EmailService(IOptions<SmtpEmail> smtp) : IEmailService
    {
        private readonly SmtpEmail _smtp = smtp.Value;
        private readonly string adminEmail = "nellyendler@gmail.com";

        public async Task SetBookingEmail(User user, Booking booking, string bookingType, int id, string cancelCode)
        {
            string cancellationLink = $"https://localhost:7139/api/Booking/CancelAppointment/{id}/{cancelCode}";
            string emailBody = $"Hej {user.FirstName}!\nDu har gjort följande bokning:\n{booking.BookingType}\n" +
                $"Datum: {booking.Day:yyyy-MM-dd}\n" +
                $"Tid: {booking.StartTime}-{booking.EndTime}" +
                $"\nAdress: Folkungatan 49.\nVälkommen!" +
                $"För att avbokad din tid, tryck på följande länk: {cancellationLink}";

            await SendEmail(user.Email, "Din bokning", emailBody);
        }

        // CANCEL EMAIL

        public async Task SendEmail(string receiver, string subject, string body)
        {
            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
                EnableSsl = _smtp.EnableSsl
            };

            var message = new MailMessage
            {
                From = new MailAddress(_smtp.SenderEmail, _smtp.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(receiver));
            await client.SendMailAsync(message);
        }

        public async Task SetAdminEmail(Booking booking, BookAppointmentRequest request, string bookingType)
        {
            var startTime = DateTime.Parse($"{booking.Day} {booking.StartTime}");
            var endTime = DateTime.Parse($"{booking.Day}   {booking.EndTime}");

            var googleCalendarLink = GenerateGoogleCalendarLink(
                booking.BookingType,
                startTime,
                endTime,
                "Adress",
                $"Bokning gjord av: {request.User.FirstName} {request.User.LastName} den: {DateTime.Now}"
                );

            string emailBody = $@"
        <p><strong>Ny boking: </p>
        <ul>
            <li><strong>{booking.BookingType} - {bookingType}</strong></li>
            <li>Datum: {booking.Day:yyyy-MM-dd}</li>
            <li>Tid: {booking.StartTime}-{booking.EndTime}</li>
            <li>Bokning gjord av: {request.User.FirstName} {request.User.LastName} <li>Datum: {DateTime.Now}</i> </li>
        </ul>
        <p>
            <a href='{googleCalendarLink}' 
               style='display:inline-block; padding:10px 15px; background-color:#4285F4; color:white; text-decoration:none; border-radius:5px;'>
               Lägg till i Google Kalender
            </a>
        </p>
    ";

            await SendEmail(adminEmail, "Ny bokning", emailBody);
        }

        public string GenerateGoogleCalendarLink(string title, DateTime startTime, DateTime endTime, string location, string details)
        {
            return
          $"https://www.google.com/calendar/render?action=TEMPLATE" +
          $"&text={Uri.EscapeDataString(title)}" +
          $"&dates={startTime:yyyyMMddTHHmmssZ}/{endTime:yyyyMMddTHHmmssZ}" +
          $"&details={Uri.EscapeDataString(details)}" +
          $"&location={Uri.EscapeDataString(location)}" +
          $"&ctz=Europe/Stockholm";
        }
    }
}
