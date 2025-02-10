using BokaInteDirekt.DTO;
using BokaInteDirekt.Interfaces;
using BokaInteDirekt.Models;
using BokaInteDirekt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace BokaInteDirekt.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class BookingController(IBookingService service, IEmailService emailService) : ControllerBase
	{
		private readonly IBookingService _service = service;
		private readonly IEmailService _emailService = emailService;

		//[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task <IActionResult> GetBookedAppointments() 
		{
			var bookings = await _service.GetBookedAppointments();
			if (bookings == null)
				return NotFound("No booked appointments where found.");

			return Ok(bookings);
		}

		[HttpGet]
		public async Task<IActionResult> GetAllAppointments()
		{
			var bookings = await _service.GetBookings();

			if(bookings == null)
				return BadRequest();
			return Ok(bookings);
		}

		[HttpGet]
		public async Task<IActionResult> GetAvailableAppointments([FromQuery] string bookingType)
		{
			var appointments = await _service.GetAvailableAppointments(bookingType);
			if (appointments == null || appointments.Count == 0)
				return NotFound("No available appointments found.");
			return Ok(appointments);
		}

		[HttpPost("{id}/{bookingType}")]
		public async Task<IActionResult> BookAppointment(int id, string bookingType, [FromBody] BookAppointmentRequest request)
		{
			if (id <= 0 || string.IsNullOrEmpty(request.User.Email))
				return BadRequest("Invalid request. Please enter a valid booking ID and email.");

			var book = await _service.BookAppointment(id, bookingType, request);
			if (book == null)
				return BadRequest("The appointment is not available.");

            await _emailService.SetBookingEmail(request.User, book, bookingType, book.Id, book.CancelId);
            await _emailService.SetAdminEmail(book, request, bookingType);

            return Ok($"Following appointment was successfully booked:\n{book}");
		}

		//[Authorize(Roles ="Admin")]
		[HttpPost]
		public async Task<IActionResult> CreateAppointment([FromBody] BookingRequest request)
		{
			var booking = await _service.CreateAppointment(request);
			return Ok("Appointment created!");
		}

        //[Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAppointment(int id)
		{
			if (id <= 0)
				return BadRequest();
			
			var deleted = await _service.DeleteAppointment(id);

			if (!deleted)
				return NotFound($"Could not find booking with ID {id}.");

			return Ok($"Booking with ID {id} was successfully deleted");
		}

		[HttpPost("{id}/{cancelCode}")]
		public async Task <IActionResult> CancelAppointment(int id, string cancelCode)
		{
			var appointment = await _service.CancelAppointment(id, cancelCode);

			if (appointment == null)
				return BadRequest("The appointment does not exist or is already cancelled.");

			return Ok($"Following appointment is cancelled:\n" +
				$"{appointment}");
		}
	}
}
