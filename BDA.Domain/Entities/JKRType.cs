using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BDA.Entities
{
    public class SendMethod
    {
        public Guid Id { get; set; }
        public string Type { get; set; }

    }
}
