using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SNMPDeviceDataAttribute : Attribute
    {
        public SNMPDeviceDataAttribute()
        {
            
        }
    }

}
