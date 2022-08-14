using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyEmployees.Entities.DataTransferObjects;
using CompanyEmployees.Repository;
using Microsoft.AspNetCore.Authorization;

namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInformationsController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public UserInformationsController(RepositoryContext context)
        {
            _context = context;
        }

        // GET: api/UserInformations
        [Authorize(Roles = "Administrator")]
        [HttpGet("GetUserInfo")]
        public async Task<ActionResult<IEnumerable<UserInformations>>> GetUserInfo()
        {
            var User = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();
            if (_context.UserInfo == null)
            {
                return NotFound();
            }
            return await _context.UserInfo.Where(x=>x.UserEmail!=new string(User.Email)).ToListAsync();
        }

        [HttpGet("GetUserInformations")]
        public async Task<ActionResult<IEnumerable<UserInformations>>> GetUserInformations()
        {
            if (_context.UserInfo == null)
            {
                return NotFound();
            }
            return await _context.UserInfo.ToListAsync();
        }

        // GET: api/UserInformations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInformations>>> GetUsersInfo()
        {
            var User = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();
            if (_context.UserInfo == null)
          {
              return NotFound();
          }
            return await _context.UserInfo.Where(x => x.UserEmail == new string(User.Email)).ToListAsync();
           // userInformations.UserEmail = User.Email;
        }

        /*
        // GET: api/UserInformations/5
        [HttpGet("{groupId}")]
        public async Task<ActionResult<UserInformations>> GetUserInformations(int groupId)
        {
          if (_context.UserInfo == null)
          {
              return NotFound();
          }
            var userInformations = await _context.UserInfo.Where(x=>x.groupId == groupId).ToListAsync();
            

            if (userInformations == null)
            {
                return NotFound();
            }

            return userInformations;
        }*/

        // PUT: api/UserInformations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserInformations(int id, UserInformations userInformations)
        {
            if (id != userInformations.Id)
            {
                return BadRequest();
            }

            //if (UserIdCheck(userInformations.EmpId) || UserEmailCheck(userInformations.UserEmail))
            //{
            //    return BadRequest();
            //}

            _context.Entry(userInformations).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserInformationsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserInformations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserInformations>> PostUserInformations(UserInformations userInformations)
        {
            var User = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();
            userInformations.UserEmail = User.Email;
            if (_context.UserInfo == null)
          {
              return Problem("Entity set 'RepositoryContext.UserInfo'  is null.");
          }
            if (UserIdCheck(userInformations.EmpId))
            {
                return BadRequest();
            }
            _context.UserInfo.Add(userInformations);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserInformations", new { id = userInformations.Id }, userInformations);
        }

        // DELETE: api/UserInformations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserInformations(int id)
        {
            if (_context.UserInfo == null)
            {
                return NotFound();
            }
            var userInformations = await _context.UserInfo.FindAsync(id);
            if (userInformations == null)
            {
                return NotFound();
            }

            _context.UserInfo.Remove(userInformations);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserInformationsExists(int id)
        {
            return (_context.UserInfo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private bool UserIdCheck(string empId)
        {
            return (_context.UserInfo.Any(e => e.EmpId == empId));
        }
        private bool UserEmailCheck(string userEmail)
        {
            return (_context.UserInfo.Any(e => e.UserEmail == userEmail));
        }
    }
}
