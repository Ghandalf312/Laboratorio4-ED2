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
            var height = Key.GetZigZagKey();
            var mLenght = text.Length;
            List<char>[] ArrayY = new List<char>[height];
            bool flag = true;
            for (int i = 0; i < height; i++)
            {
                ArrayY[i] = new List<char>();
            }
            int m = GetM(mLenght, height);
            for (int j = 0; j < height; j++)
            {
                if (j == 0 || j == height - 1)
                {
                    for (int k = 0; k < m; k++)
                    {
                        ArrayY[j].Add(text[0]);
                        text = text.Remove(0, 1);
                    }
                }
                else
                {
                    for (int k = 0; k < (2 * m); k++)
                    {
                        ArrayY[j].Add(text[0]);
                        text = text.Remove(0, 1);
                    }
                }
            }
            while (flag)
            {
                for (int i = 0; i < height - 1; i++)
                {
                    if (ArrayY[i][0].Equals('$'))
                    {
                        flag = false;
                    }
                    else
                    {
                        decrypted += ArrayY[i][0];
                        ArrayY[i].RemoveAt(0);
                    }
                }
                for (int j = height - 1; j > 0; j--)
                {
                    if (ArrayY[j][0].Equals('$'))
                    {
                        flag = false;
                    }
                    else
                    {
                        decrypted += ArrayY[j][0];
                        ArrayY[j].RemoveAt(0);
                    }
                }
            }
            return decrypted;
        }

        // Metodo ayuda para obtener la cantidad de valores altos y bajos
        public int GetM(int mLenght, int height)
        {
            int m = (mLenght / (2 + 2 * (height - 2)));
            return m;
        }
    }
}
