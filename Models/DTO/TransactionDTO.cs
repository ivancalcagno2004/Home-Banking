using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class TransactionDTO : INotifyPropertyChanged
    {
        private string? _date;
        private string? _description;
        private string? _amount;
        private string? _color;

        public string? Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        public string? Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string? Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        public string? Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
