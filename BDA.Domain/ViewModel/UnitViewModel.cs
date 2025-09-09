using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
    public class UnitViewModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Guid DivisionId { get; set; }
        public Guid FunctionId { get; set; }
        public Guid ZoneId { get; set; }
        public Zone Zone { get; set; }
        public IEnumerable<Unit> Units { get; set; }


    }
}
