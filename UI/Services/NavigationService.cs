using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Services
{
    /// <summary>
    /// Servicio de navegación de la capa UI.
    /// Centraliza la navegación por rutas de .NET MAUI Shell a través de <see cref="INavigationService"/>.
    /// </summary>
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(string route)
        {
            await Shell.Current.GoToAsync(route);
        }
    }
}
