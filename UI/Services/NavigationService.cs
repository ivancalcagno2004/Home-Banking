using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Services
{
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(string route)
        {
            await Shell.Current.GoToAsync(route);
        }
    }
}
