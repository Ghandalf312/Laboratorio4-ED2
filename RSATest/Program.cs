using System;
using ClassLibrary.Encryptors;

namespace RSATest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Ingrese el texto por cifrar:");
                string text = Console.ReadLine();
                Console.WriteLine("Ingrese p:");
                int p = int.Parse(Console.ReadLine());
                Console.WriteLine("Ingrese q:");
                int q = int.Parse(Console.ReadLine());
                var rsa = new RSA("..//..//..");
                int n = rsa.N(p, q);
                Console.WriteLine("n: " + n);
                int fi = rsa.Fi(p, q);
                Console.WriteLine("fi: " + fi);
                int e = rsa.E(fi);
                Console.WriteLine("e: " + e);
                int d = rsa.D(e, fi);
                Console.WriteLine("d: " + d);
                Console.WriteLine("El texto cifrado es:");
                string cipher = rsa.EncryptString(text, n, e);
                Console.WriteLine(cipher);
                Console.WriteLine("El texto descifrado es:");
                Console.WriteLine(rsa.EncryptString(cipher, n, d));
                Console.ReadKey();
            }
            catch
            {
                Console.WriteLine("Ha ocurrido un error.");
                Main(args);
            }
        }
    }
}
