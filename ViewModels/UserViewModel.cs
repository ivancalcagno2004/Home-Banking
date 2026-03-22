using Models;
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

        public ObservableCollection<User> Users { get; }

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

            Users = new ObservableCollection<User>();

            LoadUsersAsync();
            Users.Add(new User { 
                UserName="Ivan",
                Password="123",
                Email="Icalc@gmail.com",
                FullName="Ivan Calcano",
                CreatedAt=DateTime.UtcNow
            });
            
            Users.Add(new User { 
                UserName="Clau",
                Password="123",
                Email="Icalc@gmail.com",
                FullName="Claudia Suarez",
                CreatedAt=DateTime.UtcNow
            });
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
