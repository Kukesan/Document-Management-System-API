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
    public class GroupMessagesController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public GroupMessagesController(RepositoryContext context)
        {
            _context = context;
        }

        // GET: api/GroupMessages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupMessage>>> GetGroupMessageDetails()
        {
          if (_context.GroupMessageDetails == null)
          {
              return NotFound();
          }
            var userDetails = _context.UserInfo.Where(x => x.UserEmail == AccountsController.Email).FirstOrDefault();

            //var MessageList = _context.GroupMessageDetails.Where(x=>x.GroupId == userDetails.groupId).ToList();

            return await _context.GroupMessageDetails.Where(x => x.GroupId == userDetails.groupId).ToListAsync();
        }

        // GET: api/GroupMessages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupMessage>> GetGroupMessage(int id)
        {
          if (_context.GroupMessageDetails == null)
          {
              return NotFound();
          }
            var groupMessage = await _context.GroupMessageDetails.FindAsync(id);

            if (groupMessage == null)
            {
                return NotFound();
            }

            return groupMessage;
        }

        // PUT: api/GroupMessages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroupMessage(int id, GroupMessage groupMessage)
        {
            if (id != groupMessage.Id)
            {
                return BadRequest();
            }

            _context.Entry(groupMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupMessageExists(id))
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

        // POST: api/GroupMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GroupMessage>> PostGroupMessage(GroupMessage groupMessage)
        {
          if (_context.GroupMessageDetails == null)
          {
              return Problem("Entity set 'RepositoryContext.GroupMessageDetails'  is null.");
          }
            _context.GroupMessageDetails.Add(groupMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroupMessage", new { id = groupMessage.Id }, groupMessage);
        }

        // DELETE: api/GroupMessages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupMessage(int id)
        {
            if (_context.GroupMessageDetails == null)
            {
                return NotFound();
            }
            var groupMessage = await _context.GroupMessageDetails.FindAsync(id);
            if (groupMessage == null)
            {
                return NotFound();
            }

            _context.GroupMessageDetails.Remove(groupMessage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupMessageExists(int id)
        {
            return (_context.GroupMessageDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
