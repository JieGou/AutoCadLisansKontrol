

using MaterialDesignDemo.autocad.masterkey.ws;
using MaterialDesignDemo.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using System.Management;
using MaterialDesignDemo.Controller;

namespace AutoCadLisansKontrol.Controller
{
    public class LicenseDetection
    {

        public static CheckLicenseModel ExecutePowerShell(CheckLicenseModel chc, string username, string password, int opreationid)
        {
            //Environment.SetEnvironmentVariable("clientname", chc.Ip);
            //Environment.SetEnvironmentVariable("username", username);
            //Environment.SetEnvironmentVariable("password", password);
            //ProcessStartInfo info = new ProcessStartInfo();
            //info.FileName = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\checklicense.bat";
            //var arg = @"-c -f " + chc.Ip + " -u " + username + " -p " + password + " ";
            //info.RedirectStandardOutput = false;
            //info.UseShellExecute = true;
            //info.Verb = "runas";
            //Process p = Process.Start(info);
            //string output = p.StandardOutput.ReadToEnd();
            //p.WaitForExit();
            //Thread.Sleep(5000);

            var scripts = new List<string>();
            var FileName = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\checklicense.bat";
            var command = System.IO.File.ReadAllText(FileName);


            var createfile = "$s=\" " + command + "\"\n" +
            //"$s >> \".\\checklicense.bat\" -Encoding \"UTF8\""+
            "out-file -filepath \".\\checklicense.bat\" -inputobject $s -encoding UTF8";

            scripts.Add("New-Item -Force -ItemType directory -Path C:\\$env:computername");
            // scripts.Add("DEL \\F \\S \\Q \\A \"C:\\%computername%\\*");
            scripts.Add("cd C:\\$env:computername");
            scripts.Add(createfile);
            scripts.Add("cmd.exe /c checklicense.bat");
            scripts.Add("Get-Content C:\\$env:computername\\$env:computername.txt -Raw");
            scripts.Add("Remove-Item C:\\$env:computername\\$env:computername.txt");
            scripts.Add("Remove-Item C:\\$env:computername\\log.txt");
            scripts.Add("Remove-Item C:\\$env:computername\\checklicense.bat");
            var securepassword = ConvertToSecureString(password);
            string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
            PSCredential remoteCredential = new PSCredential(username, securepassword);
            WSManConnectionInfo connectionInfo = new WSManConnectionInfo(false, chc.Name, 5985, "/wsman", shellUri, remoteCredential);

            string output = "";
            try
            {
                using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
                {
                    runspace.Open();
                    foreach (var script in scripts)
                    {

                        Pipeline pipeline = runspace.CreatePipeline();
                        pipeline.Commands.AddScript(script);
                        var results = pipeline.Invoke();
                        if (script.Contains("Get-Content"))
                        {
                            output = results[0].ToString();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
                chc.Success = false;
            }
            chc.Output = output;
            chc.IsUnlicensed = true;
            chc.CheckDate = DateTime.Now;
            chc.OperationId = opreationid;
            chc.UpdateDate = DateTime.Now;
            return chc;
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
        public static CheckLicenseModel ExecutePsexec(CheckLicenseModel chc, string username, string password, int operationid)
        {

            //if (!ProcessWMI.Connect(chc.Name, username, password, out error))
            //{
            //    output.Append(error.ToString());
            //    chc.Fail = true;
            //    chc.Output = output.ToString();
            //    return chc;
            //}

            var timeout = 1000 * 60;
            var scripts = new List<RemoteCommand>();

            var filename = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\PSTools\psexec.exe";
            var CheckFileName = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\checklicense.bat";
            var ReadFileName = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\readlicense.bat";
            var TestConnectionFileName = System.IO.Directory.GetCurrentDirectory() + @"\BatFile\TestConnection.bat";

            var checkscript = @" -c -f \\" + chc.Name + " -u " + username + " -p " + password + " " + CheckFileName;
            var readscript = @"-c -f \\" + chc.Name + " -u " + username + " -p " + password + " " + ReadFileName;
            var testconnectionscript = @"-c -f \\" + chc.Name + " -u " + username + " -p " + password + " ipconfig";


            //scripts.Add(new RemoteCommand { Key = "testconnection", command = readscript, timeout = 1000 * 60 });

            scripts.Add(new RemoteCommand { Key = "checklicense", command = checkscript, timeout = 1000 * 60 });
            scripts.Add(new RemoteCommand { Key = "readlicense", command = readscript, timeout = 1000 * 60 });

            string outputcontent = "";
            string errorcontent = "";


            try
            {
                foreach (var script in scripts)
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = filename;
                        process.StartInfo.Arguments = script.command;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.Verb = "runas";
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        StringBuilder output = new StringBuilder();
                        StringBuilder error = new StringBuilder();
                        using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                        using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                        {
                            process.OutputDataReceived += (sender, e) =>
                            {
                                if (e.Data == null)
                                {
                                    try
                                    {

                                        outputWaitHandle.Set();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
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
                                    try
                                    {
                                        errorWaitHandle.Set();

                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                                else
                                {
                                    error.AppendLine(e.Data);
                                }
                            };

                            process.Start();


                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            if (process.WaitForExit(script.timeout) &&
                                outputWaitHandle.WaitOne(script.timeout) &&
                                errorWaitHandle.WaitOne(script.timeout))
                            {
                                outputcontent += output.ToString();
                            }
                            else
                            {
                                errorcontent += error.ToString();
                            }

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                outputcontent = ex.Message;
            }
            chc.Error = errorcontent.ToString();
            chc.Output = outputcontent.ToString();
            return chc;
        }

        public static CheckLicenseModel ExecuteWMI(string[] software, CheckLicenseModel chc, string username, string password, int opreationid)
        {
            try
            {

                chc.IsFound = false;
                chc.Fail = false;
                chc.Success = false;
                var softwares = "";

                ProcessWMI process = new ProcessWMI();
                var win32product = process.GetProductWithWMI(software, chc.Name, username, password);

              
                if (win32product.Count > 0)
                {
                    chc.IsFound = true;
                    foreach (var item in win32product)
                    {
                        softwares += item.Name + " (Installed)" + "\n\r";
                    }
                    chc.Success = true;
                    chc.IsFound = true;
                    chc.Installed = true;
                    chc.Uninstalled = false;
                    chc.Output = softwares;
                    return chc;
                }
                
                var registeryproduct = process.ReadRegisteryusingWMI(software, chc.Name, username, password);

               
                if (registeryproduct.Count > 0)
                {
                    foreach (var item in registeryproduct)
                    {
                        softwares += item.DisplayName + " (Uninstalled)" + "\n\r";
                    }
                    chc.Success = true;
                    chc.IsFound = true;
                    chc.Uninstalled = true;
                    chc.Installed = false;
                    chc.Output = softwares;
                    return chc;
                }


                chc.Success = true;
                chc.IsFound = false;
                chc.Uninstalled = false;
                chc.Installed = false;
                chc.Output = "Software not found";


            }
            catch (Exception ex)
            {
                chc.Output = ex.Message;
                chc.Fail = true;
            }

            return chc;
        }

        internal class RemoteCommand
        {
            public string Key { get; set; }
            public string command { get; set; }
            public int timeout { get; set; }
        }
    }
}
