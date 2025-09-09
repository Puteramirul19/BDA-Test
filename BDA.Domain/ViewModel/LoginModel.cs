using System;
using System.Collections.Generic;
using System.Text;

namespace BDA.ViewModel
{
   
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string Repassword { get; set; }
        //public bool RememberMe { get; set; }
    }

}
