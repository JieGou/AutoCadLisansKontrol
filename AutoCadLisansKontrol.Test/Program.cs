
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var username = "adminciler";
            var password = ConvertToSecureString("ciler471");
            string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
            PSCredential remoteCredential = new PSCredential(username, password);
            WSManConnectionInfo connectionInfo = new WSManConnectionInfo(false, "CILERTURKMEN", 5985, "/wsman", shellUri, remoteCredential);
            var FileName = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\checklicense.bat";
            var command=System.IO.File.ReadAllText(FileName);

            using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                runspace.Open();
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(command);
                var results = pipeline.Invoke();

            }

            //Environment.SetEnvironmentVariable("clientname", "CILERTURKMEN");
            //Environment.SetEnvironmentVariable("username", "adminciler");
            //Environment.SetEnvironmentVariable("password", "ciler471");
            //ProcessStartInfo info = new ProcessStartInfo();
           
            //info.RedirectStandardOutput = false;
            //info.UseShellExecute = true;
            //info.Verb = "runas";
            //Process p = Process.Start(info);
            //string output = p.StandardOutput.ReadToEnd();
            //p.WaitForExit();

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
