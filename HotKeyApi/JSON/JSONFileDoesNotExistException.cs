using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotKeyApi
{
    class JSONFileDoesNotExistException : Exception
    {

        public JSONFileDoesNotExistException()
        {

        }
        public JSONFileDoesNotExistException(string message)
            :base(message)
        {

        }
    }
}
