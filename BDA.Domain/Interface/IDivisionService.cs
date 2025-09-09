using BDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BDA.Interface
{
    public interface IDivisionService
    {
        IEnumerable<DivisionViewModel> GetDivisions();
    }
}
