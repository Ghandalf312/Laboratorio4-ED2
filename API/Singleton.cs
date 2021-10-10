using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;

namespace API
{
    public class Singleton
    {
        private static Singleton _instance = null;

        public static Singleton Instance
        {
            get
            {
                if (_instance == null) _instance = new Singleton();
                return _instance;
            }
        }
        public List<Ciphers> HistoryList = new List<Ciphers>();
    }
}
