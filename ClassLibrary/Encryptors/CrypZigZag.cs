using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Encryptors
{
    public class CrypZigZag<T> : IEncryptor<T> where T : IKeyHolder
    {
        public string EncryptFile(string savingPath, string completeFilePath, T key)
        {
            return "";
        }

        public string EncryptString(string text, T Key)
        {
            return "";
        }

        public string DecryptFile(string savingPath, string completeFilePath, T key)
        {
            return "";
        }

        public string DecryptString(string text, T Key)
        {
            return "";
        }
    }
}
