using ClassLibrary2;
using ClassLibrary2.ServiceReference1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.Http;

namespace WebApplication2.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        string development = "Discovery";

        private readonly Random _random = new Random();

        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            //ChangeB2GPasswordResponseMessage message = null;
            try
            {
                string url = "https://b2g-disc.ppsr.gov.au/PpsrB2GService/2016/05/RegisterOperations.svc/soap11";
              
                string username = "MUS258", password = "TearOfSun1$";
               
                var operationClient = PPSRService.CreatePPSROperationClientProxy(url, username, password);
                var operationChannel = operationClient.ChannelFactory.CreateChannel();
                using (new OperationContextScope((IClientChannel)operationChannel))
                {
                    var tt = operationClient.Endpoint;
                    //var message = operationChannel.ChangeB2GPassword(new ChangeB2GPasswordRequestMessage
                    //{
                    //    TargetEnvironment = development,
                    //    ChangeB2GPasswordRequest = new ChangeB2GPasswordRequestType
                    //    {
                    //        Username = "MUS258",
                    //        NewPassword = "TearOfSun1$1",
                    //        CustomersRequestMessageId = Guid.NewGuid().ToString()
                    //    }
                    //});

                    var message = operationChannel.Ping(new PingRequestMessage
                    {
                        TargetEnvironment = development,
                        PingRequest = new PingRequestType
                        {
                            CustomersRequestMessageId = Guid.NewGuid().ToString()
                        }
                    });


                    return JsonConvert.SerializeObject(message);
                }
            }
            catch (Exception ex)
            {
                return $"MESSAGE: {ex?.Message}  STACETRACE: {ex?.StackTrace}  SOURCE: {ex?.Source}  INNER EXCEPTION: {ex?.InnerException?.ToString()}";
            }
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
           
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
