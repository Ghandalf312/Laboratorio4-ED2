using System;
using System.Collections.Generic;
using System.Text;
using ClassLibrary.Helpers;
using System.Numerics;

namespace ClassLibrary.Encryptors
{
    public interface IEncryptor
    {
        public abstract string EncryptFile(byte[] content, Key key, string name);
        public abstract string EncryptString(string text, int n, int x);
    }
    public class RSA
    {
        private readonly string Path;

        public RSA(string path) { Path = path; }

        public string GenerateKeys(int p, int q)
        {
            return "";
        }

        public string EncryptString(string text, int n, int x)
        {
            return ConvertToString(EncryptString(ConvertToIntList(ConvertToByteArray(text), n), n, x));
        }

        private byte[] EncryptString(List<int> text, int n, int x)
        {
            List<int> rsa = new List<int>();
            foreach (var item in text)
            {
                int aux = 1;
                for (int i = 0; i < x; i++)
                {
                    aux *= item;
                    aux %= n;
                }
                rsa.Add(aux);
            }
            return ConvertToByteArray(rsa, n);
        }
        public int N(int p, int q)
        {
            return p * q;
        }

        public int Fi(int p, int q)
        {
            return (p - 1) * (q - 1);
        }

        public int E(int fi)
        {
            for (int i = 2; i < fi; i++)
            {
                if (IsPrime(i) && fi % i != 0)
                    return i;
            }
            return 0;
        }

        public int D(int e, int fi)
        {
            int j = 1;
            for (int i = 0; i < fi; i++)
            {
                if (j % e == 0)
                    return j / e;
                j += fi;
            }
            return 0;
        }

        private bool IsPrime(int n)
        {
            for (int i = 2; i < n; i++)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }
        private List<int> ConvertToIntList(byte[] array, int n)
        {
            BigInteger value = new BigInteger(array);
            List<int> list = new List<int>();
            while (value > 0)
            {
                list.Add((int)(value % n));
                value /= n;
            }
            return list;
        }

        private byte[] ConvertToByteArray(List<int> list, int n)
        {
            BigInteger value = 0;
            while (list.Count > 0)
            {
                value *= n;
                value += list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
            }
            return value.ToByteArray();
        }

        private string ConvertToString(byte[] array)
        {
            string text = "";
            foreach (var item in array)
                text += Convert.ToString(Convert.ToChar(item));
            return text;
        }

        private byte[] ConvertToByteArray(string text)
        {
            byte[] array = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
                array[i] = Convert.ToByte(text[i]);
            return array;
        }
    }
}
