using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Net.Mime;
using API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CipherController : ControllerBase
    {
        private IWebHostEnvironment Environment;

        public CipherController(IWebHostEnvironment env)
        {
            Environment = env;
        }

        // GET: api/<CipherController>
        [Route("/cipher")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("/api/cipher/{method}")]
        [HttpPost]
        public async Task<IActionResult> Cipher([FromForm] IFormFile file, string method, [FromForm] KeyHolder key)
        {
            try
            {
                var uploadedFilePath = await FileManager.SaveFileAsync(file, Environment.ContentRootPath);
                if (!KeyHolder.CheckKeyValidness(method, key))
                {
                    return StatusCode(500, "La llave ingresada es incorrecta");
                }
                var returningFile = FileManager.Cipher(Environment.ContentRootPath, uploadedFilePath, method, key);
                return PhysicalFile(returningFile.Path, MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(uploadedFilePath)}{returningFile.FileType}");
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [Route("/api/decipher")]
        [HttpPost]
        public async Task<IActionResult> Decipher([FromForm] IFormFile file, [FromForm] KeyHolder key)
        {
            try
            {
                var uploadedFilePath = await FileManager.SaveFileAsync(file, Environment.ContentRootPath);
                if (!KeyHolder.CheckKeyFromFileType(uploadedFilePath, key))
                {
                    return StatusCode(500, "La llave ingresada es incorrecta");
                }
                var returningFile = FileManager.Decipher(Environment.ContentRootPath, uploadedFilePath, key);
                return PhysicalFile(returningFile, MediaTypeNames.Text.Plain);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
