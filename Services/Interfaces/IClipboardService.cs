using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IClipboardService
    {
        Task SetTextAsync(string text);
    }
}
