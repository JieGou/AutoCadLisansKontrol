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
        public bool IsProgress { get; set; }
        public Visibility IsProgressVisibility { get { return IsProgress == true ? Visibility.Visible : Visibility.Hidden; } }
    }
}
