using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyEmployees.Entities.Models;
using CompanyEmployees.Repository;

namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActivitiesController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public UserActivitiesController(RepositoryContext context)
        {
            _context = context;
        }

        // GET: api/UserActivities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserActivity>>> GetUserActivityDetails()
        {
          if (_context.UserActivityDetails == null)
          {
              return NotFound();
          }
            // return await _context.UserActivityDetails.ToListAsync();
            var max = _context.UserActivityDetails.OrderByDescending(p => p.Id).FirstOrDefault().Id;
            //var max = _context.UserActivityDetails.Max(p => p.Id);
            return Ok(max);
        }

        [HttpGet("GetAllActivityDetails")]
        public async Task<ActionResult<IEnumerable<UserActivity>>> GetAllActivityDetails()
        {
            //var User = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();
            if (_context.UserActivityDetails == null)
            {
                return NotFound();
            }
            //eturn await _context.UserActivityDetails.ToListAsync();
            return await _context.UserActivityDetails.Where(x => x.UserEmail != AccountsController.Email).ToListAsync();
        }

        // GET: api/UserActivities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserActivity>> GetUserActivity(int id)
        {
          if (_context.UserActivityDetails == null)
          {
              return NotFound();
          }
            var userActivity = await _context.UserActivityDetails.FindAsync(id);

            if (userActivity == null)
            {
                return NotFound();
            }

            return userActivity;
        }

        // PUT: api/UserActivities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserActivity(int id, UserActivity userActivity)
        {
            if (id != userActivity.Id)
            {
                return BadRequest();
            }
            var UserActivityDetails = _context.UserActivityDetails.Where(p => p.Id == userActivity.Id).FirstOrDefault();

            UserActivityDetails.LogoutTime = userActivity.LogoutTime;
            //_context.Entry(userActivity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserActivityExists(id))
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

        // POST: api/UserActivities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserActivity>> PostUserActivity(UserActivity userActivity)
        {
          if (_context.UserActivityDetails == null)
          {
              return Problem("Entity set 'RepositoryContext.UserActivityDetails'  is null.");
          }
            _context.UserActivityDetails.Add(userActivity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserActivity", new { id = userActivity.Id }, userActivity);
        }

        // DELETE: api/UserActivities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserActivity(int id)
        {
            if (_context.UserActivityDetails == null)
            {
                return NotFound();
            }
            var userActivity = await _context.UserActivityDetails.FindAsync(id);
            if (userActivity == null)
            {
                return NotFound();
            }

            _context.UserActivityDetails.Remove(userActivity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserActivityExists(int id)
        {
            return (_context.UserActivityDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
