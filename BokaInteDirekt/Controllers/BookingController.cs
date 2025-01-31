using BokaInteDirekt.DTO;
using BokaInteDirekt.Interfaces;
using BokaInteDirekt.Models;
using BokaInteDirekt.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

		[HttpPost]
		public async Task<IActionResult> BookAppointment([FromBody] BookingUserRequest request)
		{
			if (!ModelState.IsValid)
			{
				Console.WriteLine(DateTime.UtcNow.ToString(), request);
				return BadRequest($"Tiden gick inte att boka.{request}");
			}
			var appointment = _service.SaveAppointment(request.Request);
			if (appointment == null)
				return BadRequest();

			await _emailService.SetBookingEmail(request.User, request.Request);
			await _emailService.SetAdminEmail(request);
			return Ok(appointment);
		}

		[HttpPost]
		public async Task<IActionResult> CreateAppointment([FromBody] BookingRequest request)
		{
			var booking = await _service.CreateAppointment(request);
			return Ok("Appointment created!");
		}
	}
}
