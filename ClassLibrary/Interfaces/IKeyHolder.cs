using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Interfaces
{
    public interface IKeyHolder 
    {
        string GetCesarKey();
        int GetZigZagKey();
    }
}
