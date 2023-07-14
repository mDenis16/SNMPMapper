using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPLib
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SNMPValueAttribute : Attribute
    {
        public string Path
        {
            get; set;
        }

        public SNMPValueAttribute(string path)
        {
            Path = path;
        }
    }

}
