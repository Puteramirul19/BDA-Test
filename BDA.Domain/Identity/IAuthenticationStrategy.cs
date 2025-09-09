using System.Threading.Tasks;
using BDA.Entities;
using Microsoft.AspNetCore.Identity;

namespace BDA.Identity
{
    public interface IAuthenticationStrategy
    {
        AuthenticationMethod AuthenticationMethod { get; }
        Task<SignInResult> SignInAsync(ApplicationUser user, string password, bool isPersistent);
    }
}
