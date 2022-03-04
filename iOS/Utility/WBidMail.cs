using Bidvalet.Model;
using System;
using System.Text;

namespace Bidvalet.iOS
{
	public class WBidMail
	{
		public WBidMail()
		{

		}
		//WBidDataDwonloadAuthServiceClient WBidDataDwonloadAuthServiceClientClient;
		public void SendCrashMail(string bidData)
		{
            MailInformation mailInfo = new MailInformation();
            mailInfo.Alias = GetmailAlias();
            mailInfo.ToAddress = " crash@wbidvalet.com";
            mailInfo.FromAddress = "admin@wbidvalet.com";
            mailInfo.MessageBody = bidData.Replace("\r\n", "<br/>");
            //mailInfo.MessageBody = getFormattedErrorMailContent(bidData);
            if (GlobalSettings.WBidINIContent != null)
            {
                if (GlobalSettings.UserInfo != null )
                {
                    mailInfo.EmployeeNumber = Convert.ToInt32(GlobalSettings.UserInfo.EmpNo);
                    mailInfo.UserAppEmail = GlobalSettings.UserInfo.Email;
                }
            }
            mailInfo.Subject = "WBidValet Error Log" + " (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " )";
            SendMail(mailInfo);
		}

		public void SendMailtoAdmin(string mailcontent, string fromId,string subject)
		{
//			MailInfo mailInfo = new MailInfo();
//			mailInfo.Alias = GetmailAlias();
//			mailInfo.ToAddress = "admin@wbidmax.com";
//			mailInfo.FromAddress = fromId;
//			mailInfo.MessageBody = mailcontent;
//			mailInfo.Subject = subject;
//			SendMail(mailInfo);
		}

		public void SendMailtoUser(string mailcontent, string toEmailId, string subject, byte[] attachement, string attachmentName)
		{
            MailInformation mailInfo = new MailInformation();
            mailInfo.Alias = "WBidValet Admin";
            mailInfo.ToAddress = toEmailId;
            mailInfo.FromAddress = "admin";
            mailInfo.MessageBody = mailcontent;
            mailInfo.Subject = subject;
           // mailInfo.Attachment1 = attachement;
            mailInfo.BaseAttachment1 = Convert.ToBase64String(attachement);
            mailInfo.Attachment1Name = attachmentName;
            mailInfo.AppNum = (int)AppNum.BidValet;
            SendMail(mailInfo);
		}


        private void SendMail(MailInformation mailInfo)
		{
			try
			{


                string result=  RestHelper.SendMailAll(mailInfo);
//				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
//				WBidDataDwonloadAuthServiceClientClient = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
//				WBidDataDwonloadAuthServiceClientClient.SendMailCompleted += dwonloadAuthServiceClient_SendMailCompleted;
//				WBidDataDwonloadAuthServiceClientClient.SendMailAsync(mailInfo);
			}
			catch (Exception ex)
			{

				throw ex;
			}
		}

		private string getFormattedErrorMailContent(string body)
		{
			var sb = new StringBuilder();
//			sb.Append("<table  border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
//			sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">Hi Admin ,</td></tr>");
//			sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">&nbsp;</td></tr>");
//			sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px; padding: 0 0 10px 0;\">");
//			sb.Append(body);
//			sb.Append("<br /><br /> Data  :" + GetApplicationbiddata());
//			if (GlobalSettings.WbidUserContent.UserInformation != null)
//			{
//				sb.Append("<br/>"+"Base            :" + GlobalSettings.WbidUserContent.UserInformation.Domicile );
//				sb.Append("<br/>"+"Seat            :" + GlobalSettings.WbidUserContent.UserInformation.Position);
//				sb.Append("<br/>"+"Employee Number :" + GlobalSettings.WbidUserContent.UserInformation.EmpNo);
//				sb.Append("<br/>" + "Email  :" + GlobalSettings.WbidUserContent.UserInformation.Email);
//			}
//
//
//			sb.Append("<br /><br /> OS  :" + "Ipad OS");
//
//
//			sb.Append("</td></tr>");
//
//			sb.Append("</table>");
//			sb.Append("<table width=\"250\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
//			sb.Append("<tr><td></td><td></td><td></td></tr><tr><td colspan=\"3\">&nbsp;</td></tr><tr><td colspan=\"3\">&nbsp;</td></tr>");
//			sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;padding:15px 0 0 0;\"><br/><br/>Sincerely ,</td></tr>");
//			sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;padding:15px 0 0 0;\">");
//			sb.Append(GetmailAlias());
//			sb.Append("</td></tr>");
//			sb.Append("</table>");
			return sb.ToString();
		}
		/// <summary>
		/// PURPOSE : Set Application Title 
		/// </summary>
		private string GetApplicationbiddata()
		{
			try
			{
//				string domicile = GlobalSettings.CurrentBidDetails.Domicile ?? string.Empty;
//				string position = GlobalSettings.CurrentBidDetails.Postion ?? string.Empty;
//				System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
//				string strMonthName = mfi.GetMonthName(GlobalSettings.CurrentBidDetails.Month).ToString();
//				string round = GlobalSettings.CurrentBidDetails.Round == "M" ? "Monthly" : "2nd Round";
//				return domicile + "/" + position + "/" + " " + round + "  Line for " + strMonthName + " " + GlobalSettings.CurrentBidDetails.Year;
			return string.Empty;
			}
			catch (Exception)
			{
				return string.Empty;
			}

		}
		private string GetmailAlias()
		{
			try
			{
                if (GlobalSettings.UserInfo != null)
                {
                    return GlobalSettings.UserInfo.FirstName + " " + GlobalSettings.UserInfo.LastName + "-" + GlobalSettings.UserInfo.EmpNo + GlobalSettings.ApplicationName;
                }
                else
					return string.Empty;
			}
			catch (Exception ex)
			{
				return string.Empty;
			}

		}
//		void dwonloadAuthServiceClient_SendMailCompleted(object sender, SendMailCompletedEventArgs e)
//		{
//			if (e.Result != null)
//			{
//
//			}
//		}
	}
}

