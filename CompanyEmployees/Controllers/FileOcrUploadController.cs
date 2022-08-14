using CompanyEmployees.Entities.Models;
using CompanyEmployees.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileOcrUploadController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public FileOcrUploadController(RepositoryContext context) => _context = context;

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var user = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();
                var userFiles = await _context.FileUploadDetails.Where(s => s.Status == true && s.UserEmail == user.Email).ToListAsync();

                return Ok(userFiles);
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
