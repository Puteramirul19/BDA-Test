using System;
using System.Collections.Generic;
using System.Text;

namespace BDA.IdGeneration
{
    public interface INumberSpecification
    {
        /// <summary>
        /// Get a key that will indicate a group of running numbers.
        /// This key will be used as unique identifier in DB (or other storage mechanism).
        /// </summary>
        /// <returns></returns>
        string GetKey();

        /// <summary>
        /// Format the number and returns a string with necessary prefix/postfix and other formattings. 
        /// </summary>
        /// <param name="number">The running number retrieved from DB (or other storage mechanism)</param>
        /// <returns></returns>
        string Format(int number);
    }
}
