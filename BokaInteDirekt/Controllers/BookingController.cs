using BokaInteDirekt.DTO;
using BokaInteDirekt.Interfaces;
using BokaInteDirekt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BokaInteDirekt.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookingController(IBookingService service) : ControllerBase
	{
		private readonly IBookingService _service = service;

		[HttpGet]
		public IActionResult GetAll() 
		{
			var bookings = _service.GetAll();
			if (bookings == null)
				return BadRequest();

			return Ok(bookings);
		}

		[HttpPost]
		public async Task<IActionResult> BookAppointment([FromBody] BookingRequest request)
		{
			if (!ModelState.IsValid)
			{
				Console.WriteLine(DateTime.UtcNow.ToString(), request);
				return BadRequest($"Tiden gick inte att boka.{request}");
			}
			var appointment = _service.SaveAppointment(request);
			if (appointment == null)
				return BadRequest();
			return Ok(appointment);
		}
	}
}
