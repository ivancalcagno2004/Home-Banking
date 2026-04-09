using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IGroqChatService
    {
        Task<string?> SendMessageAsync(string message);
    }
}
