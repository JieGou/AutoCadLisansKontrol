using LicenseController.autocad.masterkey.ws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseController.Model
{
    public class GlobalVariable
    {
        public static GlobalVariable globalVariable = null;

        public static GlobalVariable getInstance()
        {

            if (globalVariable == null)
            {
                return globalVariable = new GlobalVariable();
            }
            return globalVariable;
        }

        public UsersDTO user;
    }
}
