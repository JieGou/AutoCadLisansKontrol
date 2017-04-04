using AutoCadWs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace CheckLicense.Model.Response
{
    [Serializable]
    public class CImportMessage
    {
        public const int Success=1;
        public const int Fail = 0;
        public CImportMessage()
        {
        }

        [XmlElement(ElementName = "result")]
        public Result Result { get; set; }
        [XmlElement(ElementName = "CError",IsNullable = true)]
        public CError CError { get; set; }

        public static CImportMessage DeserializeMessage(string message)
        {
            var doc = new XmlDocument();
            doc.LoadXml(message);
            XmlNodeList xnList = doc.SelectNodes("/CImportMessage");
            var cimportMessage = GenericDataSerializer<CImportMessage>.DeserializeXml(xnList[0].OuterXml, "CImportMessage");
            return cimportMessage;
        }
      
    }
    [Serializable]
    public class Result
    {
        [XmlAttribute("success")]
        public string Success { get; set; }        
        [XmlElement(ElementName = "entity")]
        public Entity[] Entity { get; set; }
        [XmlElement(ElementName = "message")]
        public string Message { get; set; }
    }
    [Serializable]
    public class Entity
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("operation")]
        public string Operation { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
    }
    [Serializable]
    public class CError
    {
        [XmlAttribute("severity")]
        public string Severity { get; set; }
        [XmlAttribute("token")]
        public string Token { get; set; }
        [XmlElement(ElementName = "message")]
        public string Message { get; set; }
        [XmlElement(ElementName = "parameter")]
        public Parameter parameter { get; set; }
    }
    [Serializable]
    public class Parameter
    { }
}