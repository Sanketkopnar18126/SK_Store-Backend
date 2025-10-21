using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Domain.Exceptions
{
    public class InvalidProductException:Exception
    {
        public InvalidProductException() { }

        public InvalidProductException(string message)
            : base(message) { }

        public InvalidProductException(string message, Exception inner)
            : base(message, inner) { }
    }
}
