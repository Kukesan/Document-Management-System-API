using CompanyEmployees.Entities.Models;
using CompanyEmployees.Repository;
using Microsoft.AspNetCore.Cors;
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
    public class FileController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public FileController(RepositoryContext context)
        {
            _context = context;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Route("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpGet, DisableRequestSizeLimit]
        [Route("Index")]
        public async Task<IActionResult> Index(string docsearch)
        {
            try
            {
                var docs = from x in _context.FileUploadDetails
                           select x;
                if (!String.IsNullOrEmpty(docsearch))
                {
                    docs = docs.Where(s => s.Name.Contains(docsearch));
                }
                var d = docs.FirstOrDefault();
                var userdata = new FileUpload
                {
                    Id = d.Id,
                    Name = d.Name,
                    Address = d.Address,
                    ImgPath = d.ImgPath
                };
                Debug.WriteLine(userdata);
                return Ok(userdata);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("download")]
        public async Task<IActionResult> Download([FromQuery] string fileUrl)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), filePath);
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("getPhotos")]
        public IActionResult GetPhotos()
        {
            try
            {
                var folderName = Path.Combine("Resources", "Images");
                var pathToRead = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var photos = Directory.EnumerateFiles(pathToRead)
                    .Where(IsAPhotoFile)
                    .Select(fullPath => Path.Combine(folderName, Path.GetFileName(fullPath)));

                return Ok(new { photos });

                //var v = Directory.EnumerateFiles(pathToRead)
                //    .Select(User);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        private bool IsAPhotoFile(string fileName)
        {
            return fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
        [HttpPut("{id}"), DisableRequestSizeLimit]
        [Route("Change")]
        public async Task<IActionResult> Change(Guid id, FileUpload fileUpload)
        {
            if (id != fileUpload.Id)
            {
                return BadRequest();
            }

            _context.Entry(fileUpload).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileExists(id))
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

       /* [HttpGet("{folderId}")]
        [Route("GetSpecFiles")]
        public async Task<ActionResult<FileUpload>> GetSpecFiles([FromQuery] int folderId)
        {
            if (_context.FileUploadDetails == null)
            {
                return NotFound();
            }
            var issue = await _context.FileUploadDetails.FindAsync(folderId);

            if (issue == null)
            {
                return NotFound();
            }
            return issue;
            //return await _context.FileUploadDetails.Where(x => x.folderId == folderId).ToListAsync());
        }*/


        [HttpDelete("{Id}"), DisableRequestSizeLimit]
        [EnableCors("AllowOrigin")]
        [Route("Delete")]
        public async Task<IActionResult> Delete([FromQuery] Guid Id)
        {

            var user = await _context.FileUploadDetails.FindAsync(Id);
            Debug.WriteLine(user.ImgPath);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), user.ImgPath);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            else
            {
                {
                    string ExitingFile = filePath;
                    System.IO.File.Delete(ExitingFile);
                    try
                    {
                        _context.FileUploadDetails.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Internal server error: {ex}");
                    }

                }
            }



            return Ok();


        }

        [HttpPut("Change/{Id}")]
        public async Task<IActionResult> Change(Guid Id)
        {
            var user = await _context.FileUploadDetails.FindAsync(Id);
            user.Status = false;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("Restore/{Id}")]

        public async Task<IActionResult> Restore(Guid Id)
        {
            var user = await _context.FileUploadDetails.FirstOrDefaultAsync(m => m.Id == Id);
            user.Status = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool FileExists(Guid id)
        {
            return (_context.FileUploadDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
  

