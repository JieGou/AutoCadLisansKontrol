using System;
using System.Management;
using System.Collections.Generic;
using System.Text;

namespace baileySoft.Wmi.Registry
{
    public class RegistryConnection
    {
        public static void GetRegistryProperties(string machineName,
                                       ConnectionOptions options,
                                       RegistryObject propertiesHolder)
        {
            ManagementScope ms = new ManagementScope(@"\\" + machineName + @"\root\CIMV2", options);
            SelectQuery msQuery = new SelectQuery("SELECT * FROM Win32_Registry");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(ms, msQuery);

            foreach (ManagementObject item in searchProcedure.Get())
            {
                propertiesHolder.Caption = item["Caption"]==null?null:item["Caption"].ToString();
                propertiesHolder.CurrentSize = item["CurrentSize"] == null ? null : item["CurrentSize"].ToString();
                propertiesHolder.Description = item["Description"] == null ? null : item["Description"].ToString();
                propertiesHolder.InstallDate = item["InstallDate"] == null ? null : item["InstallDate"].ToString();
                propertiesHolder.MaximumSize = item["MaximumSize"] == null ? null : item["MaximumSize"].ToString();
                propertiesHolder.Name = item["Name"] == null ? null : item["Name"].ToString();
                propertiesHolder.ProposedSize = item["ProposedSize"] == null ? null : item["ProposedSize"].ToString();
                propertiesHolder.Status = item["Status"] == null ? null : item["Status"].ToString();
            }
        }
        public static ConnectionOptions RegistryConnectionOptions()
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;
            return options;
        }
        public static ManagementScope ConnectionScope(string machineName,
                                                      ConnectionOptions options,
                                                      RegistryObject propertiesHolder,
                                                      string provider)
        {
            ManagementScope connectScope = new ManagementScope();
            connectScope.Path = new ManagementPath(@"\\" + machineName + @"\root\DEFAULT:StdRegProv");
            connectScope.Options = options;
            connectScope.Options.Context.Add("__ProviderArchitecture", Convert.ToInt32(provider));
            connectScope.Options.Context.Add("__RequiredArchitecture", true);
            try
            {
                connectScope.Connect();
                propertiesHolder.IsConnected = true;
            }
            catch
            {
                propertiesHolder.IsConnected = false;
            }
            return connectScope;
        }
    }
}
