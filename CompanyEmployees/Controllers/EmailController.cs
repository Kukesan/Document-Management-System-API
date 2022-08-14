using CompanyEmployees.Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CompanyEmployees.Services.EmailService;

namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
            private readonly IEmailService _emailService;

            public EmailController(IEmailService emailService)
            {
                _emailService = emailService;
            }

            [HttpPost]
            public IActionResult SendEmail(EmailDto request)
            {
            request.To="kkukesan@gmail.com";
            request.Body = "Dear admin," +
            "Kindly please reset my " + request.Subject + " account password." +
            "Thank you.";
            _emailService.SendEmail(request);
                return Ok();
            }
    }
}
