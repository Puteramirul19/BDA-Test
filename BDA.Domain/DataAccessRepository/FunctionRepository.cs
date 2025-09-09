using BDA.Repository;
using BDA.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using BDA.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BDA.DataAccessRepository
{
    public class FunctionRepository : GenericRepository<Function>, IFunctionRepository
    {
        public FunctionRepository(IApplicationContext context)
            : base(context as DbContext)
        {
        }

        public IEnumerable<Function> GetFunctions()
        {
            return GetAll();
        }
    }
}
