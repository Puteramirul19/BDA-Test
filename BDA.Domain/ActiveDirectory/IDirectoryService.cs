using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BDA.ActiveDirectory
{
    public interface IDirectoryService
    {
        Task<DirectoryUser> GetUserByStaffNo(string staffNo);
        Task<DirectoryUser> Authenticate(string staffNo, string password);
    }
}
