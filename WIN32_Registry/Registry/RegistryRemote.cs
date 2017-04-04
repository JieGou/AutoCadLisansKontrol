using System;
using System.Collections;
using System.Management;
using System.Collections.Generic;
using System.Text;

namespace baileySoft.Wmi.Registry
{
    public class RegistryRemote : RegistryObject
    {
        #region "fields"
        private string userName; //object placeholders
        private string password;
        private string domain;
        private string machineName;
        private ManagementScope connectionScope;
        private ConnectionOptions options;

        #endregion

        #region "constructors"
        public RegistryRemote(string userName,
                             string password,
                             string domain,
                             string machineName,
                             string provider)
        {
            this.userName = userName;
            this.password = password;
            this.domain = domain;
            this.machineName = machineName;
            options = RegistryConnection.RegistryConnectionOptions();

            Connect(provider);
            GetRegistryProperties();
        }
        #endregion

        #region "polymorphic methods"
        public override void CreateKey(baseKey RootKey, string key)
        {
            if (this.IsConnected)
                try
                {
                    RegistryMethod.CreateKey(connectionScope, RootKey, key);
                }
                catch (Exception err)
                {
                    throw err;
                }
        }
        public override void DeleteKey(baseKey RootKey, string key)
        {
            if (IsConnected)
                try
                {
                    RegistryMethod.DeleteKey(connectionScope, RootKey, key);
                }
                catch (Exception err)
                {
                    throw err;
                }
        }
        public override ArrayList EnumerateKeys(baseKey RootKey, string key)
        {
            ArrayList subKeys = new ArrayList();
            if (IsConnected)
                try
                {
                    subKeys.AddRange(RegistryMethod.EnumerateKeys(connectionScope, RootKey, key));
                }
                catch (Exception err)
                {
                    throw err;
                }
            return subKeys;
        }
        public override ArrayList EnumerateValues(baseKey RootKey, string key)
        {
            ArrayList alValues = new ArrayList();
            if (IsConnected)
                try
                {
                    alValues.AddRange(RegistryMethod.EnumerateValues(connectionScope, RootKey, key));
                    alValues.Sort();
                }
                catch (Exception err)
                {
                    throw err;
                }
            return alValues;
        }
        public override string GetValue(baseKey RootKey, string key, string valueName, valueType ValueType)
        {
            string Value = string.Empty;
            if (IsConnected)
                try
                {
                    Value = RegistryMethod.GetValue(connectionScope, RootKey, key, valueName, ValueType);
                }
                catch (Exception err)
                {
                    throw err;
                }
            return Value;
        }
        public override void SetValue(baseKey RootKey, string key, string valueName, string value, valueType ValueType)
        {
            if (IsConnected)
                try
                {
                    RegistryMethod.SetValue(connectionScope, RootKey, key, valueName, value, ValueType);
                }
                catch (Exception err)
                {
                    throw err;
                }
        }
        public override void GetRegistryProperties()
        {
            if (IsConnected)
            {
                RegistryConnection.GetRegistryProperties(machineName, options, this);
            }
        }
        public override void CreateValue(baseKey RootKey, string key, string valueName, string value)
        {
            if (IsConnected)
                try
                {
                    RegistryMethod.CreateValue(connectionScope, RootKey, key, valueName, value);
                }
                catch (Exception err)
                {
                    throw err;
                }
        }
        public override void DeleteValue(baseKey RootKey, string key, string valueName)
        {
            if (IsConnected)
                try
                {
                    RegistryMethod.DeleteValue(connectionScope, RootKey, key, valueName);
                }
                catch (Exception err)
                {
                    throw err;
                }
        }
        public override bool Connect(string provider)
        {
            if (domain != null || userName != null)
            {
                options.Username = userName;
                options.Password = password;
                options.Impersonation = ImpersonationLevel.Impersonate;
            }
            connectionScope = RegistryConnection.ConnectionScope(machineName, options, this,provider);
            return this.IsConnected;
        }
        #endregion     
    }
}
