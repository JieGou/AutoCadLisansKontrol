
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
            var scripts = new List<string>();
            scripts.Add(@"-c -f hikmetyarbasi -u adminhikmet -p hikmet67 C:\Users\hikmet.yarbasi\Desktop\checklicense.bat");
           

            ProcessStartInfo info = new ProcessStartInfo(@"C:\Aygaz\PSTools");
            info.FileName = @"C:\Aygaz\PSTools\psexec.exe";
            info.Arguments = @"-c -f hikmetyarbasi -u adminhikmet -p hikmet67 C:\Users\hikmet.yarbasi\Desktop\checklicense.bat";
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.Verb = "runas";
            Process p = Process.Start(info);


            string output = p.StandardOutput.ReadToEnd();

            p.WaitForExit();

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
        public void powershellscript() {

            //var scripts = new List<string>();
            //var FileName = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\checklicense.bat";
            //var command = System.IO.File.ReadAllText(FileName);


            //var createfile = "$s=\" " + command + "\"\n" +
            ////"$s >> \".\\checklicense.bat\" -Encoding \"UTF8\""+
            //"out-file -filepath \".\\checklicense.bat\" -inputobject $s -encoding UTF8";

            //scripts.Add("New-Item -Force -ItemType directory -Path C:\\$env:computername");
            //// scripts.Add("DEL \\F \\S \\Q \\A \"C:\\%computername%\\*");
            //scripts.Add("cd C:\\$env:computername");
            //scripts.Add(createfile);
            //// scripts.Add("cmd.exe /c checklicense.bat");
            //scripts.Add("Get-Content C:\\$env:computername\\$env:computername.txt -Raw");
            //var username = "adminciler";
            //var password = ConvertToSecureString("ciler471");
            //string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
            //PSCredential remoteCredential = new PSCredential(username, password);
            //WSManConnectionInfo connectionInfo = new WSManConnectionInfo(false, "CILERTURKMEN", 5985, "/wsman", shellUri, remoteCredential);


            //using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
            //{
            //    runspace.Open();
            //    foreach (var script in scripts)
            //    {
            //        Pipeline pipeline = runspace.CreatePipeline();
            //        pipeline.Commands.AddScript(script);
            //        var results = pipeline.Invoke();
            //    }
            //}

            //ProcessStartInfo psi = new ProcessStartInfo(@"C:\Windows\System32\WindowsPowerShell\v1.0\");
            //psi.FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
            //psi.Arguments = " $username = \"adminciler\" \n"+
            //                " $password = \"ciler471\" \n" +
            //                " $secstr = New - Object - TypeName System.Security.SecureString \n" +
            //                " $password.ToCharArray() | ForEach - Object {$secstr.AppendChar($_)} \n" +
            //                " $cred = new- object - typename System.Management.Automation.PSCredential - argumentlist $username, $secstr \n" +
            //                " Invoke - Command - ComputerName CILERTURKMEN - ScriptBlock { Get - ChildItem C:\\ } -credential $cred \n";
            //psi.UseShellExecute = false;
            //psi.Verb = "runas";
            //psi.RedirectStandardOutput = true;
            //Process process = new Process();
            //process.StartInfo = psi;
            //process.Start();
            //string output = process.StandardOutput.ReadToEnd();
            //process.WaitForExit();

        }
    }
}
