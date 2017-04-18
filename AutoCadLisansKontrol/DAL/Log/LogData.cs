using CheckLicense.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace CheckLicense.Log
{
    public class LogData
    {
        public string AppName { get; set; }
        public string Id { get; set; }
        public string ReqXml { get; set; }
        public string ResXml { get; set; }
        public string Method { get; set; }
        public decimal ExpId { get; set; }
        public decimal LogId { get; set; }
        public decimal LevelId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Ip { get; set; }
        public string Host { get; set; }
        public LogDataType LogDataType { get; set; }
        public int FirmId { get; set; }
        public int OperationId { get; set; }
        public LogDataState State { get; set; }

        public int ComputerId { get; set; }

    }
    public enum LogDataType
    {

        InitiliazeProcess = 0,
        InitiliazeItemOfProcess = 1,
        UpdateItemOfProcess = 2,
        UpdateProcess = 3
    }
    public enum LogDataState
    {
        Success = 1,
        Fail = 0
    }


}