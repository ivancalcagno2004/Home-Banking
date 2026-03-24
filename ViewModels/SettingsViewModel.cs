using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly UserSession _userSession;

        public string FullName => _userSession.CurrentUser?.FullName ?? "Usuario";
        public string Email => _userSession.CurrentUser?.Email ?? "usuario@email.com";
        public string Initials => !string.IsNullOrEmpty(FullName) ? FullName[0].ToString() : "U";
        public string AppVersion
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return $"v{version?.Major}.{version?.Minor}.{version?.Build} - Tandil Bank";
            }
        }

        private bool _isDarkMode;

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _notificationsEnabled = true;

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                _notificationsEnabled = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel(UserSession userSession)
        {
            _userSession = userSession;
        }
    }
}