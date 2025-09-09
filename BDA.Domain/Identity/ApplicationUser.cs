using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BDA.Entities;
using Microsoft.AspNetCore.Identity;

namespace BDA.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {
           
            IsActive = true;
        }
        //
        // Identity related navigation properties
        //
        public virtual ICollection<ApplicationUserRole> Roles { get; set; }
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }

        //
        // Custom properties
        //
        public AuthenticationMethod AuthenticationMethod { get; set; }
        public string FullName { get; set; }
        public string Division { get; set; }
        public string Unit { get; set; }
        public string OfficeNo { get; set; }
        public string Designation { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn{ get; set; }
        public DateTime UpdatedOn{ get; set; }
        public DateTime LastLogin { get; set; }
        public int SessionCount { get; set; }
        public bool ResetPassword { get; set; }
        public DateTime PasswordExpiredOn { get; set; }
        public bool IsRemoved { get; set; }

        public IDictionary<string, string> GetMessageValues()
        {
            return new Dictionary<string, string>
            {
                { "UserName", (UserName.ToString() == null ? "" : UserName.ToString())},

            };
        }
    }

    public class ApplicationRole : IdentityRole
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }

        public bool NeedDivision { get; set; }
        public bool NeedFunction { get; set; }
        public bool NeedZone { get; set; }
        public bool NeedUnit { get; set; }
        public int LOAType { get; set; }
        public bool isActive { get; set; }
        public void SetIdUsingName()
        {
            Id = System.Text.RegularExpressions.Regex.Replace(Name, "[^A-Za-z0-9]", "");
        }
    }

    public class ApplicationUserRole : IdentityUserRole<string>
    {
        [Key]
        public Guid Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }

        public Guid? DivisionId { get; set; }
        public virtual Division Division { get; set; }
        public Guid? FunctionId { get; set; }
        public virtual Function Function { get; set; }
        public Guid? ZoneId { get; set; }
        public virtual Zone Zone { get; set; }
        public Guid? UnitId { get; set; }
        public virtual Unit Unit { get; set; }

    }

    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public virtual ApplicationRole Role { get; set; }
    }

    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}