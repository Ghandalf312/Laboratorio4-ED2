using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Interfaces
{
    interface IEncryptor<T> where T : IKeyHolder
    {
        string EncryptFile(string savingPath, string completeFilePath, T key);
        string DecryptFile(string savingPath, string completeFilePath, T key);
        string EncryptString(string text, T Key);
        string DecryptString(string text, T Key);
    }
}
