using System;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text;

namespace Bidvalet.Shared
{
	public class SWAAuthentication
	{

		#region Private Variables
		private string _serverUrl = string.Empty; 
		#endregion

		#region Public Methods
		public string CheckCredential(string userId, string password)
		{
			try
			{ 
				//_serverUrl = "https://www27.swalife.com/webbid3pty/ThirdParty";
				_serverUrl = GlobalSettings.QATest ? SWAConstants.QASWAUrl : SWAConstants.SWAUrl;
				//_serverUrl = SWAConstants.SWAUrl;
				//Get precredential details
				string preCredentials = GetPreCredentials();

				//Get session credentials
				var sessioncredential = GetSessionCredentials(userId, password, preCredentials);

				return sessioncredential;
			}
			catch (Exception ex)
			{
				//downloadInfo.SessionCredentials =  ex.Message;
				return "Exception";

			}

		} 
		#endregion

		#region Private Methods

		/// <summary>
		/// Get PreCredentials string
		/// </summary>
		/// <returns></returns>
		private string GetPreCredentials()
		{
			try
			{
				ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serverUrl);
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Stream stream = response.GetResponseStream();
				StreamReader sr = new StreamReader(stream);
				return sr.ReadToEnd();
			}
			catch (Exception ex)
			{

				throw ex;
			}
		}



		/// <summary>
		/// Get session credential string
		/// </summary>
		/// <param name="preCredentials"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public string GetSessionCredentials( string username, string password,string preCredentials)
		{
			try
			{

                WBidWebClient webClient = new WBidWebClient();
				NameValueCollection formData = new NameValueCollection();
				formData["CREDENTIALS"] = preCredentials;
				formData["REQUEST"] = "LOGON";
				formData["UID"] = username;
				formData["PWD"] = password;
                
				byte[] responseBytes = webClient.UploadValues(_serverUrl, "POST", formData);
				return Encoding.UTF8.GetString(responseBytes);
			}
			catch (Exception ex)
			{

				throw ex;
			}

		}




        private class WBidWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 90000;
                return w;
            }
        }
		#endregion
	}
}

