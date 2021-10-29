using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Helpers
{
    public class Key
    {
        public int X { get; set; }
        public int N { get; set; }

        public Key() { }

        public Key(string text)
        {
            string[] split = text.Split(',');
            N = int.Parse(split[0]);
            X = int.Parse(split[1]);
        }
    }
}
