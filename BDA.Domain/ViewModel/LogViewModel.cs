using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDA.ViewModel
{
    public class LogViewModel
    {
        public string TableName { get; set; }
        public List<string> ColumnList { get; set; }
        public List<List<string>> ColumnArray { get; set; }
        public string FilterWhere { get; set; }
    }
}
