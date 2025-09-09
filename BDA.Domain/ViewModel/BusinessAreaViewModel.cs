using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;

namespace BDA.ViewModel
{
    public class BusinessAreaViewModel
    {
      
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public Guid StateId { get; set; }
        //public State State { get; set; }
        public Guid DivisionId { get; set; }
        public Division Division { get; set; }

        //public string Zone { get; set; }
        public IEnumerable<BusinessArea> BusinessAreas { get; set; }

    }
}
