using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mupdflibnet
{
    /// <summary>
    /// Represents an exception that occurred in unmanaged code
    /// </summary>
    public class NativeException : Exception
    {
        public NativeException(string message) : base(message) { }
    }
}
