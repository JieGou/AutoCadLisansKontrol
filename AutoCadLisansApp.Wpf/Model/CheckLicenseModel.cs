using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaterialDesignDemo.Model
{
    public class CheckLicenseModel : autocad.masterkey.ws.CheckLicense
    {
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
                    this._fail = value;
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

        private string _error="";
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
}
