using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace ClassLibrary2
{
    public class SoapAuthHeader : MessageHeader
    {
        private readonly string _password, _username, _nonce, _createdDate;
        public SoapAuthHeader(string id, string username, string password, string nonce, string created)
        {
            _password = password;
            _username = username;
            _nonce = nonce;
            _createdDate = created;
            this.Id = id;
        }

        public string Id { get; set; }

        public override string Name => "Security";

        public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";


        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("soap", "Header", "http://www.w3.org/2003/05/soap-envelope");
            writer.WriteStartElement("wsse", Name, Namespace);
            writer.WriteAttributeString("soap:mustUnderstand", "1");
            writer.WriteXmlnsAttribute("wsse", Namespace);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("wsse", "UsernameToken", Namespace);
            writer.WriteAttributeString("wsu:Id", "UsernameToken-2");
            writer.WriteAttributeString("xmlns:wsu", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");

            writer.WriteStartElement("wsse", "Username", Namespace);
            writer.WriteValue(_username);
            writer.WriteEndElement();

            writer.WriteStartElement("wsse", "Password", Namespace);
            writer.WriteAttributeString("Type", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText");
            writer.WriteValue(_password);
            writer.WriteEndElement();

            writer.WriteStartElement("wsse", "Nonce", Namespace);
            writer.WriteAttributeString("EncodingType", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
            writer.WriteValue(_nonce);
            writer.WriteEndElement();

            writer.WriteStartElement("wsu:Created");
            writer.WriteValue(_createdDate);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }

    public class ClientMessageInspector : IClientMessageInspector
    {
        string username = "MUS258";
        string password = "YFU7C2EALZ7V";
        string development = "Discovery";
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var nonce = getNonce();
            //DateTime.Now.ToString("o");
            var datetime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");//2009-11-02T10:22:42.866Z
            string nonceToSend = Convert.ToBase64String(Encoding.UTF8.GetBytes(nonce));

            SoapAuthHeader header = new SoapAuthHeader(string.Empty, username, password, nonceToSend, datetime);
            request.Headers.RemoveAt(0);
            request.Headers.Add(header);
            return request;
        }
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        protected string getNonce()
        {
            string phrase = Guid.NewGuid().ToString();
            return phrase;

        }

        protected string GetSHA1String(string phrase)
        {
            SHA1CryptoServiceProvider sha1Hasher = new SHA1CryptoServiceProvider();
            byte[] hashedDataBytes = sha1Hasher.ComputeHash(Encoding.UTF8.GetBytes(phrase));
            string test = Convert.ToString(hashedDataBytes);
            return Convert.ToBase64String(hashedDataBytes);
        }
    }
    public class CustomEndpointBehavior : IEndpointBehavior
    {
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new ClientMessageInspector());
        }
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
