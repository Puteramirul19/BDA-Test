using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDA.Entities;
using Microsoft.AspNetCore.Identity;

namespace BDA.Identity
{
    public interface IAuthenticator
    {
        Task<SignInResult> SignInAsync(ApplicationUser user, string password, bool isPersistent = false);
    }

    public class Authenticator : IAuthenticator
    {
        private IEnumerable<IAuthenticationStrategy> authenticationStrategies;

        public Authenticator(IEnumerable<IAuthenticationStrategy> authenticationStrategies)
        {
            this.authenticationStrategies = authenticationStrategies;
        }

        public async Task<SignInResult> SignInAsync(ApplicationUser user, string password, bool isPersistent = false)
        {
            var strategy = authenticationStrategies.FirstOrDefault(x => x.AuthenticationMethod == user.AuthenticationMethod);
            if (strategy == null)
                throw new Exception($"Cannot find Authentication Strategy for Authentication Method = '{user.AuthenticationMethod}'");

            return await strategy.SignInAsync(user, password, isPersistent);
        }
    }
}
