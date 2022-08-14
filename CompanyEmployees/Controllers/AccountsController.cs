using AutoMapper;
using Castle.Core.Smtp;
using CompanyEmployees.Entities.DataTransferObjects;
using CompanyEmployees.Entities.Models;
using CompanyEmployees.JwtFeatures;
using CompanyEmployees.Repository;
/*using EmailService;*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.IdentityModel.Tokens.Jwt;

namespace CompanyEmployees.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;
/*        private readonly EmailService.IEmailSender _emailSender;
*/        private readonly RepositoryContext _context;

        public AccountsController(UserManager<User> userManager, IMapper mapper, JwtHandler jwtHandler,RepositoryContext context)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
/*            _emailSender = emailSender;
*/            _context = context;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration == null || !ModelState.IsValid)
                return BadRequest();

            var user = _mapper.Map<User>(userForRegistration);
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }
            Email = userForRegistration.Email;
            await _userManager.AddToRoleAsync(user, "Viewer");

            return StatusCode(201);
        }
        /*
        [HttpGet("{id}")]
        public async Task<ActionResult<Issue>> GetIssue(int id)
        {
            if(_userManager.Employees == null)
            {
                return NotFound();
            }
            var issue = await _userManager.userForRegistrationDto.FindAsync(id);

            if (issue == null)
            {
                return NotFound();
            }

            return user;
        }
        */
        /*
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userManager.GetAll();
            return Ok(users);
        }
        */

        public static string? Email;
/*        private EmailService.IEmailSender emailSender;
*/         

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            
            var UserDetail = _context.UserInfo.Where(x => x.UserEmail == userForAuthentication.Email).FirstOrDefault();

            if (UserDetail == null)
                //return Unauthorized(new AuthResponseDto { ErrorMessage = "User not found" });
                return Content("Incorrect Credentials");


            //var EnteringUser = UserDetail.Where(x => x.UserAccepted == true);
            bool IsRegistered = false;
            if (UserDetail.UserAccepted == true)
            {
                IsRegistered=true;
            }

            //bool IsRegistered =new bool(EnteringUser.UserAccepted);

            var user = await _userManager.FindByNameAsync(userForAuthentication.Email);
            
            if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password) || IsRegistered==false)
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });

            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = await _jwtHandler.GetClaims(user);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            Email = userForAuthentication.Email;

            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }
        
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return BadRequest("Invalid Request");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var param = new Dictionary<string, string?>
            {
                {"token", token },
                {"email", forgotPasswordDto.Email }
            };
            var callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientURI, param);
/*            var message = new Message(new string[] { user.Email }, "Reset password token", callback, null);
            await _emailSender.SendEmailAsync(message);*/

            return Ok();
        }
    }
}
