using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICredentialService
    {
        Task SaveCredentialsAsync(string username, string password);
        Task<(string? username, string? password)> GetCredentialsAsync();
        Task ClearCredentialsAsync();
    }
}
