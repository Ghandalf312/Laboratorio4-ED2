using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ClassLibrary.Interfaces;
using System.IO;

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
            var height = key.GetZigZagKey();
            var fillingByte = Convert.ToByte(Fillingchar);
            List<byte>[] arrayY = new List<byte>[height];
            int k = 0;
            using var fileForReading = new FileStream(completeFilePath, FileMode.Open);
            using var reader = new BinaryReader(fileForReading);
            var buffer = new byte[2000];
            var fileRoute = $"{savingPath}/{Path.GetFileNameWithoutExtension(completeFilePath) + ".zz"}";
            using var fileForWriting = new FileStream(fileRoute, FileMode.OpenOrCreate);
            using var writer = new BinaryWriter(fileForWriting);
            for (int i = 0; i < height; i++)
            {
                arrayY[i] = new List<byte>();
            }
            while (fileForReading.Position != fileForReading.Length)
            {
                k = 0;
                buffer = reader.ReadBytes(buffer.Length);
                while (k < buffer.Length)
                {
                    for (int i = 0; i < height - 1; i++)
                    {
                        if (k < buffer.Length)
                        {
                            arrayY[i].Add(buffer[k]);
                            k++;
                        }
                        else if (!(fileForReading.Position != fileForReading.Length))
                        {
                            arrayY[i].Add(fillingByte);
                        }
                        else
                        {
                            k = 0;
                            buffer = reader.ReadBytes(buffer.Length);
                            arrayY[i].Add(buffer[k]);
                            k++;
                        }
                    }
                    for (int j = height - 1; j > 0; j--)
                    {
                        if (k < buffer.Length)
                        {
                            arrayY[j].Add(buffer[k]);
                            k++;
                        }
                        else if (!(fileForReading.Position != fileForReading.Length))
                        {
                            arrayY[j].Add(fillingByte);
                        }
                        else
                        {
                            k = 0;
                            buffer = reader.ReadBytes(buffer.Length);
                            arrayY[j].Add(buffer[k]);
                            k++;
                        }
                    }
                }
            }
            for (int i = 0; i < height; i++)
            {
                foreach (var item in arrayY[i])
                {
                    writer.Write(item);
                }
            }
            reader.Close();
            fileForReading.Close();
            writer.Close();
            fileForWriting.Close();
            return fileRoute;
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
            var height = key.GetZigZagKey();
            List<byte>[] arrayY = new List<byte>[height];
            int n = 0;
            using var fileForReading = new FileStream(completeFilePath, FileMode.Open);
            using var reader = new BinaryReader(fileForReading);
            var buffer = new byte[2000];
            var fileRoute = $"{savingPath}/{Path.GetFileNameWithoutExtension(completeFilePath) + ".txt"}";
            using var fileForWriting = new FileStream(fileRoute, FileMode.OpenOrCreate);
            using var writer = new BinaryWriter(fileForWriting);
            for (int i = 0; i < height; i++)
            {
                arrayY[i] = new List<byte>();
            }
            while (fileForReading.Position != fileForReading.Length)
            {
                buffer = reader.ReadBytes(buffer.Length);
                bool flag = true;
                int m = GetM(Convert.ToInt32(fileForReading.Length), height);
                for (int j = 0; j < height; j++)
                {
                    if (j == 0 || j == height - 1)
                    {
                        for (int k = 0; k < m; k++)
                        {
                            if (n < buffer.Length)
                            {
                                arrayY[j].Add(buffer[n]);
                                n++;
                            }
                            else
                            {
                                n = 0;
                                buffer = reader.ReadBytes(buffer.Length);
                                arrayY[j].Add(buffer[n]);
                                n++;
                            }

                        }
                    }
                    else
                    {
                        for (int k = 0; k < (2 * m); k++)
                        {
                            if (n < buffer.Length)
                            {
                                arrayY[j].Add(buffer[n]);
                                n++;
                            }
                            else
                            {
                                n = 0;
                                buffer = reader.ReadBytes(buffer.Length);
                                arrayY[j].Add(buffer[n]);
                                n++;
                            }
                        }
                    }
                }
                while (flag)
                {
                    for (int i = 0; i < height - 1; i++)
                    {
                        if (arrayY[i].Count != 0)
                        {
                            if (arrayY[i][0].Equals(Convert.ToByte('*')))
                            {
                                flag = false;
                            }
                            else
                            {
                                writer.Write(arrayY[i][0]);
                                arrayY[i].RemoveAt(0);
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    for (int j = height - 1; j > 0; j--)
                    {
                        if (arrayY[j].Count != 0)
                        {
                            if (arrayY[j][0].Equals(Convert.ToByte('*')))
                            {
                                flag = false;
                            }
                            else
                            {
                                writer.Write(arrayY[j][0]);
                                arrayY[j].RemoveAt(0);
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                }

            }
            reader.Close();
            fileForReading.Close();
            writer.Close();
            fileForWriting.Close();
            return fileRoute;
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
