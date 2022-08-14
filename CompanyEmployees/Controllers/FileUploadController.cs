using CompanyEmployees.Entities.Models;
using CompanyEmployees.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public FileUploadController(RepositoryContext context) => _context = context;

        [HttpGet("all-user")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var user = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();
                var userFiles = await _context.FileUploadDetails.Where(s=>s.Status==true && s.userId == user.Id).ToListAsync();

                return Ok(userFiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }

        [HttpGet("all-admin")]
        public async Task<IActionResult> AdminGetAllUsers()
        {
            try
            {
                var users = await _context.FileUploadDetails.Where(s => s.Status == true).ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet]
        [Route("GetReUsers")]
        public async Task<IActionResult> GetReUsers()
        {
            try
            {
                /*var docs = from x in _context.FileUploadDetails
                           select x;*/

                var users = await _context.FileUploadDetails.Where(s => s.Status == false).ToListAsync();


                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] FileUpload user)
        {
            var User = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();
            user.UserEmail = AccountsController.Email;

            user.userId = User.Id;
            try
            {
                if (user is null)
                {
                    return BadRequest("User object is null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model object");
                }

                user.Id = Guid.NewGuid();
                _context.Add(user);
                await _context.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

    }
}
