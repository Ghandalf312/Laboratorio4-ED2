using System;
using System.Collections.Generic;
using System.Text;
using ClassLibrary.Helpers;
namespace ClassLibrary.Interfaces
{
    public interface IEncryptorRSa
    {
        public abstract string Cipher(byte[] content, Key key, string name);
    }
}
