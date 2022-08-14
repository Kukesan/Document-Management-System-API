using CompanyEmployees.Entities.Models;
using CompanyEmployees.Repository;
using IronOcr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileOcrController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public FileOcrController(RepositoryContext context)
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
        [Route("scandownload")]
        public async Task<IActionResult> scandownload()
        {

            var folderName = Path.Combine("Resources", "Ocr");
            /*string filePath = Path.Combine(Directory.GetCurrentDirectory(), folderName);*/
            string fileName = "Ocr.txt";
            string fullpath = Path.Combine(folderName, fileName);
            /* byte[] fileBytes = System.IO.File.ReadAllBytes(fullpath);

             return File(fileBytes, fullpath);*/
            var memory = new MemoryStream();
            await using (var stream = new FileStream(fullpath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(fullpath), fullpath);


        }

        [HttpDelete("{id}"), DisableRequestSizeLimit]
        [Route("Delete")]


        public async Task<IActionResult> Delete([FromQuery] string fileUrl, Guid Id)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);
            Debug.WriteLine(filePath);
            var user = await _context.Users.FindAsync(Id);
            if (!System.IO.File.Exists(filePath))
                return NotFound();
            else
            {
                {
                    string ExitingFile = filePath;
                    System.IO.File.Delete(ExitingFile);
                    try
                    {
                        _context.Users.Remove(user);
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
        [HttpPost("{fileUrl}"), DisableRequestSizeLimit]
        [Route("Scan")]


        public async Task<IActionResult> Scan([FromQuery] string fileUrl)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl);
            if (!System.IO.File.Exists(filePath))
                return NotFound();
            else
            {
                {
                    try
                    {
                        var ocr = new IronTesseract();
                        using (var input = new OcrInput(filePath))
                        {
                            var result = ocr.Read(input);
                            var folderName = Path.Combine("Resources", "Ocr");
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                            var fileName = Path.GetFileName(pathToSave);
                            var fullPath = Path.Combine(pathToSave, fileName + ".txt");
                            result.SaveAsTextFile(fullPath);
                            return Ok();
                        }


                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Internal server error: {ex}");
                    }


                }

            }
            return Ok();


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
    }
}
