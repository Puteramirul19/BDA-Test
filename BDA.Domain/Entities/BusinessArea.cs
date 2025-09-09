using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.Entities
{
    public class BusinessArea
    {
        public Guid Id { get; set; }
        //public Guid StateId { get; set; }
        //public State State { get; set; }
        public Guid DivisionId { get; set; }
        public Division Division { get; set; }
        //public String Zone { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
    }
}
