using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BDA.Data;
using Microsoft.EntityFrameworkCore;

namespace BDA.Identity
{
    public class UserRoleHelper
    {
        private readonly BdaDBContext db;

        public UserRoleHelper(BdaDBContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersWithPermission(Permission permission)
        {
            var claim = permission.ToClaim();

            return await db.Users.FromSqlRaw(@"
                    select u.*
                    from AspNetUserRoles ur
                    inner
                    join AspNetRoleClaims rc on rc.RoleId = ur.RoleId
                    inner
                    join AspNetUsers u on u.Id = ur.UserId
                    where rc.ClaimType = {0} AND rc.ClaimValue = {1}
                ", claim.Type, claim.Value)
                 .ToListAsync();



            //return await db.UserClaims
            //            .Include(x => x.User)
            //            .Where(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value)
            //            .Select(x => x.User)
            //            .ToListAsync();
        }
    }
}
