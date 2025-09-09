using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;
using Microsoft.EntityFrameworkCore;

namespace BDA.Interface
{
    public interface IApplicationContext
    {
        DbSet<Division> Division { get; set; }
        DbSet<Function> Function { get; set; }
    }
}
