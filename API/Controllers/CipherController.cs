using API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ClassLibrary.Encryptors;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    public class CipherController : ControllerBase
    {
        private IWebHostEnvironment Environment;

        public CipherController(IWebHostEnvironment env)
        {
            Environment = env;
        }

        // GET: api/<CipherController>
        [Route("cipher")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("cipher/{method}")]
        [HttpPost]
        public async Task<IActionResult> Cipher([FromForm] IFormFile file, string method, [FromForm]KeyHolder key)
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

        [Route("sdes/cipher/{name}")]
        [HttpPost]
        public FileResult CipherSDES([FromForm] IFormFile file, string name, [FromForm] KeyHolder key)
        {
            try
            {
                var sdes = new SDES<KeyHolder>();
                if (!KeyHolder.CheckKeyValidness("sdes", key))
                {
                    return null;
                }
                int i = 1;
                var originalName = name;
                while (System.IO.File.Exists($"{Environment.ContentRootPath}/{name}"))
                {
                    name = originalName + "(" + i.ToString() + ")";
                    i++;
                }
                var reader = new StreamReader(file.OpenReadStream());
                string text = reader.ReadToEnd();
                reader.Close();
                string cipher = sdes.EncryptString(text, key);
                byte[] array = Encoding.UTF8.GetBytes(cipher);

                var cipherInfo = new Ciphers();
                cipherInfo.SetAttributes(Environment.ContentRootPath, file.FileName, name);
                Singleton.Instance.HistoryList.Add(cipherInfo);
                return base.File(array, MediaTypeNames.Text.Plain, name + ".sdes");
                //return PhysicalFile($"{Environment.ContentRootPath}/{name}", MediaTypeNames.Text.Plain, $"{name}.sdes");
            }
            catch
            {
                return null;
            }
        }

        [Route("decipher")]
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

        [Route("sdes/decipher")]
        [HttpPost]
        public async Task<FileResult> DecipherSDES([FromForm] IFormFile file, [FromForm] KeyHolder key)
        {
            try
            {
                byte[] bytes;
                var sdes = new SDES<KeyHolder>();

                if (!KeyHolder.CheckKeyFromFileType(".sdes", key))
                {
                    return null;
                }
                Ciphers.LoadHistList(Environment.ContentRootPath);
                var name = "";
                foreach (var item in Singleton.Instance.HistoryList)
                {
                    if (item.CompressedName == file.FileName.Substring(0, (file.FileName.Length) - 5))
                    {
                        name = item.OriginalName;
                    }
                }
                using (var memory = new MemoryStream())
                {
                    await file.CopyToAsync(memory);
                    bytes = memory.ToArray();
                    List<byte> aux = bytes.OfType<byte>().ToList();
                }
                var cipher = Encoding.UTF8.GetString(bytes);
                var text = sdes.DecryptString(cipher, key);
                byte[] array = Encoding.UTF8.GetBytes(text);
                return base.File(array, MediaTypeNames.Text.Plain, (name.Substring(0, name.Length - 4)) + ".txt");
            }
            catch
            {
                return null;
            }
        }
    }
}
