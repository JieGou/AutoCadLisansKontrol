
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCadLisansKontrol.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var timeout = 1000*60;
            var scripts = new List<string>();
            

            var ciler = @"-c -f \\cilerturkmen -u adminciler -p ciler471 C:\Users\hikmet\Desktop\checklicense.bat";
            var hikmet = @"-c -f \\hikmetyarbasi -u YARBASI\adminhikmet -p hikmet67 -accepteula C:\Users\hikmet.yarbasi\Desktop\checklicense.bat 2>C:\Users\hikmet.yarbasi\Desktop\output.txt";
            var readcontent = @"-c -f \\hikmetyarbasi -u YARBASI\adminhikmet -p hikmet67  C:\Users\hikmet.yarbasi\Desktop\readlicense.bat 2>C:\Users\hikmet.yarbasi\Desktop\output.txt";
            var readcontent2 = @"psexec \\<hikmetyarbasi> cmd /c  ""type C\%computername%\%computername%.txt""";
            //scripts.Add(hikmet);
            scripts.Add(readcontent);
            var filename = @"C:\Aygaz\PSTools\psexec.exe";
            string errorcontent = "";
            string outputcontent = "";
            foreach (var script in scripts)
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = filename;
                    process.StartInfo.Arguments = script;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.Verb = "runas";
                    StringBuilder output = new StringBuilder();
                    StringBuilder error = new StringBuilder();
                    
                    using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                    using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                    {
                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                outputWaitHandle.Set();
                            }
                            else
                            {
                                output.AppendLine(e.Data);
                            }
                        };
                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                errorWaitHandle.Set();
                            }
                            else
                            {
                                error.AppendLine(e.Data);
                            }
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        if (process.WaitForExit(timeout) &&
                            outputWaitHandle.WaitOne(timeout) &&
                            errorWaitHandle.WaitOne(timeout))
                        {
                            outputcontent +=output.ToString();
                            
                        }
                        else
                        {
                            errorcontent += error.ToString();
                        }
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
        public void powershellscript()
        {

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
