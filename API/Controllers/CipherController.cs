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

                while (System.IO.File.Exists($"{Environment.ContentRootPath}/{name}" + ".sdes"))
                {
                    name = originalName + "(" + i.ToString() + ")";
                    i++;
                }
                var reader = new StreamReader(file.OpenReadStream());
                string text = reader.ReadToEnd();
                reader.Close();
                string cipher = sdes.EncryptFile(text, key);
                byte[] array = Encoding.UTF8.GetBytes(cipher);
                var cipherInfo = new Ciphers();
                cipherInfo.SetAttributes(Environment.ContentRootPath, file.FileName, name);
                Singleton.Instance.HistoryList.Add(cipherInfo);
                return base.File(array, MediaTypeNames.Text.Plain, name + ".sdes");
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
                var text = sdes.DecryptFile(cipher, key);
                byte[] array = Encoding.UTF8.GetBytes(text);
                return base.File(array, MediaTypeNames.Text.Plain, (name.Substring(0, name.Length - 4)) + ".txt");
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        [Route("rsa/keys/{p}/{q}")]
        public IActionResult GenerateKeys(int p, int q)
        {
            try
            {
                var encryptor = new RSA(Environment.ContentRootPath);
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
        [Route("rsa/{nombre}")]
        public IActionResult Cipher([FromForm] IFormFile file, [FromForm] IFormFile key, string nombre)
        {
            try
            {
                if (key.FileName.Substring(key.FileName.LastIndexOf('.')) == ".key")
                {
                    string path = Environment.ContentRootPath + "\\" + file.FileName;
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
                        var encryptor = new RSA(Environment.ContentRootPath);
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
