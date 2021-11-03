using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Helpers
{
    class KeyRSA
    {
        public int X { get; set; }
        public int N { get; set; }

        public KeyRSA()
        {

        }

        public KeyRSA(string text)
        {
            string[] split = text.Split(',');
            N = int.Parse(split[0]);
            X = int.Parse(split[1]);
        }
    }
}
