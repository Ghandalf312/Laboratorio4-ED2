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
using ClassLibrary.Helpers;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptorController : ControllerBase
    {
        readonly IWebHostEnvironment env;

        public EncryptorController(IWebHostEnvironment _env)
        {
            env = _env;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return NoContent();
        }

        [HttpGet]
        [Route("/api/rsa/keys/{p}/{q}")]
        public IActionResult GenerateKeys(int p, int q)
        {
            try
            {
                var encryptor = new RSA(env.ContentRootPath);
                string path = encryptor.GenerateKeys(p, q);
                if (path != "")
                {
                    FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
                    return File(fileStream, "application/zip");
                }
                else
                    return StatusCode(500, "La llave no es valida");
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("/api/rsa/{nombre}")]
        public IActionResult Cipher([FromForm] IFormFile file, [FromForm] IFormFile key, string nombre)
        {
            try
            {
                if (key.FileName.Substring(key.FileName.LastIndexOf('.')) == ".key")
                {
                    string path = env.ContentRootPath + "\\" + file.FileName;
                    using var saver = new FileStream(path, FileMode.Create);
                    file.CopyTo(saver);
                    saver.Close();
                    using var fileWritten = new FileStream(path, FileMode.OpenOrCreate);
                    using var reader = new BinaryReader(fileWritten);
                    byte[] buffer = new byte[0];
                    while (fileWritten.Position < fileWritten.Length)
                    {
                        int index = buffer.Length;
                        Array.Resize<byte>(ref buffer, index + 100000);
                        byte[] aux = reader.ReadBytes(100000);
                        aux.CopyTo(buffer, index);
                    }
                    reader.Close();
                    fileWritten.Close();
                    for (int i = buffer.Length - 1; i >= 0; i--)
                    {
                        if (buffer[i] != 0)
                        {
                            Array.Resize<byte>(ref buffer, i + 1);
                            break;
                        }
                    }
                    if (buffer.Length > 0)
                    {
                        using var content = new MemoryStream();
                        key.CopyTo(content);
                        var text = Encoding.ASCII.GetString(content.ToArray());
                        var encryptor = new RSA(env.ContentRootPath);
                        path = encryptor.EncryptFile(buffer, new Key(text), nombre);
                        if (path != "")
                        {
                            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
                            return File(fileStream, "text/plain");
                        }
                        else
                            return StatusCode(500, "La llave no es valida");
                    }
                    else
                        return StatusCode(500, "El archivo está vacío");
                }
                else
                    return StatusCode(500, "La llave no es un archivo .key");
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
