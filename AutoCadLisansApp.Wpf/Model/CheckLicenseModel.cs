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
    public class CheckLicenseModel : LicenseController.autocad.masterkey.ws.CheckLicenseDTO
    {
        private SoftwareDTO _app = new SoftwareDTO();
        public LicenseController.autocad.masterkey.ws.Service1Client client = new LicenseController.autocad.masterkey.ws.Service1Client();
        public SoftwareDTO App { get { return _app; } set { _app = value; } }


        public QueryState QueryState { get; set; }
        public string Description { get; set; }

        public void InitiliazeObject()
        {

            Fail = false;
            IsFound = false;
            Success = false;
            Installed = false;
            Uninstalled = false;
            InstallDate = null;
            UnInstallDate = null;
            CheckDate = DateTime.Now;
        }

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
    public enum QueryState
    {
        User_not_authorized_to_login_computer = 0,
        Remote_computer_doesnt_respond_Maybe_it_is_switched_off_or_WMI_service_is_not_running_on_it = 1,
        Product_found = 2,
        No_Product_found = 3,
        UnKnownState = 4,
        User_credentials_cannot_be_used_for_local_connections = 5
    }
}
