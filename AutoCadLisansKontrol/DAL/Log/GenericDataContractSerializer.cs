using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AutoCadWs
{
    public class GenericDataSerializer<T>
    {
        private static string _message;
        [XmlElement("CDataElement")]
        public static XmlCDataSection Message
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(_message);
            }
            set
            {
                _message = value.Value;
            }
        }
        /// <summary>
        /// Serializer
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject(T obj)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var stringBuilder = new StringBuilder();
                var settings = new XmlWriterSettings { OmitXmlDeclaration = true };
                var stringWriter = XmlWriter.Create(stringBuilder, settings);
                var xnameSpace = new XmlSerializerNamespaces();
                xnameSpace.Add("", "");
                xmlSerializer.Serialize(stringWriter, obj, xnameSpace);
                return stringBuilder.ToString();
              
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to serialize data " + typeof(T) + " object to xml string:", exception);
            }
        }

        /// <summary>
        /// DeserializeXml
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static T DeserializeXml(string xml, string root)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));
                return (T)xmlSerializer.Deserialize(new StringReader(xml));
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to deserialize xml string to data contract object:", exception);
            }
        }
    }

}