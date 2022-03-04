﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bidvalet.Model;
using Bidvalet.iOS;
namespace Bidvalet.Shared
{
	public class DownloadAward
	{
		public DownloadAward()
		{
		}
		private bool buddyBidTest = false;
		/// <summary>
		/// PURPOSE : Download Award Details
		/// </summary>
		/// <param name="downloadInfo"></param>
		/// <returns></returns>
		public List<DownloadedFileInfo> DownloadAwardDetails(DownloadInfo downloadInfo)
		{

			try
			{
				List<DownloadedFileInfo> lstDownloadedFiles = null;
				string fileName = string.Empty;
				string fileNametoSave = string.Empty;

				lstDownloadedFiles = DownloadFiles(downloadInfo);

				return lstDownloadedFiles;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		/// <summary>
		/// PURPOSE : Download Files
		/// </summary>
		/// <param name="downloadInfo"> Creential details UserId,Password,List of files to be downloaded etc</param>
		/// <returns></returns>
		public List<DownloadedFileInfo> DownloadFiles(DownloadInfo downloadInfo)
		{
			try
			{

				List<DownloadedFileInfo> downloadedFileDetails = new List<DownloadedFileInfo>();

				string packetType = string.Empty;

				foreach (string filename in downloadInfo.DownloadList)
				{
					packetType = string.Empty;
					//if length <10 or > 11 will add an error message  and continue to download next file. 
					if (filename.Length < 10 || filename.Length > 11)
					{
						downloadedFileDetails.Add(new DownloadedFileInfo() { IsError = true, Message = "", FileName = filename });
						continue;
					}
					//finding packet type
					packetType = (filename.Substring(7, 3) == "737") ? "ZIPPACKET" : "TXTPACKET";


					//Download the selected file and adding  downloaded information to downloadedFileDetails list.
					downloadedFileDetails.Add(GetFile(downloadInfo, filename.ToUpper(), packetType));

				}


				return downloadedFileDetails;
			}
			catch (Exception)
			{
				return null;

			}
		}

		/// <summary>
		/// PURPOSE : Download File
		/// </summary>
		/// <param name="downloadInfo">It holds username, password,Precredentials etc</param>
		/// <param name="fileToGet"> file name to download UPPER CASE</param>
		/// <param name="packetType"> packet type "ZIPPACKET" / "TXTPACKET"</param>
		/// <returns></returns>
		public DownloadedFileInfo GetFile(DownloadInfo downloadInfo, string fileToGet, string packetType)
		{
			try
			{

				DownloadedFileInfo fileDetails = new DownloadedFileInfo();
				CustomWebClient webClient1 = new CustomWebClient();
				webClient1.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
				NameValueCollection formData = new NameValueCollection();
				formData["REQUEST"] = packetType;  // can also be "TXTPACKET"
				formData["CREDENTIALS"] = downloadInfo.SessionCredentials;
				formData["NAME"] = fileToGet;

				// frank add for buddy bid testing
				string url = buddyBidTest ? "https://12.191.17.47/webbid3pty/ThirdParty" :
											"https://www27.swalife.com/webbid3pty/ThirdParty";

				fileDetails.byteArray = webClient1.UploadValues(url, "POST", formData);
				fileDetails.FileName = fileToGet;
				var buf = Encoding.Convert(Encoding.GetEncoding("iso-8859-1"), Encoding.UTF8, fileDetails.byteArray);
				string temp = Encoding.UTF8.GetString(buf, 0, buf.Length);

				if (fileDetails.byteArray.Length < 200)
				{
					fileDetails.IsError = true;
					if (temp.Substring(temp.Length - 2, 2) == Environment.NewLine)
					{
						fileDetails.Message = temp.Substring(0, temp.Length - 2);
					}
					else
					{
						fileDetails.Message = temp;
					}
					//logStream.WriteLine("{0} {1}", DateTime.Now, fileDetails.message);
				}
				else
				{
					fileDetails.IsError = false;
					fileDetails.Message = string.Empty;
					//logStream.WriteLine("{0} File: {1} downloaded", DateTime.Now, fileToGet);
				}

				return fileDetails;
			}
			catch (Exception ex)
			{

				throw ex;
			}


		}
		private class CustomWebClient : WebClient
		{
			public CustomWebClient()
			{

			}

			protected override WebRequest GetWebRequest(Uri uri)
			{
				WebRequest w = base.GetWebRequest(uri);
				w.Timeout = 90000;
				return w;
			}
		}

	}
}
