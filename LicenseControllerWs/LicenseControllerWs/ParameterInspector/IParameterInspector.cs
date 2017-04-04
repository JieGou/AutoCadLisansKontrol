using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using CheckLicense.Log;
using CheckLicense.Model.Response;
using ProtectionConnLib_4.DataAccessModel;
using System.Net;
using System.Configuration;

namespace LicenseControllerWs.ParameterInspector
{
    public class IncomingMessageLogger : IDispatchMessageInspector
    {
        private int _levelId;
        protected string Id;
        readonly ILog _logger = LoggerFactory.CreateLogger(DatabaseType.MsSql);
        public object AfterReceiveRequest(ref Message request,
         System.ServiceModel.IClientChannel channel,
         System.ServiceModel.InstanceContext instanceContext)
        {
            
            var guidId = Guid.NewGuid().ToString();
            HttpContext.Current.Items["Requestid"] = guidId;
            try
            {
                var action = OperationContext.Current.IncomingMessageHeaders.Action;
                if (action == null) return null;
                action = action.Split('/').ToList().Last();
                var host = HttpContext.Current.Request.Url.ToString();
                var sourceIp = GetVisitorIpAddress();
                var starttime = DateTime.Now;
                HttpContext.Current.Items["StartTime"] = starttime;
                _logger.InitiliazeProcess(new LogData() { AppName = ConfigurationManager.AppSettings["AppName"], Id = guidId, StartTime = starttime, Ip = sourceIp, Host = host, Method = action });
                _levelId = (int)_logger.InitiliazeItemOfProcess(new LogData() { ReqXml = MessageToString(ref request), Id = guidId, Method = action, StartTime = DateTime.Now });
                HttpContext.Current.Items["levelid"] = _levelId;
                return null;

            }
            catch (Exception ex)
            {
                var levelid = Convert.ToDecimal(HttpContext.Current.Items["levelid"]);
             
                return null;
            }

        }
        public static string GetVisitorIpAddress(bool getLan = false)
        {
            string visitorIpAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (String.IsNullOrEmpty(visitorIpAddress))
                visitorIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(visitorIpAddress))
                visitorIpAddress = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(visitorIpAddress) || visitorIpAddress.Trim() == "::1")
            {
                getLan = true;
                visitorIpAddress = string.Empty;
            }

            if (getLan && string.IsNullOrEmpty(visitorIpAddress))
            {
                //This is for Local(LAN) Connected ID Address
                string stringHostName = Dns.GetHostName();
                //Get Ip Host Entry
                IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                //Get Ip Address From The Ip Host Entry Address List
                IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                try
                {
                    visitorIpAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
                }
                catch
                {
                    try
                    {
                        visitorIpAddress = arrIpAddress[0].ToString();
                    }
                    catch
                    {
                        try
                        {
                            arrIpAddress = Dns.GetHostAddresses(stringHostName);
                            visitorIpAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            visitorIpAddress = "127.0.0.1";
                        }
                    }
                }

            }


            return visitorIpAddress;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            string requestid = "";
            
            try
            {
                requestid = HttpContext.Current.Items["Requestid"].ToString();
                var action = OperationContext.Current.IncomingMessageHeaders.Action;
                if (action == null) return;

                _logger.UpdateItemOfProcess(new LogData()
                {
                    Id = requestid,
                    ResXml = MessageToString(ref reply),
                    LevelId = _levelId,
                    EndTime = DateTime.Now
                });
                _logger.UpdateProcess(new LogData() { Id = requestid, EndTime = DateTime.Now });
            }
            catch (System.ObjectDisposedException ex)
            {
                return;
            }
            catch (Exception ex)
            {
                var levelid = Convert.ToDecimal(HttpContext.Current.Items["levelid"]);
            }
        }
        private string MessageToString(ref Message message)
        {
            WebContentFormat messageFormat = this.GetMessageContentFormat(message);
            MemoryStream ms = new MemoryStream();
            XmlDictionaryWriter writer = null;
            switch (messageFormat)
            {
                case WebContentFormat.Default:
                case WebContentFormat.Xml:
                    writer = XmlDictionaryWriter.CreateTextWriter(ms);
                    break;
                case WebContentFormat.Json:
                    writer = JsonReaderWriterFactory.CreateJsonWriter(ms);
                    break;
                case WebContentFormat.Raw:
                    // special case for raw, easier implemented separately
                    return this.ReadRawBody(ref message);
            }

            message.WriteMessage(writer);
            writer.Flush();
            string messageBody = Encoding.UTF8.GetString(ms.ToArray());

            // Here would be a good place to change the message body, if so desired.

            // now that the message was read, it needs to be recreated.
            ms.Position = 0;

            // if the message body was modified, needs to reencode it, as show below
            // ms = new MemoryStream(Encoding.UTF8.GetBytes(messageBody));

            XmlDictionaryReader reader;
            if (messageFormat == WebContentFormat.Json)
            {
                reader = JsonReaderWriterFactory.CreateJsonReader(ms, XmlDictionaryReaderQuotas.Max);
            }
            else
            {
                reader = XmlDictionaryReader.CreateTextReader(ms, XmlDictionaryReaderQuotas.Max);
            }

            Message newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            message = newMessage;

            return messageBody;
        }
        private WebContentFormat GetMessageContentFormat(Message message)
        {
            WebContentFormat format = WebContentFormat.Default;
            if (message.Properties.ContainsKey(WebBodyFormatMessageProperty.Name))
            {
                WebBodyFormatMessageProperty bodyFormat;
                bodyFormat = (WebBodyFormatMessageProperty)message.Properties[WebBodyFormatMessageProperty.Name];
                format = bodyFormat.Format;
            }

            return format;
        }
        private string ReadRawBody(ref Message message)
        {
            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement("Binary");
            byte[] bodyBytes = bodyReader.ReadContentAsBase64();
            string messageBody = Encoding.UTF8.GetString(bodyBytes);

            // Now to recreate the message
            MemoryStream ms = new MemoryStream();
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(ms);
            writer.WriteStartElement("Binary");
            writer.WriteBase64(bodyBytes, 0, bodyBytes.Length);
            writer.WriteEndElement();
            writer.Flush();
            ms.Position = 0;
            XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max);
            Message newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            message = newMessage;

            return messageBody;
        }
    }
}