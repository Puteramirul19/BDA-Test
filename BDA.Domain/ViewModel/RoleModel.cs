using System;
using System.Collections.Generic;
using System.Text;

namespace BDA.ViewModel
{
   
    public class RoleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool NeedDivision { get; set; }
        public bool NeedFunction { get; set; }
        public bool NeedZone { get; set; }
        public bool NeedUnit { get; set; }
    }

}
