using ClassLibrary2.CustomClasses;
using ClassLibrary2.ServiceReference1;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace ClassLibrary2
{
    public class PPSRService
    {
        public static CollateralRegistrationSearchService2016Client CreatePPSRClient(string url,
                                                                string username,
                                                                string password)
        {
            CustomBinding binding = new CustomBinding();
            var security = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            security.IncludeTimestamp = true;

            security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

            var encoding = new TextMessageEncodingBindingElement();
            encoding.MessageVersion = MessageVersion.Soap11;
            var transport = new HttpsTransportBindingElement();
            transport.MaxReceivedMessageSize = 2000000; // 20 megs

            binding.Elements.Add(security);
            binding.Elements.Add(encoding);
            binding.Elements.Add(transport);

            CollateralRegistrationSearchService2016Client client = new CollateralRegistrationSearchService2016Client(binding,
                new EndpointAddress(url));

            // to use full client credential with Nonce uncomment this code:
            // it looks like this might not be required - the service seems to work without it
            client.Endpoint.Behaviors.Remove<System.ServiceModel.Description.ClientCredentials>();
            client.Endpoint.Behaviors.Add(new CustomCredentials());

            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = password;

            return client;
        }

        public static RegisterOperationsService2016Client CreatePPSROperationClientProxy(string url,
                                                                string username,
                                                                string password)
        {
            if (string.IsNullOrEmpty(url))
                url = "https://b2g-disc.ppsr.gov.au/PpsrB2GService/2011/02/CollateralRegistration.svc/soap11";

            CustomBinding binding = new CustomBinding();

            var security = TransportSecurityBindingElement.CreateUserNameOverTransportBindingElement();
            security.IncludeTimestamp = false;
            security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

            var encoding = new TextMessageEncodingBindingElement();
            encoding.MessageVersion = MessageVersion.Soap11;

            var transport = new HttpsTransportBindingElement();
            transport.MaxReceivedMessageSize = 20000000; // 20 megs

            binding.Elements.Add(security);
            binding.Elements.Add(encoding);
            binding.Elements.Add(transport);

            RegisterOperationsService2016Client client = new RegisterOperationsService2016Client(binding,
                new EndpointAddress(url));

            // to use full client credential with Nonce uncomment this code:
            // it looks like this might not be required - the service seems to work without it
            // client.ChannelFactory.Endpoint.Behaviors.Add(new InspectorBehavior());
            client.ChannelFactory.Endpoint.Behaviors.Remove<System.ServiceModel.Description.ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new CustomCredentials());

            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = password;

            return client;
        }

        public static RegisterOperationsService2016Client CreatePPSROperationClient(string url,
                                                                   string username,
                                                                   string password)
        {
            CustomBinding binding = new CustomBinding();
            var security = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            security.IncludeTimestamp = false;
            security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

            var encoding = new TextMessageEncodingBindingElement();
            encoding.MessageVersion = MessageVersion.Soap11;

            var transport = new HttpsTransportBindingElement();
            transport.MaxReceivedMessageSize = 2000000; // 20 megs

            binding.Name = "RegisterOperationsSoap11";

            binding.Elements.Add(security);
            binding.Elements.Add(encoding);
            binding.Elements.Add(transport);

            RegisterOperationsService2016Client client = new RegisterOperationsService2016Client(binding,
                new EndpointAddress(url));

            // to use full client credential with Nonce uncomment this code:
            // it looks like this might not be required - the service seems to work without it
            client.Endpoint.Behaviors.Remove<System.ServiceModel.Description.ClientCredentials>();
            //client.Endpoint.Behaviors.Add(new CustomEndpointBehavior());
            client.Endpoint.Behaviors.Add(new CustomCredentials());

            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = password;

            return client;
        }
    }
}