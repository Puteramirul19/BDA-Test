using System.Security.Claims;
using System.Threading.Tasks;
using BDA.ActiveDirectory;
using BDA.Entities;
using Microsoft.AspNetCore.Identity;

namespace BDA.Identity
{
    public class ActiveDirectoryAuthenticationStrategy : IAuthenticationStrategy
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IDirectoryService directoryService;

        public ActiveDirectoryAuthenticationStrategy(SignInManager<ApplicationUser> signInManager, IDirectoryService directoryService)
        {
            this.signInManager = signInManager;
            this.directoryService = directoryService;
        }

        public AuthenticationMethod AuthenticationMethod => AuthenticationMethod.ActiveDirectory;

        public async Task<SignInResult> SignInAsync(ApplicationUser user, string password, bool isPersistent)
        {
            if (!user.IsActive)
                return SignInResult.NotAllowed;

            if (await ValidateCredentials(user.UserName, password))
            {
                await signInManager.SignInAsync(user, isPersistent);
                return SignInResult.Success;
            }
            else
                return SignInResult.Failed;
        }

        public async Task<bool> ValidateCredentials(string username, string password)
        {
            var user = await directoryService.Authenticate(username, password);
            return user != null;
        }
    }
}
