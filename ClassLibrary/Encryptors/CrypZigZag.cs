using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Encryptors
{
    public class CrypZigZag<T> : IEncryptor<T> where T : IKeyHolder
    {
        public int height = 0;
        string encrypted = "";
        string decrypted = "";
        public char Fillingchar = '$';
        public CrypZigZag() { }
        public string EncryptFile(string savingPath, string completeFilePath, T key)
        {
            return "";
        }

        public string EncryptString(string text, T Key)
        {
            var height = Key.GetZigZagKey();
            List<char>[] ArrayY = new List<char>[height];
            for (int i = 0; i < height; i++)
            {
                ArrayY[i] = new List<char>();
            }
            while (text.Length > 0)
            {
                for (int i = 0; i < height - 1; i++)
                {
                    if (text.Length > 0)
                    {
                        ArrayY[i].Add(text[0]);
                        text = text.Remove(0, 1);
                    }
                    else
                    {
                        ArrayY[i].Add(Fillingchar);

                    }
                }
                for (int j = height - 1; j > 0; j--)
                {
                    if (text.Length > 0)
                    {
                        ArrayY[j].Add(text[0]);
                        text = text.Remove(0, 1);
                    }
                    else
                    {
                        ArrayY[j].Add(Fillingchar);
                    }
                }

            }
            for (int i = 0; i < height; i++)
            {
                foreach (var item in ArrayY[i])
                {
                    encrypted += item;
                }
            };
            return encrypted;
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
