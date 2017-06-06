

using LicenseController.autocad.masterkey.ws;
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
using LicenseController.Properties;
using ORTEC_WCF;
using System.Globalization;

namespace AutoCadLisansKontrol.Controller
{
    public class LicenseDetection :IDisposable
    {
        public LicenseController.autocad.masterkey.ws.Service1Client client = new LicenseController.autocad.masterkey.ws.Service1Client();
        static OutputSniff outputs = new OutputSniff();
        public CheckLicenseModel ExecutePowerShell(CheckLicenseModel chc, string username, string password, int opreationid)
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
        public CheckLicenseModel ExecutePsexec(CheckLicenseModel chc, string username, string password, int operationid)
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

        public void ExecuteWMIForOneApp(CheckLicenseModel chc, string username, string password, int opreationidi, List<ControlPointDTO> checklist, bool isRemote)
        {
            var levelid = 0;
            var softwares = "";
            var software = chc.App;
            var guids = Guid.NewGuid();
            var guidId = guids.ToString();
            var logs = new List<LogData>();
            var remote = String.IsNullOrEmpty(chc.Name) ? chc.Ip : chc.Name;
            var xmlreq = new RequestedXml { ComputerName = chc.Name, Ip = chc.Ip, IsRemote = isRemote.ToString(), Password = password, UserName = username, WorkDate = DateTime.Now.ToString("dd-MM-yyyy") };
            ProcessWMI process = new ProcessWMI(remote, username, password, isRemote);

            try
            {
                logs.Add(new LogData { AppName = remote, Id = guidId, StartTime = DateTime.Now, Method = "ExecuteWMI", LogDataType = LogDataType.InitiliazeProcess, ComputerId = (int)chc.ComputerId });
                logs.Add(new LogData { AppName = remote, ReqXml = GenericDataContractSerializer<RequestedXml>.SerializeObject(xmlreq), Id = guidId, LevelId = levelid, Method = "ExecuteWMI", StartTime = DateTime.Now, LogDataType = LogDataType.InitiliazeItemOfProcess, OperationId = (int)chc.OperationId });
                chc.LogId = guids;
                chc.InitiliazeObject();
                process.Connect();

                foreach (var item in checklist)
                {
                    if (item.Name == "Add or Remove Programs" && item.WillChecked == true)
                    {
                        var addremove = "";
                        try
                        {
                            levelid++;
                            logs.Add(new LogData { ReqXml = GenericDataContractSerializer<ControlPointDTO>.SerializeObject(item), Id = guidId, LevelId = levelid, Method = "Add or Remove Programs", StartTime = DateTime.Now, LogDataType = LogDataType.InitiliazeItemOfProcess });
                            addremove = "ADD OR REMOVE PROGRAMS" + "\n\r" + "\n\r";
                            var win32product = process.GetProductWithWMI(software);
                            outputs.Win32_products = win32product;
                            if (win32product.Count > 0)
                            {
                                foreach (var prd in win32product)
                                {
                                    addremove += prd.Name + " InstallDate : " + prd.InstallDate + ")" + "\n\r";
                                }
                                chc.Installed = true;
                                chc.IsFound = true;
                                chc.SerialNumber = win32product[0].ProductID;
                            }
                            else
                            {
                                addremove += "Programs are not found" + "\n\r";
                            }
                        }
                        catch (Exception ex)
                        {

                            addremove += "Fail : " + ex.Message + "\n\r";
                        }
                        softwares += addremove;
                        logs.Add(new LogData { Id = guidId, ResXml = addremove, State = LogDataState.Success, LevelId = levelid, EndTime = DateTime.Now, LogDataType = LogDataType.UpdateItemOfProcess });

                    }
                    if (item.Name == "Registry Key" && item.WillChecked == true)
                    {
                        var registerys = "";
                        try
                        {
                            levelid++;
                            logs.Add(new LogData { ReqXml = GenericDataContractSerializer<ControlPointDTO>.SerializeObject(item), Id = guidId, LevelId = levelid, Method = "Registry Key", StartTime = DateTime.Now, LogDataType = LogDataType.InitiliazeItemOfProcess });
                            registerys += "\n\r" + "REGISTERY KEY" + "\n\r" + "\n\r";

                            var registeryproduct = process.ReadRegisteryusingWMI(software);

                            outputs.RegistryAutoDesk = registeryproduct;
                            if (registeryproduct.Count > 0)
                            {
                                foreach (var reg in registeryproduct)
                                {
                                    registerys += reg.DisplayName + " InstallDate" + reg.InstallDate + ")" + "\n\r";
                                }
                                chc.Installed = true;
                                chc.IsFound = true;
                            }
                            else
                            {
                                registerys += "Registry keys are not found." + "\n\r";
                            }
                        }
                        catch (Exception ex)
                        {

                            registerys += "Fail : " + ex.Message + "\n\r";
                        }
                        softwares += registerys;

                        logs.Add(new LogData { Id = guidId, ResXml = registerys, State = LogDataState.Success, LevelId = levelid, EndTime = DateTime.Now, LogDataType = LogDataType.UpdateItemOfProcess });

                    }
                    if (item.Name == "Application Events" && item.WillChecked == true)
                    {
                        levelid++;
                        var appevent = "";
                        logs.Add(new LogData { ReqXml = GenericDataContractSerializer<ControlPointDTO>.SerializeObject(item), Id = guidId, LevelId = levelid, Method = "Application Events", StartTime = DateTime.Now, LogDataType = LogDataType.InitiliazeItemOfProcess });
                        try
                        {
                            appevent += "APPLICATION EVENTS" + "\n\r" + "\n\r";

                            var events = process.GetApplicationEvent(software);

                            outputs.ApplicationEvents = events;
                            if (events.Count > 0)
                            {
                                foreach (var evt in events)
                                {
                                    appevent += evt.Message + "(" + evt.TimeWritten + ")" + "\n\r";
                                }
                                if (chc.Installed == false)
                                    chc.Uninstalled = true;
                                chc.IsFound = true;
                            }
                            else
                            {
                                appevent += "Application events are not found" + "\n\r";
                            }
                        }
                        catch (Exception ex)
                        {

                            appevent += "Fail : " + ex.Message + "\n\r";
                        }
                        softwares += appevent;
                        logs.Add(new LogData { Id = guidId, ResXml = appevent, State = LogDataState.Success, LevelId = levelid, EndTime = DateTime.Now, LogDataType = LogDataType.UpdateItemOfProcess });

                    }
                    if (item.Name == "File Explorer" && item.WillChecked == true)
                    {
                        var fileexplorer = "";
                        levelid++;
                        logs.Add(new LogData { ReqXml = GenericDataContractSerializer<ControlPointDTO>.SerializeObject(item), Id = guidId, LevelId = levelid, Method = "File Explorer", StartTime = DateTime.Now, LogDataType = LogDataType.InitiliazeItemOfProcess });
                        try
                        {


                            fileexplorer += "FILE EXPLORER" + "\n\r" + "\n\r" + "\n\r";

                            var model = process.DirectoryChecklist(client.GetFEControlList(software.Id).ToList());


                            if (model.directories.Count > 0 || model.files.Count > 0)
                            {
                                foreach (var dir in model.directories)
                                {
                                    fileexplorer += dir.Name + " InstallDate :  (" + dir.InstallDate + ")" + "\n\r";
                                }
                                foreach (var dir in model.files)
                                {
                                    fileexplorer += dir.Name + "  InstallDate : (" + dir.InstallDate + ")" + "\n\r";
                                }
                                outputs.FileExplorerModel = model;
                                if (chc.Installed == false)
                                    chc.Uninstalled = true;
                                chc.IsFound = true;
                            }
                        }
                        catch (Exception ex)
                        {

                            fileexplorer += "Fail : " + ex.Message + "\n\r";
                        }
                        softwares += fileexplorer;
                        logs.Add(new LogData { Id = guidId, ResXml = fileexplorer, State = LogDataState.Success, LevelId = levelid, EndTime = DateTime.Now, LogDataType = LogDataType.UpdateItemOfProcess });
                    }
                }
                
                chc.Success = true;

                if (outputs.Win32_products != null && outputs.Win32_products.Count > 0)
                {
                    try
                    {
                        chc.InstallDate = outputs.Win32_products.GroupBy(s => s.ProductID).Select(s => s.OrderByDescending(x => x.InstallDate).FirstOrDefault().InstallDate).First();
                        chc.Installed = true;
                        chc.Uninstalled = false;
                        chc.Description = "Install Date is found from add or remove programs...";
                    }
                    catch (Exception ex)
                    {
                        chc.Output += "Fail When Get Install Date from Win32_products  :" + ex.Message + "\n\r";
                    }

                }
                else if (outputs.RegistryAutoDesk != null && outputs.RegistryAutoDesk.Count > 0)
                {
                    try
                    {
                        chc.InstallDate = outputs.RegistryAutoDesk.Where(x => x.InstallDate != null).GroupBy(s => s.DisplayName).Select(s => s.OrderByDescending(x => x.InstallDate).FirstOrDefault().InstallDate).First();
                        chc.Installed = true;
                        chc.Uninstalled = false;
                        chc.Description = "Install Date is found from registery keys...";
                    }
                    catch (Exception ex)
                    {
                        chc.Output += "Fail When Get Install Date from RegistryAutoDesk  :" + ex.Message + "\n\r";
                    }
                }
                else if (outputs.ApplicationEvents != null && outputs.ApplicationEvents.Count > 0)
                {
                    try
                    {
                        chc.UnInstallDate = outputs.ApplicationEvents.GroupBy(s => s.ComputerName).Select(s => s.OrderByDescending(x => x.TimeWritten).FirstOrDefault().TimeWritten).First();
                        chc.Installed = false;
                        chc.Uninstalled = true;
                        chc.Description = "UnInstall Date is found from Application Events...";
                    }
                    catch (Exception ex)
                    {
                        chc.Output += "Fail When Get Install Date from ApplicationEvents :" + ex.Message + "\n\r";
                    }

                }
                else if (outputs.FileExplorerModel != null && outputs.FileExplorerModel.directories.Count > 0)
                {
                    try
                    {
                        chc.UnInstallDate = outputs.FileExplorerModel.directories.GroupBy(s => s.Name).Select(s => s.OrderByDescending(x => x.LastAccessed).FirstOrDefault().LastAccessed).First();
                        chc.Installed = false;
                        chc.Uninstalled = true;
                        chc.Description = "UnInstall Date is found from File Explorer";
                    }
                    catch (Exception ex)
                    {
                        chc.Output += "Fail When Get Install Date from FileExplorerModel :" + ex.Message + "\n\r";
                    }
                }
                chc.Success = true;
                chc.Output = softwares;

            }
            catch (Exception ex)
            {
                softwares += "\r\n" + ex.Message;
                chc.Fail = true;
                chc.Success = false;
                chc.Output = softwares;
                logs.Add(new LogData { AppName = remote, Id = guidId, ResXml = chc.Output, State = LogDataState.Success, LevelId = 0, EndTime = DateTime.Now, LogDataType = LogDataType.UpdateItemOfProcess });
                logs.Add(new LogData() { AppName = remote, Id = guidId, EndTime = DateTime.Now, LogDataType = LogDataType.UpdateProcess });
                try
                {
                    client.LogToDb(logs.ToArray());
                }
                catch (Exception e)
                {
                    var message = e.Message;
                }

                chc.QueryState = CategorizeProcessResult(chc.Output, chc.App.AppName);
                chc.State = (int)chc.QueryState;

                return;
            }
            try
            {
                logs.Add(new LogData { AppName = remote, Id = guidId, ResXml = chc.Output, State = LogDataState.Success, LevelId = 0, EndTime = DateTime.Now, LogDataType = LogDataType.UpdateItemOfProcess });
                logs.Add(new LogData() { AppName = remote, Id = guidId, EndTime = DateTime.Now, LogDataType = LogDataType.UpdateProcess });
                client.LogToDb(logs.ToArray());
                chc.QueryState = CategorizeProcessResult(chc.Output, chc.App.AppName);
                chc.State = (int)chc.QueryState;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return;
        }
        public static QueryState CategorizeProcessResult(string output, string application)
        {
            try
            {
                if (output.Contains("User credentials cannot be used for local connections"))
                {
                    return QueryState.User_credentials_cannot_be_used_for_local_connections;
                }
                if (output.Contains("failed with the following error Access is denied") || output.Contains("failed with the following error Erişim engellendi"))
                {
                    return QueryState.User_not_authorized_to_login_computer;
                }
                if (output.Contains("failed with the following error The RPC server is unavailable.") || output.Contains("failed with the following error RPC sunucusu kullanılamıyor."))
                {
                    return QueryState.Remote_computer_doesnt_respond_Maybe_it_is_switched_off_or_WMI_service_is_not_running_on_it;
                }
                if (output.Contains(application))
                {
                    return QueryState.Product_found;
                }
                if (!output.Contains(application))
                {
                    return QueryState.No_Product_found;
                }
                {
                    return QueryState.UnKnownState;
                }
            }
            catch (Exception ex)
            {

                return QueryState.UnKnownState;
            }

        }

        internal class RemoteCommand
        {
            public string Key { get; set; }
            public string command { get; set; }
            public int timeout { get; set; }
        }
        public class RequestedXml
        {
            public string ComputerName { get; set; }
            public string Ip { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string IsRemote { get; set; }
            public string WorkDate { get; set; }

        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
