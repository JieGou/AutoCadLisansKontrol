using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MaterialDesignDemo.Controller
{
    public class ProcessWMI
    {
        public uint ProcessId;
        public int ExitCode;
        public bool EventArrived;
        public ManualResetEvent mre = new ManualResetEvent(false);
        public void ProcessStoptEventArrived(object sender, EventArrivedEventArgs e)
        {
            if ((uint)e.NewEvent.Properties["ProcessId"].Value == ProcessId)
            {
                Console.WriteLine("Process: {0}, Stopped with Code: {1}", (int)(uint)e.NewEvent.Properties["ProcessId"].Value, (int)(uint)e.NewEvent.Properties["ExitStatus"].Value);
                ExitCode = (int)(uint)e.NewEvent.Properties["ExitStatus"].Value;
                EventArrived = true;
                mre.Set();
            }
        }
        public ProcessWMI()
        {
            this.ProcessId = 0;
            ExitCode = -1;
            EventArrived = false;
        }
        public void ExecuteRemoteProcessWMI(string remoteComputerName, string username, string password, string arguments, int WaitTimePerCommand)
        {
            string strUserName = string.Empty;
            try
            {
                ConnectionOptions connOptions = new ConnectionOptions();
                connOptions.Impersonation = ImpersonationLevel.Impersonate;
                connOptions.EnablePrivileges = true;
                connOptions.Username = username;
                connOptions.Password = password;

                ManagementScope manScope = new ManagementScope(String.Format(@"\\{0}\ROOT\CIMV2", remoteComputerName), connOptions);

                try
                {
                    manScope.Connect();
                }
                catch (Exception e)
                {
                    throw new Exception("Management Connect to remote machine " + remoteComputerName + " as user " + strUserName + " failed with the following error " + e.Message);
                }
                ObjectGetOptions objectGetOptions = new ObjectGetOptions();
                ManagementPath managementPath = new ManagementPath("Win32_Process");
                using (ManagementClass processClass = new ManagementClass(manScope, managementPath, objectGetOptions))
                {
                    using (ManagementBaseObject inParams = processClass.GetMethodParameters("Create"))
                    {
                        inParams["CommandLine"] = arguments;
                        using (ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, null))
                        {

                            if ((uint)outParams["returnValue"] != 0)
                            {
                                throw new Exception("Error while starting process " + arguments + " creation returned an exit code of " + outParams["returnValue"] + ". It was launched as " + strUserName + " on " + remoteComputerName);
                            }
                            this.ProcessId = (uint)outParams["processId"];
                        }
                    }
                }

                SelectQuery CheckProcess = new SelectQuery("Select * from Win32_Process Where ProcessId = " + ProcessId);
                using (ManagementObjectSearcher ProcessSearcher = new ManagementObjectSearcher(manScope, CheckProcess))
                {
                    using (ManagementObjectCollection MoC = ProcessSearcher.Get())
                    {
                        if (MoC.Count == 0)
                        {
                            throw new Exception("ERROR AS WARNING: Process " + arguments + " terminated before it could be tracked on " + remoteComputerName);
                        }
                    }
                }

                WqlEventQuery q = new WqlEventQuery("Win32_ProcessStopTrace");
                using (ManagementEventWatcher w = new ManagementEventWatcher(manScope, q))
                {
                    w.EventArrived += new EventArrivedEventHandler(this.ProcessStoptEventArrived);
                    w.Start();
                    if (!mre.WaitOne(WaitTimePerCommand, false))
                    {
                        w.Stop();
                        this.EventArrived = false;
                    }
                    else
                        w.Stop();
                }
                if (!this.EventArrived)
                {
                    SelectQuery sq = new SelectQuery("Select * from Win32_Process Where ProcessId = " + ProcessId);
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(manScope, sq))
                    {
                        foreach (ManagementObject queryObj in searcher.Get())
                        {
                            queryObj.InvokeMethod("Terminate", null);
                            queryObj.Dispose();
                            throw new Exception("Process " + arguments + " timed out and was killed on " + remoteComputerName);
                        }
                    }
                }
                else
                {
                    if (this.ExitCode != 0)
                        throw new Exception("Process " + arguments + "exited with exit code " + this.ExitCode + " on " + remoteComputerName + " run as " + strUserName);
                    else
                        Console.WriteLine("process exited with Exit code 0");
                }

            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Execute process failed Machinename {0}, ProcessName {1}, RunAs {2}, Error is {3}, Stack trace {4}", remoteComputerName, arguments, strUserName, e.Message, e.StackTrace), e);
            }
        }

        public void GetProductWithWMI()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            foreach (ManagementObject mo in mos.Get())
            {
                Console.WriteLine(mo["Name"]);
            }
        }

        public List<Software> ReadRegisteryusingWMI(string[] software)
        {

            var list32 = ReadRegistryusingWMICore(software, @"Software\Microsoft\Windows\CurrentVersion\Uninstall");
            var list64 = ReadRegistryusingWMICore(software, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

            var uniqueList = list32.Concat(list64)
                                   .GroupBy(item => item.DisplayName)
                                   .Select(group => group.First())
                                   .ToList();

            return uniqueList;
        }
        private List<Software> ReadRegistryusingWMICore(string[] software, string softwareRegLoc)
        {
            List<Software> programs = new List<Software>();

            ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2");
            scope.Connect();

            ManagementClass registry = new ManagementClass(scope, new ManagementPath("StdRegProv"), null);
            ManagementBaseObject inParams = registry.GetMethodParameters("EnumKey");
            inParams["hDefKey"] = 0x80000002;//HKEY_LOCAL_MACHINE
            inParams["sSubKeyName"] = softwareRegLoc;

            // Read Registry Key Names 
            ManagementBaseObject outParams = registry.InvokeMethod("EnumKey", inParams, null);
            string[] programGuids = outParams["sNames"] as string[];
            var searchingproduct = programGuids.Where(x => software.Select(y => y.ToUpper()).ToList().Contains(x.ToUpper())).ToList();
            foreach (string subKeyName in searchingproduct)
            {
                var item = new Software();
                inParams = registry.GetMethodParameters("GetStringValue");
                inParams["sSubKeyName"] = softwareRegLoc + @"\" + subKeyName;

                inParams["sValueName"] = "InstallDate";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.InstallDate = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "Contact";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.Contact = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "DisplayIcon";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.DisplayIcon = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "DisplayName";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.DisplayName = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "DisplayVersion";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.DisplayVersion = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "EstimatedSize";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.EstimatedSize = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "HelpLink";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.HelpLink = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "HelpTelephone";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.HelpTelephone = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "InstallLocation";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.InstallLocation = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "InstallSource ";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.InstallSource = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "Language";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.Language = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "ModifyPath_Hidden";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.ModifyPath_Hidden = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "NoModify";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.NoModify = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "NoRemove";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.NoRemove = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "NoRepair";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.NoRepair = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "Publisher";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.Publisher = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "Readme";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.Readme = outParams.Properties["sValue"].Value.ToString();
                }

                inParams["sValueName"] = "UninstallString";
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                if (outParams.Properties["sValue"].Value != null)
                {
                    item.UninstallString = outParams.Properties["sValue"].Value.ToString();
                }
                programs.Add(item);
            }

            return programs;
        }
    }
    public class Software
    {
        public string Comments { get; set; }
        public string Contact { get; set; }
        public string DisplayIcon { get; set; }
        public string DisplayName { get; set; }
        public string DisplayVersion { get; set; }
        public string EstimatedSize { get; set; }
        public string HelpLink { get; set; }
        public string HelpTelephone { get; set; }
        public string InstallDate { get; set; }
        public string InstallLocation { get; set; }
        public string InstallSource { get; set; }
        public string Language { get; set; }
        public string ModifyPath_Hidden { get; set; }
        public string NoModify { get; set; }
        public string NoRemove { get; set; }
        public string NoRepair { get; set; }
        public string Publisher { get; set; }
        public string Readme { get; set; }
        public string UninstallString { get; set; }


    }
}
