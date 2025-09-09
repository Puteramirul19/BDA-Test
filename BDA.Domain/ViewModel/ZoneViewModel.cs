using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
    public class ZoneViewModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Guid DivisionId { get; set; }
        public Guid FunctionId { get; set; }
        public Function Function { get; set; }
        public IEnumerable<Zone> Zones { get; set; }

    }
}
