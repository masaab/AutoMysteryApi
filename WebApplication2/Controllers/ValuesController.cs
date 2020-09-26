using ClassLibrary2;
using ClassLibrary2.ServiceReference1;
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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            ChangeB2GPasswordResponseMessage message = null;
            try
            {
                //string url= https://b2g-disc.ppsr.gov.au/PpsrB2GService/2016/05/CollateralRegistrationSearch.svc/soap11
                string url = "https://b2g-disc.ppsr.gov.au/PpsrB2GService/2016/05/RegisterOperations.svc/soap11";
                string username = "MUS258", password = "YFU7C2EALZ7V";
               
                var operationClient = PPSRService.CreatePPSROperationClient(url, username, password);
                var operationChannel = operationClient.ChannelFactory.CreateChannel();
                using (new OperationContextScope((IClientChannel)operationChannel))
                {
                    var tt = operationClient.Endpoint;
                    message = operationChannel.ChangeB2GPassword(new ChangeB2GPasswordRequestMessage
                    {
                        TargetEnvironment = development,
                        ChangeB2GPasswordRequest = new ChangeB2GPasswordRequestType
                        {
                            Username = "MUS258",
                            NewPassword = "TEAROFSUN"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return $"MESSAGE: {ex.Message}  STACETRACE: {ex.StackTrace}  SOURCE: {ex.Source}  INNER EXCEPTION: {ex.InnerException}";
            }
            return "value";
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
