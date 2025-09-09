//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.ComponentModel.DataAnnotations;
//using Newtonsoft.Json;
//using Microsoft.AspNetCore.Identity;
//using System.Security.Claims;
//using BDA.Identity;

//namespace BDA.Entities
//{
//    public class Users2
//    {
//        public string Id { get; set; }
//        public LoginType LoginType { get; set; }
//        public DateTime LastLogin { get; set; }
//        public string FullName { get; set; }
//        public string NRIC { get; set; }
//        public string Photo { get; set; }
//        public string Division { get; set; }
//        public string Unit { get; set; }
//        public string Language { get; set; }
//        public string OfficeNumber { get; set; }
//        public string PhoneNumber { get; set; }
//        public string Email { get; set; }
//        public string PasswordHash { get; set; }
//        public string SecurityStamp { get; set; }
//        public DateTime LastPasswordChangeDate { get; set; }
//        public bool AccessFailedCount { get; set; }
//        public DateTime LockoutEndDate { get; set; }
//        public string TwoFactorEnabled { get; set; }
//        public string UserName { get; set; }
//        public bool IsActive { get; set; }
//        public AuthenticationMethod AuthenticationMethod { get; set; }

//    }

//    public enum LoginType
//    {
//        Internal,
//        ActiveDirectory
//    }

//}
