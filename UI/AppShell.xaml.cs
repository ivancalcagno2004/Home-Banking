using UI.Views.Pages;
using UI.Resources.Helpers;

namespace UI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            ConstruirMenuResponsive();
        }

        private void ConstruirMenuResponsive()
        {
            bool isMobile = DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Idiom == DeviceIdiom.Tablet;

            if (isMobile)
            {
                this.FlyoutBehavior = FlyoutBehavior.Disabled;

                Routing.RegisterRoute("SettingsPageMobile", typeof(SettingsPage));

                var tabBar = new TabBar();

                tabBar.Items.Add(CrearTab("Inicio", IconFont.Home, typeof(HomePage), "HomePage"));
                tabBar.Items.Add(CrearTab("Transferencias", IconFont.Transfer, typeof(TransferPage), "TransferPage"));
                tabBar.Items.Add(CrearTab("Pagos", IconFont.Pagos, typeof(PaymentsPage), "PaymentsPage"));
                tabBar.Items.Add(CrearTab("Movimientos", IconFont.Transactions, typeof(TransactionsPage), "TransactionsPage"));

                this.Items.Add(tabBar);
            }
            else
            {
                this.FlyoutBehavior = FlyoutBehavior.Locked; 

                this.Items.Add(CrearFlyoutItem("Inicio", IconFont.Home, typeof(HomePage), "HomePage", "Mi Resumen"));
                this.Items.Add(CrearFlyoutItem("Transferencias", IconFont.Transfer, typeof(TransferPage), "TransferPage", "Transferir Dinero"));
                this.Items.Add(CrearFlyoutItem("Pagos de Servicios", IconFont.Pagos, typeof(PaymentsPage), "PaymentsPage", "Pagar Impuestos"));
                this.Items.Add(CrearFlyoutItem("Últimos Movimientos", IconFont.Transactions, typeof(TransactionsPage), "TransactionsPage", "Historial"));
                this.Items.Add(CrearFlyoutItem("Configuración", IconFont.Settings, typeof(SettingsPage), "SettingsPage", "Ajustes"));
            }
        }

        private Tab CrearTab(string titulo, string icono, Type pagina, string ruta)
        {
            var fontImage = new FontImageSource { FontFamily = "MaterialDesign", Glyph = icono, Size = 22, Color = Colors.Blue };

            var shellContent = new ShellContent
            {
                Route = ruta,
                ContentTemplate = new DataTemplate(pagina)
            };
            Shell.SetNavBarIsVisible(shellContent, true);

            return new Tab
            {
                Title = titulo,
                Icon = fontImage,
                Items = { shellContent }
            };
        }

        private FlyoutItem CrearFlyoutItem(string tituloFlyout, string icono, Type pagina, string ruta, string tituloPagina)
        {
            var fontImage = new FontImageSource { FontFamily = "MaterialDesign", Glyph = icono, Size = 22, Color = Colors.Blue };
            fontImage.SetAppThemeColor(FontImageSource.ColorProperty, Colors.Black, Colors.White);

            var shellContent = new ShellContent
            {
                Title = tituloPagina,
                Route = ruta,
                ContentTemplate = new DataTemplate(pagina)
            };
            Shell.SetNavBarIsVisible(shellContent, false);

            return new FlyoutItem
            {
                Title = tituloFlyout,
                Icon = fontImage,
                Items = { shellContent }
            };
        }

        private async void OnLogOutClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("🚪 [NAVEGACIÓN] El usuario hizo clic en Cerrar Sesión.");
            await Current.GoToAsync("//SignInPage");
        }
    }
}