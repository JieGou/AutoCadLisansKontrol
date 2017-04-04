using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialDesignDemo.ViewModel
{
    public class BaseViewModel
    {
        public LicenseController.autocad.masterkey.ws.Service1Client client = new LicenseController.autocad.masterkey.ws.Service1Client();
    }
}
