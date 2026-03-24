namespace UI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("SignUpPage", typeof(Views.SignUpPage));
        }

        private async void OnLogOutClicked(object sender, EventArgs e)
        {
            await Current.GoToAsync("//SignInPage");
        }
    }
}
