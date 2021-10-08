using System;
using System.Threading;
using API.Models;
using ClassLibrary.Encryptors;

namespace CryptoTest
{
    class Program
    {
        static string CompressedText;
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Escribe el método con el que deseas cifrar el texto");
                var method = Console.ReadLine();
                Console.WriteLine("Escribe el string a cifrar");
                string text = Console.ReadLine();
                Console.WriteLine("Escribe la llave del cifrado");
                string givenKey = Console.ReadLine();
                KeyHolder key = new KeyHolder();
                if (key.SetKeyFromString(method.ToLower(), givenKey))
                {
                    Console.WriteLine("Se ha guardado el string con éxito para cifrar");
                    Console.WriteLine("El resultado de la compresión es el siguiente:");
                    if (WriteCipher(key, method.ToLower(), text))
                    {
                        Console.WriteLine("¿Desea descifrarlo? | Presione 'Y'. De lo contrario, presione cualquier otra tecla.");
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            Console.Clear();
                            Console.WriteLine("El resultado de la descifrado es el siguiente:");
                            if (!WriteDecipher(key, method.ToLower()))
                            {
                                HandleError();
                            }
                            Console.ReadLine();
                        }
                        Console.WriteLine("Feliz día!");
                        Console.ReadLine();
                    }
                    else
                    {
                        HandleError();
                    }
                }
                else
                {
                    HandleError();
                }

            }
            catch
            {
                HandleError();
            }
        }

        static void HandleError()
        {
            Console.WriteLine("Inserte un string válido");
            Console.WriteLine("Los tipos de cifrado que puede escoger son cesar, zigzag y SDES");
            Console.WriteLine("La llave del cifrado Cesar debe contener solamente letras del abecedario inglés");
            Console.WriteLine("La llave del cifrado Zigzag debe ser un número mayor a 0");
            Console.WriteLine("La llave del cifrado SDES debe ser un número mayor a 0 pero menor a 1023");
            Console.ReadLine();
            Console.Clear();
        }

        static bool WriteCipher(KeyHolder key, string method, string text)
        {
            try
            {
                switch (method)
                {
                    case "cesar":
                        var cesarEncryptor = new CrypCesar<KeyHolder>();
                        CompressedText = cesarEncryptor.EncryptString(text, key);
                        Console.WriteLine(CompressedText);
                        return true;
                    case "zigzag":
                        var zigzagEncryptor = new CrypZigZag<KeyHolder>();
                        CompressedText = zigzagEncryptor.EncryptString(text, key);
                        Console.WriteLine(CompressedText);
                        return true;
                    case "sdes":
                        var SDESEncryptor = new SDES<KeyHolder>();
                        CompressedText = SDESEncryptor.EncryptString(text, key);
                        Console.WriteLine(CompressedText);
                        return true;
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        static bool WriteDecipher(KeyHolder key, string method)
        {
            try
            {
                switch (method)
                {
                    case "cesar":
                        var cesarEncryptor = new CrypCesar<KeyHolder>();
                        Console.WriteLine(cesarEncryptor.DecryptString(CompressedText, key));
                        return true;
                    case "zigzag":
                        var zigzagEncryptor = new CrypZigZag<KeyHolder>();
                        Console.WriteLine(zigzagEncryptor.DecryptString(CompressedText, key));
                        return true;
                    case "sdes":
                        var SDESEncryptor = new SDES<KeyHolder>();
                        Console.WriteLine(SDESEncryptor.DecryptString(CompressedText, key));
                        return true;
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
