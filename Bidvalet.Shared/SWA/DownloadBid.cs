#region NameSpace

//using MiniZip.ZipArchive;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using System.IO.Compression;

using Bidvalet.Model;
using Bidvalet.iOS;

#endregion
namespace Bidvalet.Shared
{
	public class DownloadBid
	{	//#region Private Variables
		private string _serverUrl = string.Empty;

		//#endregion
		public DownloadedFileInfo DownloadBidFile (DownloadInfo downloadInfo, string fileToGet, string packetType)
		{
			DownloadedFileInfo fileDetails = new DownloadedFileInfo ();
			try {
				//_serverUrl = SWAConstants.SWAUrl;

				_serverUrl = GlobalSettings.QATest ? SWAConstants.QASWAUrl : SWAConstants.SWAUrl;

				WebClient webClient1 = new WebClient ();
				webClient1.Headers.Add ("Content-Type", "application/x-www-form-urlencoded");
				NameValueCollection formData = new NameValueCollection ();
				formData ["REQUEST"] = packetType;  // can also be "TXTPACKET"
				formData ["CREDENTIALS"] = downloadInfo.SessionCredentials;
				formData ["NAME"] = fileToGet;
				fileDetails.byteArray = webClient1.UploadValues (_serverUrl, "POST", formData);
				fileDetails.FileName = fileToGet;
				var buf = Encoding.Convert (Encoding.GetEncoding ("iso-8859-1"), Encoding.UTF8, fileDetails.byteArray);
				string temp = Encoding.UTF8.GetString (buf, 0, buf.Length);

				if (fileDetails.byteArray.Length < 200) {
					fileDetails.IsError = true;
					if (temp.Length > 2 && temp.Substring (temp.Length - 2, 2) == Environment.NewLine) {
						fileDetails.Message = temp.Substring (0, temp.Length - 2);
					} else {
						fileDetails.Message = temp;
					}
				} else {
					fileDetails.IsError = false;
					fileDetails.Message = string.Empty;
				}

				return fileDetails;
			} catch (Exception ex) {

				fileDetails.IsException = true;
				//throw ex;
			}
			return fileDetails;
		}


		public bool SaveDownloadedBidFiles (List<DownloadedFileInfo> lstDownloadedFiles, string appDataFilePath)
		{
			bool status = false;
			try {

				string filePath = string.Empty;
				if (lstDownloadedFiles.Count > 0) {
					foreach (DownloadedFileInfo sWAFileInfo in lstDownloadedFiles) {

						filePath = Path.Combine (appDataFilePath, sWAFileInfo.FileName);
						//If the file is error status, we dont need to save the file
						if (sWAFileInfo.IsError) {
							continue;
						}

						FileStream fStream = new FileStream (filePath, FileMode.Create);
						fStream.Write (sWAFileInfo.byteArray, 0, sWAFileInfo.byteArray.Length);
						fStream.Dispose ();

						//Extract tIhe zip file
						if (Path.GetExtension (sWAFileInfo.FileName) == ".737") {
							Extract737File (sWAFileInfo.FileName);

							//Delete the .737 file
							if (File.Exists (filePath))
								File.Delete (filePath);
						}

						
					}

				}
			} catch (Exception ex) {
                status = true;
				//throw ex;
			}
			return status;
		}

		public void Extract737File (string fileName)
		{
			// = ZipFile.Read(fileName)
			try {




                string appDataPath = WBidHelper.GetAppDataPath();
				string folderPath = appDataPath + "/" + Path.GetFileNameWithoutExtension (fileName);
				if (Directory.Exists (folderPath))
					Directory.Delete (folderPath, true);
				// else


				Directory.CreateDirectory (folderPath);

				// string personal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string target = Path.Combine (appDataPath, appDataPath + "/" + Path.GetFileNameWithoutExtension (fileName)) + "/";
				string zipFile = Path.Combine (appDataPath, fileName);

				//                var zip = new ZipArchive();
				//                zip.EasyUnzip(zipFile, folderPath, true,"");

				//ZipStorer.
				if(File.Exists(zipFile))
				{
					System.IO.Compression.ZipFile.ExtractToDirectory(zipFile,target);
				}
				//				var zipFile2 = zipFile.Replace (".737", ".zip");
				//				if (File.Exists (zipFile2))
				//					File.Delete (zipFile2);
				//
				//				File.Copy (zipFile, zipFile2);
				//
				//				// Open an existing zip file for reading
				//				ZipStorer zip = ZipStorer.Open (zipFile2, FileAccess.Read);
				//
				//				// Read the central directory collection
				//				List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir ();
				//
				//				// Look for the desired file
				//				foreach (ZipStorer.ZipFileEntry entry in dir) {
				//					zip.ExtractFile (entry, target + entry);
				//				}
				//				zip.Close ();
				//
				//				if (File.Exists (zipFile2))
				//					File.Delete (zipFile2);

			} catch (Exception ex) {

				int x = 10;

				//				throw ex;
			}
		}


		public static bool DownloadWBidFile (string appDataFilePath, string fileName)
		{
			bool status = true;

			WebClient wcClient = new WebClient ();
			try {

				wcClient.DownloadFile (SWAConstants.WBidDownloadFileUrl + fileName, appDataFilePath + "/" + fileName);
			} catch (Exception) {
				status = false;
               
			}

			return status;
		}


        public static bool DownloadWBidSeniorityFile(string appDataFilePath, string fileName)
        {
            bool status = true;

            WebClient wcClient = new WebClient();
            try
            {
                string ss = SWAConstants.WBidDownloadFileUrl;

                wcClient.DownloadFile(SWAConstants.WBidDownloadFileUrl + "SeniorityList/" + fileName, appDataFilePath + "/" + fileName);
            }
            catch (Exception ex)
            {
                status = false;

            }

            return status;
        }

		

	}
}

