using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LicenseControllerWs.ParameterInspector
{
    public class ClientInfo
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ClientInfo() { }





        /// <summary>
        /// Name of the machine, which generates a request for a WCF/Remoting service.
        /// </summary>
        public string MachineName
        {
            get;
            set;
        }

        /// <summary>
        /// Time, when the given request was generated.
        /// </summary>
        public DateTime TimeStamp
        {
            get;
            set;
        }

        /// <summary>
        /// The action or the service method for which, the request is generated.
        /// </summary>
        public string Action
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the client application consuming the message.
        /// </summary>
        public string ApplicationName
        {
            get;
            set;
        }


        /// <summary>
        /// Machine name where remoting/WCF service is running.
        /// </summary>
        public string ServerName
        {
            get;
            set;
        }

        /// <summary>
        /// Timestamp immediately after request is received at the WCF/Remoting server
        /// </summary>
        public DateTime ServerTimeStamp
        {
            get;
            set;
        }

        /// <summary>
        /// Timestamp at the WCF/Remoting server immediately before sending the response back to client
        /// </summary>
        public DateTime ServerEndTimeStamp
        {
            get;
            set;
        }

        /// <summary>
        /// Server process name at the WCF/Remoting server
        /// </summary>
        public string ServerProcessName
        {
            get;
            set;
        }

        /// <summary>
        /// WCF/Remoting request XML collected at serverside.
        /// </summary>
        public string Request
        {
            get;
            set;
        }

        /// <summary>
        /// WCF/Remoting response XML collected at serverside.
        /// </summary>
        public string Response
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates if there is an error at serverside.
        /// </summary>
        public bool IsError
        {
            get;
            set;
        }


        /// <summary>
        /// Assembly name of the of request type.
        /// </summary>
        public string AssemblyName
        {
            get;
            set;
        }

        /// <summary>
        /// Type name of the of request type including namespace.
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }


        /// <summary>
        /// Communication Platform; Remoting or Wcf marked at the Server.
        /// </summary>
        public string Platform
        {
            get;
            set;
        }
    }
}