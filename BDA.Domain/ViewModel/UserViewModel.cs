using System;
using System.Collections.Generic;
using System.Text;
using BDA.Entities;
using BDA.Identity;

namespace BDA.ViewModel
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string AuthenticationMethod { get; set; }
        public string Password { get; set; }
        public string Repassword { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string OfficeNo { get; set; }
        public string Email { get; set; }
        public string DivisionName{ get; set; }
        public string UnitName { get; set; }
        public string Designation { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime LastLogin { get; set; }
        //public ApplicationRole Roles { get; set; }
        public string RoleId { get; set; }
        public ApplicationRole Role { get; set; }
        public string UserRoleId { get; set; }
        public IEnumerable<ApplicationUserRole> UserRoles { get; set; }
        public Nullable<Guid> DivisionId { get; set; }
        public Division Division { get; set; }
        public Nullable<Guid> FunctionId { get; set; }
        public Function Function { get; set; }
        public Nullable<Guid> ZoneId { get; set; }
        public Zone Zone { get; set; }
        public Nullable<Guid> UnitId { get; set; }
        public Unit Unit { get; set; }
        public bool IsRemoved { get; set; }
    }
}
