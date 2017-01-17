
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var securePassword = ConvertToSecureString("ciler471");

            var cred = new PSCredential("CILERTURKMEN", securePassword);

            var connectionInfo = new WSManConnectionInfo(new Uri("http://superserver.corp/powershell"),
        
                                 "http://schemas.microsoft.com/powershell/Microsoft.Exchange",
                                 cred);

            connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Credssp; //or Basic, Digest, Kerberos etc.
          

            using (var runspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                using (var ps = System.Management.Automation.PowerShell.Create())
                {
                    string psCommand = "";
                    //psCommand was created above
                    

                    runspace.Open();
                    ps.Runspace = runspace;

                    try
                    {
                        var results = ps.Invoke();

                        if (!ps.HadErrors)
                        {
                            runspace.Close();
                            //Yay! no errors. Do something else?
                        }
                    }
                    catch (RemoteException ex)
                    {
                        //error handling here
                    }
                }
            }

        }
        private static SecureString ConvertToSecureString(string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}
