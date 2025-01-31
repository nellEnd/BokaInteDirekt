using BokaInteDirekt.DTO;
using BokaInteDirekt.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BokaInteDirekt.Controllers
{
    [Route("api/smtp/[action]")]
    [ApiController]
    public class SmtpEmailController(EmailService service) : ControllerBase
    {
        private readonly EmailService _service = service;

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            await _service.SendEmail(request.Receiver, request.Subject, request.Body);
            return Ok("Email successfully sent!");
        }

    }
}
