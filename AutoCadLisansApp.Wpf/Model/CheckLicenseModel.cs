using LicenseController.autocad.masterkey.ws;
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
        private Software _app = new Software();
        public LicenseController.autocad.masterkey.ws.Service1Client client = new LicenseController.autocad.masterkey.ws.Service1Client();
        public Software App { get { return _app; } set { _app = value; } }
        public string Description { get; set; }

        private System.Nullable<bool> _isProgress;
        public System.Nullable<bool> IsProgress
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
      

        private System.Nullable<bool> _fail = false;
        public System.Nullable<bool> Fail
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

        private System.Nullable<bool> _success = false;
        public System.Nullable<bool> Success
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
    public class OutputSniff
    {
        public FileExplorerModel FileExplorerModel { get; set; }
        public List<ApplicationEvent> ApplicationEvents { get; set; }

        public List<Win32_Product> Win32_products { get; set; }

        public List<RegistrySoftware> RegistryAutoDesk { get; set; }
    }
}
