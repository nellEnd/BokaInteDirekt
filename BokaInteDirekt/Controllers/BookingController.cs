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

		[HttpGet]
		public IActionResult GetAll() 
		{
			var bookings = _service.GetAll();
			if (bookings == null)
				return BadRequest();

			return Ok(bookings);
		}

		[HttpGet]
		public async Task<IActionResult> GetBookings()
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

			await _emailService.SetBookingEmail(request.User, book);
            await _emailService.SetAdminEmail(book, request);

            return Ok($"Following appointment was successfully booked:\n{book}");

			/*if (!ModelState.IsValid || request == null)
			{
				Console.WriteLine(DateTime.UtcNow.ToString(), request);
				return BadRequest($"Tiden gick inte att boka.{request}");
			}
			var appointment = _service.SaveAppointment(request.Request);
			if (appointment == null)
				return BadRequest();

			await _emailService.SetBookingEmail(request.User, request.Request);
			await _emailService.SetAdminEmail(request);
			return Ok(appointment);*/
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
	}
}
