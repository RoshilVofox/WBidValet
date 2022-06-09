using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;

namespace Bidvalet.iOS.Utility
{
    public class ServiceUtility
    {
		public static EndpointAddress EndPoint =  new EndpointAddress(GlobalSettings.WBidAuthenticationServiceUrl) ;
		public static readonly EndpointAddress PushEndPoint = new EndpointAddress("http://push.wbidmax.com:8007/WBidPushSerivce.svc");
		public static StreamReader GetRestData(string serviceName, string jsonString)
		{
			string url = GlobalSettings.DataDownloadAuthenticationUrl + serviceName;
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
		public static BasicHttpBinding CreateBasicHttp()
		{
			BasicHttpBinding binding = new BasicHttpBinding
			{
				Name = "basicHttpBinding",
				MaxBufferSize = 2147483647,
				MaxReceivedMessageSize = 2147483647
			};
			TimeSpan timeout = new TimeSpan(0, 0, 30);
			binding.SendTimeout = timeout;
			binding.OpenTimeout = timeout;
			binding.ReceiveTimeout = timeout;
			EndPoint = new EndpointAddress(GlobalSettings.WBidAuthenticationServiceUrl);
			return binding;
		}
		public static string JsonSerializer<T>(T t)
		{
			string jsonString = string.Empty;
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
			MemoryStream ms = new MemoryStream();
			ser.WriteObject(ms, t);
			jsonString = Encoding.UTF8.GetString(ms.ToArray());
			return jsonString;
		}
	}
}
