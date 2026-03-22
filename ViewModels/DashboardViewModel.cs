using Models;
using Services.Interfaces;
using ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {

        public ICommand LogoutCommand { get; }

        public DashboardViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            LogoutCommand = new RelayCommand(param => Logout());
        }

        private void Logout()
        {
            _navigationService!.NavigateToAsync("//SignInPage");
        }

    }
}
