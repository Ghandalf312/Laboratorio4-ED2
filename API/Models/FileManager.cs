using ClassLibrary.Encryptors;
using ClassLibrary.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class FileManager
    {
        public static async Task<string> SaveFileAsync(IFormFile file, string path)
        {
            if (!Directory.Exists($"{path}/Uploads"))
            {
                Directory.CreateDirectory($"{path}/Uploads");
            }
            using var saver = new FileStream($"{path}/Uploads/{file.FileName}", FileMode.OpenOrCreate);
            await file.CopyToAsync(saver);
            saver.Close();
            return $"{path}/Uploads/{file.FileName}";
        }

        public static FileProperties Cipher(string directory, string filePath, string method, KeyHolder key)
        {
            var encryptedFileProperties = new FileProperties();
            var savingPath = $"{directory}/Ciphers";
            if (!Directory.Exists(savingPath))
            {
                Directory.CreateDirectory(savingPath);
            }
            switch (method.ToLower())
            {
                case "cesar":
                    var cesarEncryptor = new CrypCesar<KeyHolder>();
                    encryptedFileProperties.Path = cesarEncryptor.EncryptFile(savingPath, filePath, key);
                    encryptedFileProperties.FileType = ".csr";
                    break;
                case "zigzag":
                    var zigzagEncryptor = new CrypZigZag<KeyHolder>();
                    encryptedFileProperties.Path = zigzagEncryptor.EncryptFile(savingPath, filePath, key);
                    encryptedFileProperties.FileType = ".zz";
                    break;
            }
            return encryptedFileProperties;
        }

        public static string Decipher(string folderPath, string filePath, KeyHolder key)
        {
            var filetype = Path.GetExtension(filePath);
            var savingPath = $"{folderPath}/Deciphers";
            if (!Directory.Exists(savingPath))
            {
                Directory.CreateDirectory(savingPath);
            }
            switch (filetype)
            {
                case ".csr":
                    var cesarDecryptor = new CrypCesar<KeyHolder>();
                    return cesarDecryptor.DecryptFile(savingPath, filePath, key);
                case ".zz":
                    var zigzagDecryptor = new CrypZigZag<KeyHolder>();
                    return zigzagDecryptor.DecryptFile(savingPath, filePath, key);
            }
            return string.Empty;
        }
    }
}
