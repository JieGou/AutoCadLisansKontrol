using LicenseController.autocad.masterkey.ws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaterialDesignDemo.Model
{
    public class ComputerModel:Computer
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
    }
}
