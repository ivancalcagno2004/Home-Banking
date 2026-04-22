using Services.Interfaces;

namespace UI.Services
{
    public class ClipboardService : IClipboardService
    {
        public async Task SetTextAsync(string text)
        {
            await Clipboard.Default.SetTextAsync(text);
        }
    }
}
