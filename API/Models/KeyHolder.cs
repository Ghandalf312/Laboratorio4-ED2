using ClassLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;


namespace API.Models
{
    public class KeyHolder : IKeyHolder
    {
        public string Word { get; set; }
        public int Levels { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public string GetCesarKey() { return Word; }

        public static bool CheckKeyValidness(string method, KeyHolder key)
        {
            switch (method.ToLower())
            {
                case "cesar":
                    if (key.Word == null || key.Word == string.Empty)
                    {
                        return false;
                    }
                    foreach (var item in key.Word)
                    {
                        if ((byte)item < 65 || (byte)item > 90 && (byte)item < 97 || (byte)item > 122)
                        {
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }

        public static bool CheckKeyFromFileType(string path, KeyHolder key)
        {
            switch (Path.GetExtension(path))
            {
                case ".csr":
                    if (key.Word == null || key.Word == string.Empty)
                    {
                        return false;
                    }
                    foreach (var item in key.Word)
                    {
                        if ((byte)item < 65 || (byte)item > 90 && (byte)item < 97 || (byte)item > 122)
                        {
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }

        public bool SetKeyFromString(string method, string key)
        {
            switch (method.ToLower())
            {
                case "cesar":
                    Word = key;
                    return CheckKeyValidness(method.ToLower(), this);
                default:
                    return false;
            }
        }
    }
}
