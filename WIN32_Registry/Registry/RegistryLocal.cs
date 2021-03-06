using System;
using System.Collections;
using System.Management;
using System.Collections.Generic;
using System.Text;

namespace baileySoft.Wmi.Registry
{
    public class RegistryLocal:RegistryObject
    {
        #region "fields"
        ConnectionOptions options;
        ManagementScope connectionScope;
        #endregion

        #region "constructors"
        public RegistryLocal(string provider)
        {
            options = RegistryConnection.RegistryConnectionOptions();
            Connect(provider);
            GetRegistryProperties();
        }
        #endregion

        #region "polymorphic methods"
        public override void CreateKey(baseKey RootKey, string key)
        {
            if (IsConnected)
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
        public override void GetRegistryProperties()
        {
            if (IsConnected)
            {
                RegistryConnection.GetRegistryProperties(Environment.MachineName,
                                                                    options, this);
            }
        }
        public override bool Connect(string provider)
        {
            connectionScope = RegistryConnection.ConnectionScope(Environment.MachineName,
                                                                    options, this, provider);
            return this.IsConnected;
        }
        #endregion
    
    }
}
