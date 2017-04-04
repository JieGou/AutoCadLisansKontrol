using System;
using System.Collections;
using System.Management;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace baileySoft.Wmi.Registry
{
    public class RegistryMethod
    {
        public static void CreateKey(ManagementScope connectionScope,
                                     baseKey BaseKey,
                                     string key)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("CreateKey");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("CreateKey",
                                                                        methodParams, null);
        }
        public static void DeleteKey(ManagementScope connectionScope,
                                     baseKey BaseKey,
                                     string key)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("DeleteKey");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("DeleteKey",
                                                                        methodParams, null);
        }
        public static void CreateValue(ManagementScope connectionScope,
                                       baseKey BaseKey,
                                       string key,
                                       string valueName,
                                       string value)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("SetStringValue");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;
            methodParams["sValue"] = value;
            methodParams["sValueName"] = valueName;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("SetStringValue",
                                                                        methodParams, null);
        }
        public static void DeleteValue(ManagementScope connectionScope,
                                       baseKey BaseKey,
                                       string key,
                                       string valueName)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("DeleteValue");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;
            methodParams["sValueName"] = valueName;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("DeleteValue",
                                                                        methodParams, null);
        }
        public static List<string> EnumerateKeys(ManagementScope connectionScope,
                                             baseKey BaseKey,
                                             string key)
        {
           
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("EnumKey");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("EnumKey",
                                                        methodParams, null);
            System.String[] subKeys;
            subKeys = (string[])exitCode["sNames"];

            return new List<string>(subKeys);
            //var HKLM32 = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machinename, RegistryView.Registry32);
            //var HKLM64 = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machinename, RegistryView.Registry64);

            //RegistryKey OurKey32 = Microsoft.Win32.Registry.LocalMachine;
            //RegistryKey OurKey64 = Microsoft.Win32.Registry.LocalMachine;
            //OurKey32 = HKLM32.OpenSubKey(key, false);
            //OurKey64 = HKLM64.OpenSubKey(key, false);
            //var list = GetSubKeys(OurKey32);
            //var list2 = GetSubKeys(OurKey64);
            ////list.AddRange(list2);
            //return list;
        }
        private static List<string> GetSubKeys(RegistryKey SubKey)
        {
            List<string> mainlist = new List<string>();
            var count = SubKey.SubKeyCount;
            List<string> templist = new List<string>(SubKey.GetSubKeyNames());
            foreach (string sub in templist)
            {
                RegistryKey local = Microsoft.Win32.Registry.LocalMachine;
                mainlist.Add(sub);
                if (SubKey.SubKeyCount != 0)
                {
                    local = SubKey.OpenSubKey(sub, false);
                    //mainlist.AddRange(GetSubKeys(local)); // By recalling itselfit makes sure it get all the subkey names
                }
            }
            return mainlist;
        }
        public static string[] EnumerateValues(ManagementScope connectionScope,
                                               baseKey BaseKey,
                                               string key)
        {
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters("EnumValues");

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;

            ManagementBaseObject exitCode = registryTask.InvokeMethod("EnumValues",
                                                        methodParams, null);
            System.String[] values;
            values = (string[])exitCode["sNames"];
            return values;
        }
        public static string GetValue(ManagementScope connectionScope,
                                      baseKey BaseKey,
                                      string key,
                                      string valueName,
                                      valueType ValueType)
        {
            string typeOfValue = RegistryMethod.ConvertGetValueType(ValueType);
            string returnValue = string.Empty;
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters(typeOfValue);

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;
            methodParams["sValueName"] = valueName;

            ManagementBaseObject exitValue = registryTask.InvokeMethod(typeOfValue,
                                                                     methodParams, null);
            try
            {
                returnValue = exitValue["sValue"].ToString();
            }
            catch
            {
                try
                { //ToDo: fix this ASAP, nested try/catch, I mean come on dude!
                    returnValue = exitValue["uValue"].ToString();
                }
                catch (SystemException e)
                {
                    returnValue = e.Message.ToString();
                }
            }
            return returnValue;
        }
        public static void SetValue(ManagementScope connectionScope,
                                     baseKey BaseKey,
                                     string key,
                                     string valueName,
                                     string value,
                                     valueType ValueType)
        {
            string typeOfValue = RegistryMethod.ConvertSetValueType(ValueType);
            string returnValue = string.Empty;
            ManagementClass registryTask = new ManagementClass(connectionScope,
                           new ManagementPath("DEFAULT:StdRegProv"), new ObjectGetOptions());
            ManagementBaseObject methodParams = registryTask.GetMethodParameters(typeOfValue);

            methodParams["hDefKey"] = BaseKey;
            methodParams["sSubKeyName"] = key;
            methodParams["sValueName"] = valueName;
            methodParams["sValue"] = value;

            ManagementBaseObject exitValue = registryTask.InvokeMethod(typeOfValue,
                                                                     methodParams, null);
        }

        #region "helpers"
        public static string ConvertGetValueType(valueType entry)
        {
            string translation = string.Empty;
            switch (entry)
            {
                case valueType.BINARY:
                    translation = "GetBinaryValue";
                    break;
                case valueType.DWORD:
                    translation = "GetDWORDValue";
                    break;
                case valueType.EXPANDED_STRING:
                    translation = "GetExpandedStringValue";
                    break;
                case valueType.MULTI_STRING:
                    translation = "GetMultiStringValue";
                    break;
                case valueType.STRING:
                    translation = "GetStringValue";
                    break;
            }
            return translation;
        }
        public static string ConvertSetValueType(valueType entry)
        {
            string translation = string.Empty;
            switch (entry)
            {
                case valueType.BINARY:
                    translation = "SetBinaryValue";
                    break;
                case valueType.DWORD:
                    translation = "SetDWORDValue";
                    break;
                case valueType.EXPANDED_STRING:
                    translation = "SetExpandedStringValue";
                    break;
                case valueType.MULTI_STRING:
                    translation = "SetMultiStringValue";
                    break;
                case valueType.STRING:
                    translation = "SetStringValue";
                    break;
            }
            return translation;
        }
        #endregion

    }
}
