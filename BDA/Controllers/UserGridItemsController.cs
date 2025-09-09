//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using BDA.Data;
//using BDA.Entities;
//using Microsoft.AspNetCore.Authorization;
//using BDA.Identity;

//namespace BDA.Web.Controllers
//{
//    public class UserGridItem
//    {
//        public string Id { get; set; }
//        public string UserName { get; set; }
//        public string FullName { get; set; }
//        public string Email { get; set; }
//        public string PhoneNumber { get; set; }
//        public bool IsActive { get; set; }
//        public ICollection<UserRole> UserRoles { get; set; }
//    }

//    public class UserRole
//    {
//        public string Id { get; set; }
//        public string Name { get; set; }
//    }

//    [Authorize]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserGridItemsController : BaseApiController
//    {
//        [HttpGet]
//        public async Task<DataSourceResult> Get([DataSourceRequest]DataSourceRequest request)
//        {
//            IQueryable<ApplicationUser> query = Db.Users.OrderBy(x => x.UserName);

//            var appUser = await GetApplicationUser();

//            return await query.Select(x => new UserGridItem
//            {
//                Id = x.Id,
//                UserName = x.UserName,
//                FullName = x.FullName,
//                Email = x.Email,
//                PhoneNumber = x.PhoneNumber,
//                IsActive = x.IsActive,
//                //UserRoles = x.UserRoles
//                //                .OrderBy(y => y.Role.Name)
//                //                .Select(y => new UserRole { Id = y.RoleId, Name = y.Role.Name })
//                //                .ToList(),
//            })
//                .ToDataSourceResultAsync(request);
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(string id)
//        {
//            var user = await Db.Users.FindAsync(id);
//            if (user == null)
//            {
//                return NotFound();
//            }

//            var userRoles = await Db.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();
//            foreach (var role in userRoles)
//            {
//                Db.SetDeleted(role);
//            }

//            Db.Users.Remove(user);
//            await Db.SaveChangesAsync();

//            return Ok();
//        }
//    }
//}
