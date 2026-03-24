using Microsoft.Extensions.DependencyInjection;

namespace UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            window.Width = 1280;  
            window.Height = 720;  

            window.MinimumWidth = 900;
            window.MinimumHeight = 650;

            
            window.MaximumWidth = 1920;
            window.MaximumHeight = 1080;

            return window;
        }
    }
}