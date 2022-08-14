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
    public class FoldersController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public FoldersController(RepositoryContext context)
        {
            _context = context;
        }

        // GET: api/Folders
        [HttpGet]
        [Route("AdminGetFolder")]
        public async Task<ActionResult<IEnumerable<Folder>>> AdminGetFolder()
        {
          if (_context.Folder == null)
          {
              return NotFound();
          }
            return await _context.Folder.ToListAsync();
        }

        [HttpGet]
        [Route("GetFolder")]
        public async Task<ActionResult<IEnumerable<Folder>>> GetFolder()
        {
            if (_context.Folder == null)
            {
                return NotFound();
            }
            var User = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();


            return await _context.Folder.Where(x=>x.userId == User.Id).ToListAsync();
        }

        // GET: api/Folders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Folder>> GetFolder(int id)
        {
          if (_context.Folder == null)
          {
              return NotFound();
          }
            var folder = await _context.Folder.FindAsync(id);

            if (folder == null)
            {
                return NotFound();
            }

            return folder;
        }

        // PUT: api/Folders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFolder(int id, Folder folder)
        {
            if (id != folder.folderId)
            {
                return BadRequest();
            }

            _context.Entry(folder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FolderExists(id))
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

        // POST: api/Folders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Folder>> PostFolder(Folder folder)
        {
           var User = _context.Users.Where(x => x.Email == AccountsController.Email).FirstOrDefault();
            folder.UserEmail = AccountsController.Email;

            folder.userId = User.Id;


            // folder.UserEmail = new string(User.Email);
            if (_context.Folder == null)
          {
              return Problem("Entity set 'RepositoryContext.Folder'  is null.");
          }
            _context.Folder.Add(folder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFolder", new { id = folder.folderId }, folder);
        }

        // DELETE: api/Folders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            if (_context.Folder == null)
            {
                return NotFound();
            }
            var folder = await _context.Folder.FindAsync(id);
            if (folder == null)
            {
                return NotFound();
            }

            _context.Folder.Remove(folder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FolderExists(int id)
        {
            return (_context.Folder?.Any(e => e.folderId == id)).GetValueOrDefault();
        }
    }
}
