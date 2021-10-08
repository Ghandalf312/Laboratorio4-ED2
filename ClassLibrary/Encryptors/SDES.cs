using System;
using System.Collections.Generic;
using System.Text;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Encryptors
{
    public interface Encryptor<T> where T : IKeyHolder
    {
        string EncryptString(string text, T Key);
        string DecryptString(string text, T Key);
    }
    public class SDES<T> : Encryptor<T> where T : IKeyHolder
    {
        public string EncryptString(string text, T Key)
        {
            var key = Key.GetSDESKey();
            return "";
        }

        public string DecryptString(string text, T Key)
        {
            return "";
        }
    }
}
