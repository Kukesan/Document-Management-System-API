using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyEmployees.Entities.Models;
using CompanyEmployees.Repository;
using Microsoft.AspNetCore.Authorization;

namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize]
    public class IssuesController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public IssuesController(RepositoryContext context)
        {
            _context = context;
        }

        // GET: api/Issues
        [HttpGet("AdminGetIssuesDetails")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<Issue>>> AdminGetIssuesDetails()
        {
            if (_context.IssuesDetails == null)
            {
                return NotFound();
            }
            //return await _context.IssuesDetails.ToListAsync();

            return await _context.IssuesDetails.ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Issue>>> GetIssuesDetails()
        {
          var User = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();
          if (_context.IssuesDetails == null)
          {
              return NotFound();
          }
            //return await _context.IssuesDetails.ToListAsync();

           return await _context.IssuesDetails.Where(x => x.Id == new Guid(User.Id)).ToListAsync();
        }

        // GET: api/Issues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Issue>> GetIssue(int id)
        {
          if (_context.IssuesDetails == null)
          {
              return NotFound();
          }
            var issue = await _context.IssuesDetails.FindAsync(id);

            if (issue == null)
            {
                return NotFound();
            }
            return issue;
        }

        // PUT: api/Issues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIssue(int id, Issue issue)
        {
            if (id != issue.IssueId)
            {
                return BadRequest();
            }

            _context.Entry(issue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(id))
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

        // POST: api/Issues
        // To protect from overposting attacks, see https://go.microsoft.com/fwlontext.ink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Issue>> PostIssue(Issue issue)
        {
            var User = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();
            issue.Id = new Guid(User.Id);
            issue.UserEmail = new string(User.Email);
            if (_context.IssuesDetails == null)
          {               
              return Problem("Entity set 'RepositoryContext.IssuesDetails'  is null.");
          }
            _context.IssuesDetails.Add(issue);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIssue", new { id = issue.IssueId }, issue);
        }

        // DELETE: api/Issues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssue(int id)
        {
            if (_context.IssuesDetails == null)
            {
                return NotFound();
            }
            var issue = await _context.IssuesDetails.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            _context.IssuesDetails.Remove(issue);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IssueExists(int id)
        {
            return (_context.IssuesDetails?.Any(e => e.IssueId == id)).GetValueOrDefault();
        }
    }
}
