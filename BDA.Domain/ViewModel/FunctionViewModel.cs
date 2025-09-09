using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
    public class FunctionViewModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Guid DivisionId { get; set; }
        public Division Division { get; set; }
        public IEnumerable<Function> Functions { get; set; }

    }
}
