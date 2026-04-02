using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Services
{
    public class CredentialService : ICredentialService
    {
        public async Task SaveCredentialsAsync(string username, string password)
        {
            await SecureStorage.Default.SetAsync("saved_username", username);
            await SecureStorage.Default.SetAsync("saved_password", password);
        }

        public async Task<(string? username, string? password)> GetCredentialsAsync()
        {
            var user = await SecureStorage.Default.GetAsync("saved_username");
            var pass = await SecureStorage.Default.GetAsync("saved_password");

            return (user, pass);
        }
    }
}
