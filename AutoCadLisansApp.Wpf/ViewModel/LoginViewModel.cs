using MaterialDesignDemo.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LicenseController.ViewModel
{
    public class LoginViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private bool _notificationIsVisible = false;
        public bool NotificationIsVisible { get { return _notificationIsVisible; } set { _notificationIsVisible = value; OnPropertyChanged("NotificationIsVisible"); } }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
