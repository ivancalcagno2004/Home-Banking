using Models;
using Models.DTO;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class UserViewModel : BaseViewModel
    {

        public ObservableCollection<UserProfileDTO> Users { get; }

        private User? _selectedUser;

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        public UserViewModel(IUserService userService)
        {
            _userService = userService;

            Users = new ObservableCollection<UserProfileDTO>();

            LoadUsersAsync();
        }

        private async void LoadUsersAsync()
        {
            var users = await _userService!.GetAllAsync();
            
            foreach (var user in users)
            {
                if (!Users.Contains(user))
                {
                    Users.Add(user);
                }
            }
        }
    }
}
