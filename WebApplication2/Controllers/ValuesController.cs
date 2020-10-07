using ClassLibrary2;
using ClassLibrary2.ServiceReference1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
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

        public string Get(string id)
        {
            try
            {
                string url = "https://b2g-disc.ppsr.gov.au/PpsrB2GService/2016/05/CollateralRegistrationSearch.svc/soap11";
                string urlMTOM = "https://b2g-disc.ppsr.gov.au/PpsrB2GService/2016/05/CollateralRegistrationSearch.svc/soap11mtom";
                string username = "MUS258", password = "TearOfSun1$";

                SearchBySerialNumberResponseMessage message = null;
                RequestSearchCertificateResponseMessage dsd = null;
                RetrieveSearchCertificateResponseMessage ee = null;

                var operationClient = PPSRService.CreatePPSRClient(url, username, password, UrlType.normal);
                var operationChannel = operationClient.ChannelFactory.CreateChannel();
                using (new OperationContextScope((IClientChannel)operationChannel))
                {
                    message = operationChannel.SearchBySerialNumber(new SearchBySerialNumberRequestMessage
                    {
                        TargetEnvironment = development,
                        SearchBySerialNumberRequest = new SearchBySerialNumberRequestType
                        {
                            CustomersRequestMessageId = Guid.NewGuid().ToString(),
                            SearchCriteria = new SearchBySerialNumberSearchCriteria
                            {
                                IncludeCurrent = true,
                                SerialNumberType = SerialNumberSearchCriteriaType.VIN,
                                SerialNumber = id//"JT752EP9100474959"//"JS1SP46A000504266"
                            }
                        }
                    });


                    // return JsonConvert.SerializeObject(message);
                }

                using (new OperationContextScope((IClientChannel)operationChannel))
                {
                    dsd = operationChannel.RequestSearchCertificate(new RequestSearchCertificateRequestMessage
                    {
                        TargetEnvironment = development,
                        RequestSearchCertificateRequest = new RequestSearchCertificateRequestType
                        {
                            CustomersRequestMessageId = Guid.NewGuid().ToString(),
                            SearchNumber = message.SearchBySerialNumberResponse.SearchSummary.SearchNumber,
                            RegistrationNumber = message.SearchBySerialNumberResponse.SearchResult.ResultDetails.Last().RegistrationDetail.RegistrationNumber,
                            ChangeNumber = message.SearchBySerialNumberResponse.SearchResult.ResultDetails.Last().RegistrationDetail.ChangeNumber
                        }
                    });


                    //sb.AppendLine(JsonConvert.SerializeObject(dsd));
                    //sb.AppendLine();
                    return JsonConvert.SerializeObject(dsd);
                }

            }
            catch (Exception ex)
            {
                return $"MESSAGE: {ex?.Message}  STACETRACE: {ex?.StackTrace}  SOURCE: {ex?.Source}  INNER EXCEPTION: {ex?.InnerException?.ToString()}";
            }
        }

        [HttpGet]
        [Route("download/{searchCertificateNumber}")]
        public IHttpActionResult Download(string searchCertificateNumber)
        {
            try
            {
                string urlMTOM = "https://b2g-disc.ppsr.gov.au/PpsrB2GService/2016/05/CollateralRegistrationSearch.svc/soap11mtom";
                string username = "MUS258", password = "TearOfSun1$";

                RetrieveSearchCertificateResponseMessage ee = null;

                //Create a new Client with new URL which uses MTOM
                var operationClient1 = PPSRService.CreatePPSRClient1(urlMTOM, username, password);
                var operationChannel1 = operationClient1.ChannelFactory.CreateChannel();
                using (new OperationContextScope((IClientChannel)operationChannel1))
                {
                    ee = operationChannel1.RetrieveSearchCertificate(new RetrieveSearchCertificateRequestMessage
                    {
                        TargetEnvironment = development,
                        RetrieveSearchCertificateRequest = new RetrieveSearchCertificateRequestType
                        {
                            CustomersRequestMessageId = Guid.NewGuid().ToString(),
                            SearchCertificateNumber = searchCertificateNumber,
                        }
                    });
                }

                var res = ee?.RetrieveSearchCertificateResponse;

                if (res?.SearchCertificateFile != null)
                {
                    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(new MemoryStream(res.SearchCertificateFile)),
                    };

                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = res.SearchCertificateFileName ?? "Test.pdf"
                    };

                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    return ResponseMessage(result);
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public string Get(int id)
        //{
        //    //ChangeB2GPasswordResponseMessage message = nul    l;
        //    try
        //    {
        //        string url = "https://b2g-disc.ppsr.gov.au/PpsrB2GService/2016/05/RegisterOperations.svc/soap11";

        //        string username = "MUS258", password = "TearOfSun1$";

        //        var operationClient = PPSRService.CreatePPSROperationClientProxy(url, username, password);
        //        var operationChannel = operationClient.ChannelFactory.CreateChannel();
        //        using (new OperationContextScope((IClientChannel)operationChannel))
        //        {
        //            //var message = operationChannel.ChangeB2GPassword(new ChangeB2GPasswordRequestMessage
        //            //{
        //            //    TargetEnvironment = development,
        //            //    ChangeB2GPasswordRequest = new ChangeB2GPasswordRequestType
        //            //    {
        //            //        Username = "MUS258",
        //            //        NewPassword = "TearOfSun1$1",
        //            //        CustomersRequestMessageId = Guid.NewGuid().ToString()
        //            //    }
        //            //});

        //            var message = operationChannel.Ping(new PingRequestMessage
        //            {
        //                TargetEnvironment = development,
        //                PingRequest = new PingRequestType
        //                {
        //                    CustomersRequestMessageId = Guid.NewGuid().ToString()
        //                }
        //            });


        //            return JsonConvert.SerializeObject(message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"MESSAGE: {ex?.Message}  STACETRACE: {ex?.StackTrace}  SOURCE: {ex?.Source}  INNER EXCEPTION: {ex?.InnerException?.ToString()}";
        //    }
        //}
    }
}
