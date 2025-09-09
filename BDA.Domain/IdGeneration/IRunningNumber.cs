using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDA.IdGeneration
{
    public interface IRunningNumber
    {
        string Next(INumberSpecification numberSpecification);
        void InitializeStorage(bool deleteExisting = false);
    }
}