﻿using System;
using System.Collections.Generic;
using System.Text;

using ClassLibrary.Interfaces;
using Microsoft.VisualBasic.CompilerServices;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;




namespace ClassLibrary.Encryptors
{
    class CrypCesar<T>  where T : IKeyHolder
    {
        #region Variables
        Dictionary<byte, byte> CesarDictionary = new Dictionary<byte, byte>();
        #endregion

        #region DictionaryLoad
        private void LoadDictionary(T key, bool encryption)
        {
            var keyValue = key.GetCesarKey();
            var upperCharacters = Enumerable.Range('A', 'Z' - 'A' + 1).Select(x => (byte)x).ToList();
            var lowerCharacters = Enumerable.Range('a', 'z' - 'a' + 1).Select(x => (byte)x).ToList();
            if (encryption)
            {
                FillDictionary(keyValue.ToUpper(), true, upperCharacters);
                FillDictionary(keyValue.ToLower(), true, lowerCharacters);
            }
            else
            {
                FillDictionary(keyValue.ToUpper(), false, upperCharacters);
                FillDictionary(keyValue.ToLower(), false, lowerCharacters);
            }
        }

        private void FillDictionary(string key, bool encryption, List<byte> letterList)
        {
            var characterList = letterList;
            var keyList = new List<byte>();
            foreach (var character in key)
            {
                if (!keyList.Contains((byte)character))
                {
                    keyList.Add((byte)character);
                }
            }
            var secondaryList = new List<byte>();
            if (encryption)
            {
                for (int i = 0; i < keyList.Count; i++)
                {
                    CesarDictionary.Add(characterList[i], keyList[i]);
                }
                foreach (var character in characterList)
                {
                    if (!CesarDictionary.ContainsValue(character))
                    {
                        secondaryList.Add(character);
                    }
                }
            }
            else
            {
                for (int i = 0; i < keyList.Count; i++)
                {
                    CesarDictionary.Add(keyList[i], characterList[i]);
                }
                foreach (var character in characterList)
                {
                    if (!CesarDictionary.ContainsKey(character))
                    {
                        secondaryList.Add(character);
                    }
                }
            }
            characterList.RemoveRange(0, keyList.Count);
            if (encryption)
            {
                for (int i = 0; i < secondaryList.Count; i++)
                {
                    CesarDictionary.Add(characterList[i], secondaryList[i]);
                }
            }
            else
            {
                for (int i = 0; i < secondaryList.Count; i++)
                {
                    CesarDictionary.Add(secondaryList[i], characterList[i]);
                }
            }
        }
        #endregion

        #region Encryption
        public string EncryptString(string text, T key)
        {
            CesarDictionary.Clear();
            var encryptedString = string.Empty;
            LoadDictionary(key, true);
            foreach (var character in text)
            {
                if (CesarDictionary.ContainsKey((byte)character))
                {
                    encryptedString += (char)CesarDictionary[(byte)character];
                }
                else
                {
                    encryptedString += (char)character;
                }
            }
            return encryptedString;
        }
        #endregion


        #region Decryption

        public string DecryptString(string text, T key)
        {
            CesarDictionary.Clear();
            var decryptedString = string.Empty;
            LoadDictionary(key, false);
            foreach (var character in text)
            {
                if (CesarDictionary.ContainsKey((byte)character))
                {
                    decryptedString += (char)CesarDictionary[(byte)character];
                }
                else
                {
                    decryptedString += (char)character;
                }
            }
            return decryptedString;
        }
        #endregion
    }
}