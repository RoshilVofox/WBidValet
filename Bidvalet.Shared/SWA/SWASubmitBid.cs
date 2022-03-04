using Bidvalet.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bidvalet.Shared.SWA
{
    public class SWASubmitBid
    {
        // frank add buddy bid test
        // ***** only frank can use this feature for now  *****

        private bool buddyBidTest =   GlobalSettings.QATest;
   
        #region Public Methods
        /// <summary>
        /// Submit the Bid Details to the SWA Server
        /// </summary>
        /// <param name="submitBid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public string SubmitBid(SubmitBid submitBid, string username, string password, string sessioncredential)
        {
            


            CustomWebClient webClient1 = new CustomWebClient();
            webClient1.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            //set the formdata values
            NameValueCollection formData = new NameValueCollection();
            formData["REQUEST"] = "UPLOAD_BID";
            formData["BASE"] = submitBid.Base;
            formData["BID"] = submitBid.Bid;
            formData["BIDDER"] = submitBid.Bidder;
            formData["BIDROUND"] = submitBid.BidRound;
            //formData["CLOSEDBIDSIM"] = "N";
            formData["CREDENTIALS"] = sessioncredential;
            formData["PACKETID"] = submitBid.PacketId;
            formData["SEAT"] = submitBid.Seat;
            formData["VENDOR"] = "WBidValet";
            // should always be null for CP and FA
            if (submitBid.Pilot1 != null) formData["PILOT1"] = submitBid.Pilot1;
            if (submitBid.Pilot2 != null) formData["PILOT2"] = submitBid.Pilot2;
            if (submitBid.Pilot3 != null) formData["PILOT3"] = submitBid.Pilot3;
            // should always be null for CP and FO
            if (submitBid.Buddy1 != null) formData["BUDDY1"] = submitBid.Buddy1;
            if (submitBid.Buddy2 != null) formData["BUDDY2"] = submitBid.Buddy2;

            byte[] responsebyte = null;

            // frank add for buddy bid testing
            string url = buddyBidTest ? SWAConstants.QASWAUrl : SWAConstants.SWAUrl;
            try
            {
                responsebyte = webClient1.UploadValues(url, "POST", formData);
            }
            catch (Exception ex)
            {
                return "server failure";
            }
            var response = Encoding.Convert(Encoding.GetEncoding("iso-8859-1"), Encoding.UTF8, responsebyte);
            string status = Encoding.UTF8.GetString(response, 0, response.Length);
            return status;



            //StringBuilder strr = new StringBuilder();
            //strr.AppendLine("22028");
            //strr.AppendLine(submitBid.Bid.Replace(",", "\n"));
            //strr.AppendLine("*\nSUBMITTED BY: [e22028]     22028    " + DateTime.Now.ToLongDateString());

            //return strr.ToString();


        }
        private class CustomWebClient : WebClient
        {
            public CustomWebClient()
            {

            }

            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 60000;
                return w;
            }
        }
        #endregion

    }
}
