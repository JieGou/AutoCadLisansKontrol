using MaterialDesignDemo.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaterialDesignDemo.Model
{
    public class CheckLicenseModel : LicenseController.autocad.masterkey.ws.CheckLicense
    {
        public FileExplorerModel FileExplorerModel { get; set; }
        public List<ApplicationEvent> ApplicationEvents { get; set; }

        public List<Win32_Product> Win32_products { get; set; }

        public List<Software> UnInstallRegisterySoftwares { get; set; }
        public List<Software> RegistryAutoDesk { get; set; }


        private bool _isProgress;
        public bool IsProgress
        {
            get
            {
                return this._isProgress;
            }
            set
            {
                if ((this._isProgress.Equals(value) != true))
                {
                    this._isProgress = value;
                    this.RaisePropertyChanged("IsProgress");
                }
            }
        }
        private bool _isfound = false;
        public bool IsFound
        {
            get
            {
                return this._isfound;
            }
            set
            {
                if ((this._isfound.Equals(value) != true))
                {
                    this._isfound = value;
                    this.RaisePropertyChanged("IsFound");
                }
            }
        }

        private bool _fail = false;
        public bool Fail
        {
            get
            {
                return this._fail;
            }
            set
            {
                if ((this._fail.Equals(value) != true))
                {
                    _fail = value;
                    this.RaisePropertyChanged("Fail");
                }
            }
        }

        private bool _success = false;
        public bool Success
        {
            get
            {
                return this._success;
            }
            set
            {
                if ((this._success.Equals(value) != true))
                {
                    this._success = value;
                    this.RaisePropertyChanged("Success");
                }
            }
        }

        private string _error = "";
        public string Error
        {
            get
            {
                return this._error;
            }
            set
            {
                if ((this._error.Equals(value) != true))
                {
                    this._error = value;
                    this.RaisePropertyChanged("Error");
                }
            }
        }


    }
    public class CheckList : INotifyPropertyChanged
    {
        private bool _willchecked;
        public string Name { get; set; }
        public string AvgTime { get; set; }
        public bool WillChecked
        {
            get
            {
                return this._willchecked;
            }
            set
            {
                this._willchecked = value;
                this.OnPropertyChanged("WillChecked");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
