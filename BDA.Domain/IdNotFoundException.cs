using System;
using System.Collections.Generic;
using System.Text;

namespace BDA
{
    public class IdNotFoundException<T> : Exception
    {
        public IdNotFoundException(object id) : base("No '" + typeof(T) + "' found with ID='" + id + "'.")
        {
        }
    }
}
