using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
   
        public class DivisionViewModel
        {
            public string Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public bool IsActive { get; set; }
            public int LOAType { get; set; }
        public IEnumerable<Division> Divisions { get; set; }

    }

}
