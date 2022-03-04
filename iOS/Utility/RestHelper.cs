using System;
using System.Net;
using Bidvalet.Business;
using System.IO;
using System.Text;
using Bidvalet.Model;


namespace Bidvalet.iOS
{
    public class RestHelper
    {

        public static T GetResponse<T>(string url)
        {
            T temp = default(T);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 30000;
                request.ContentType = "application/json";
                request.Method = "GET";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var stream = response.GetResponseStream();
                        var reader = new StreamReader(stream);
                        temp = SerializeHelper.ConvertJSonStringToObject<T>(reader.ReadToEnd());
                        stream.Dispose();
                        stream.Close();
                    }
                };
                request = null;
            }
            catch (Exception ex)
            {
                // temp = null;
                //var aa = ex.Message.ToString();
            }
            return temp;
        }

        public static CustomServiceResponse UpdateAllUserDetails(ServerUserInformation sUersinformation)
        {
            CustomServiceResponse customServiceResponse = new CustomServiceResponse();
            try
            {
                string url = GlobalSettings.WBidAuthenticationServiceUrl + "/UpdateAllUserDetails";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var data = SerializeHelper.JsonObjectToStringSerializer<ServerUserInformation>(sUersinformation);
                var bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                request.GetRequestStream().Write(bytes, 0, bytes.Length);
                request.Timeout = 60000;
                //Response
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();

                    var reader = new StreamReader(stream);

                    customServiceResponse = SerializeHelper.ConvertJSonStringToObject<CustomServiceResponse>(reader.ReadToEnd());

                }
            }
            catch (Exception ex)
            {
                customServiceResponse = null;
            }

            return customServiceResponse;
        }

        public static CustomServiceResponse CreateAllUser(ServerUserInformation sUersinformation)
        {
            CustomServiceResponse customServiceResponse = new CustomServiceResponse();
            try
            {
                string url = GlobalSettings.WBidAuthenticationServiceUrl + "/CreateAllUser";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var data = SerializeHelper.JsonObjectToStringSerializer<ServerUserInformation>(sUersinformation);
                var bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                request.GetRequestStream().Write(bytes, 0, bytes.Length);
                request.Timeout = 60000;
                //Response
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();

                    var reader = new StreamReader(stream);

                    customServiceResponse = SerializeHelper.ConvertJSonStringToObject<CustomServiceResponse>(reader.ReadToEnd());

                }
            }
            catch (Exception ex)
            {
                customServiceResponse = null;
            }

            return customServiceResponse;
        }

        public static AuthServiceResponseModel CheckWBidAuthentication(ClientRequestModel clientRequestModel)
        {
            Guid token = Guid.NewGuid();

            clientRequestModel.Token = token;
            // bool status = false;
            AuthServiceResponseModel responseModel = new AuthServiceResponseModel();
            try
            {

                var url = GlobalSettings.WBidAuthenticationServiceUrl + "/GetAllAuthentication";
                var data = string.Empty;
                //var
                var objClient = clientRequestModel;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                data = SerializeHelper.JsonObjectToStringSerializer<ClientRequestModel>(objClient);
                var bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                request.GetRequestStream().Write(bytes, 0, bytes.Length);
                request.Timeout = 60000;
                //Response
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();
                    var reader = new StreamReader(stream);

                    responseModel = SerializeHelper.ConvertJSonStringToObject<AuthServiceResponseModel>(reader.ReadToEnd());
                }
            }
            catch (WebException ex)
            {

                if (ex.Status == WebExceptionStatus.Timeout)
                {

                    responseModel.Type = "TimeOut";
                    try
                    {
                        var url = GlobalSettings.WBidAuthenticationServiceUrl + "/LogAllTimeOutDetails/" + token.ToString() + "/" + (int)AppNum.BidValet;
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.Timeout = 40000;
                        request.ContentType = "application/json";
                        request.Method = "GET";
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                        }


                    }
                    catch (Exception exx)
                    {
                        //responseModel = null;
                    }
                }
                else
                {
                    responseModel = null;
                }

                //responseModel = null;
            }
            catch (Exception exOther)
            {
                responseModel = null;

            }
            return responseModel;
        }

        public static void LogOperation(LogInformation logInformation)
        {

            try
            {
                string url = GlobalSettings.WBidAuthenticationServiceUrl + "/LogOperationAll";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var data = SerializeHelper.JsonObjectToStringSerializer<LogInformation>(logInformation);
                var bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                request.GetRequestStream().Write(bytes, 0, bytes.Length);
                request.Timeout = 60000;
                //Response
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //var stream = response.GetResponseStream ();

                    //var reader = new StreamReader (stream);

                    //customServiceResponse = SerializeHelper.ConvertJSonStringToObject<CustomServiceResponse>(reader.ReadToEnd());

                }
            }
            catch (Exception ex)
            {

            }


        }

        public static bool LogOfflineEvents(OffLineHelper offLineHelper)
        {

            try
            {
                string url = GlobalSettings.WBidAuthenticationServiceUrl + "/OfflineEventsAll";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var data = SerializeHelper.JsonObjectToStringSerializer<OffLineHelper>(offLineHelper);


                var bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                request.GetRequestStream().Write(bytes, 0, bytes.Length);
                request.Timeout = 60000;
                //Response
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {


                    var stream = response.GetResponseStream();

                    var reader = new StreamReader(stream);

                    //customServiceResponse = SerializeHelper.ConvertJSonStringToObject<CustomServiceResponse>(reader.ReadToEnd());
                    return true;
                }
            }
            catch (Exception ex)
            {

            }

            return false;

        }

        public static string SendMailAll(MailInformation mailInformation)
        {

            string result = string.Empty;
            try
            {




                string url = GlobalSettings.WBidAuthenticationServiceUrl + "/SendMailAll";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var data = SerializeHelper.JsonObjectToStringSerializer<MailInformation>(mailInformation);
                var bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                try
                {
                    request.GetRequestStream().Write(bytes, 0, bytes.Length);
                }
                catch (Exception ex)
                {
                    string aas = ex.Message;
                }
                request.Timeout = 60000;
                //Response
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();

                    var reader = new StreamReader(stream);

                    result = reader.ReadToEnd();
                    //customServiceResponse = SerializeHelper.ConvertJSonStringToObject<CustomServiceResponse>(reader.ReadToEnd());

                }
            }
            catch (Exception ex)
            {

            }

            return string.Empty;

        }

        public static ServerUserInformation UpdateAllPaidUntilDate(PaymentUpdateModel paymentUpdateModel)
        {
            // bool status = false;
            ServerUserInformation responseModel =null;
            try
            {

                var url = GlobalSettings.WBidAuthenticationServiceUrl + "/UpdateAllPaidUntilDate";


                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var data = SerializeHelper.JsonObjectToStringSerializer<PaymentUpdateModel>(paymentUpdateModel);
                var bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                request.GetRequestStream().Write(bytes, 0, bytes.Length);
                request.Timeout = 60000;
                //Response
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();
                    var reader = new StreamReader(stream);

                    responseModel = SerializeHelper.ConvertJSonStringToObject<ServerUserInformation>(reader.ReadToEnd());
                    if (responseModel.TopSubscriptionLine == "Error")
                        responseModel = null;
                }
            }
            catch (Exception ex)
            {
            }
            return responseModel;
        }

        public static string SyncState(StateSync stateSync)
        {
            try
            {


                string url = GlobalSettings.synchServiceUrl + "SaveWBidStateToServer/";

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var data = SerializeHelper.JsonObjectToStringSerializerMethod<StateSync>(stateSync);
                var bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                request.GetRequestStream().Write(bytes, 0, bytes.Length);
                request.Timeout = 60000;
                //Response
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();
                    var reader = new StreamReader(stream);
                    string result = reader.ReadToEnd();

                    return result.Trim('"');

                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }


        public static HistoricalFileInfo DownloadHistoricalData(HistoricalBidDetails historicalBidDetails, bool is737)
        {

            HistoricalFileInfo responseModel = null;
            try
            {
                string url = GlobalSettings.WBidAuthenticationServiceUrl;
                if (is737)
                {

                    url = url + "/DownloadHistoricalDataAll/";
                }
                else
                {
                    url = url + "/DownloadHistoricalBidLineAll/";
                }


                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                var data = SerializeHelper.JsonObjectToStringSerializerMethod<HistoricalBidDetails>(historicalBidDetails);
                var bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                request.GetRequestStream().Write(bytes, 0, bytes.Length);
                request.Timeout = 60000;
                //Response
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();
                    var reader = new StreamReader(stream);
                    responseModel = SerializeHelper.ConvertJSonStringToObject<HistoricalFileInfo>(reader.ReadToEnd());

                }
            }
            catch (Exception ex)
            {

            }
            return responseModel;
        }
        public static StreamReader GetRestData(string serviceNameandParameter)
        {
            string url = GlobalSettings.WBidAuthenticationServiceUrl + serviceNameandParameter;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 30000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            var reader = new StreamReader(stream);
            return reader;

        }
        public static StreamReader GetRestData(string serviceName, string jsonString)
        {
            string url = GlobalSettings.WBidAuthenticationServiceUrl + serviceName;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var bytes = Encoding.UTF8.GetBytes(jsonString);
            request.ContentLength = bytes.Length;
            request.GetRequestStream().Write(bytes, 0, bytes.Length);
            //Response
            var response = (HttpWebResponse)request.GetResponse();
            var streamoutput = response.GetResponseStream();
            var readeroutput = new StreamReader(streamoutput);
            return readeroutput;

            // streamoutput.Dispose();
            // readeroutput.Dispose();
        }

       


    }


}

