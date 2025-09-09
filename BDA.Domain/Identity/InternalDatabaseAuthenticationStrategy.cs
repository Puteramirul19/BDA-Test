using System.Threading.Tasks;
using BDA.Entities;
using Microsoft.AspNetCore.Identity;

namespace BDA.Identity
{
    public class InternalDatabaseAuthenticationStrategy : IAuthenticationStrategy
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        public InternalDatabaseAuthenticationStrategy(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        public AuthenticationMethod AuthenticationMethod => AuthenticationMethod.InternalDatabase;

        public async Task<SignInResult> SignInAsync(ApplicationUser user, string password, bool isPersistent)
        {
            if (!user.IsActive)
                return SignInResult.NotAllowed;

            return await signInManager.PasswordSignInAsync(user, password, isPersistent, false);
        }
    }
}
