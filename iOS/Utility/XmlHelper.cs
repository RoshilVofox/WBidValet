using System;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using Bidvalet.Model;
using System.Collections.Generic;

namespace Bidvalet.iOS
{
	public class XmlHelper
	{
		#region Methods
		/// <summary>
		/// PURPOSE : Save Configuration details to XML
		/// </summary>
		/// <param name="wBidINI"></param>
		/// <returns></returns>
		public static bool SerializeToXml<T>(T configType, string filePath)
		{
			bool status = false;
			try
			{
				XmlWriterSettings xmlWriterSettings;
				XmlSerializerNamespaces xmlSerializerNamespaces;

				xmlWriterSettings = new XmlWriterSettings
				{
					Indent = true,
					OmitXmlDeclaration = false,
					NamespaceHandling = NamespaceHandling.OmitDuplicates,
					Encoding = Encoding.UTF8,
					CloseOutput = true


				};
				xmlSerializerNamespaces = new XmlSerializerNamespaces();
				xmlSerializerNamespaces.Add("", "");

				if (!Directory.Exists(Path.GetDirectoryName(filePath)))
					Directory.CreateDirectory(Path.GetDirectoryName(filePath));

				using (FileStream configurationFileStream = new FileStream(filePath, FileMode.Create))
				{

					using (XmlWriter xmlWriter = XmlWriter.Create(configurationFileStream, xmlWriterSettings))
					{
						XmlSerializer serializer = new XmlSerializer(typeof(T));
						serializer.Serialize(xmlWriter, configType, xmlSerializerNamespaces);
					}
				}

				status = true;
			}
			catch (Exception)
			{
				status = false;
			}

			return status;
		}

		/// <summary>
		/// Load configuration details from XML
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static T DeserializeFromXml<T>(string filePath)
		{
			try
			{

				T wBidConfiguration;
				using (TextReader configurationFileStream = new StreamReader(filePath))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					wBidConfiguration = (T)xmlSerializer.Deserialize(configurationFileStream);
					return wBidConfiguration;
				}


			}
			catch (Exception ex)
			{
				throw;
			}
		}



		//		public static void   ReCreateStateFile(string fileName, int lineCount, int startValue)
		//		{
		//
		//
		//
		//
		//			if (GlobalSettings.WbidUserContent == null || GlobalSettings.WbidUserContent.UserInformation == null)
		//			{
		//				GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser> (WBidHelper.WBidUserFilePath);
		//
		//
		//			}
		//
		//			//WBidLogEvent logEvent = new WBidLogEvent();
		//			//logEvent.LogEvent(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "wbsRecreate", "0", "0");
		//
		//
		//
		//
		//
		//			}

		public static WBidStateCollection ReadStateFile(string StatefileName)
		{


			WBidStateCollection wBidStateCollection = null;


            try
            {
                wBidStateCollection = XmlHelper.DeserializeFromXml<WBidStateCollection>(StatefileName);


                if (decimal.Parse(wBidStateCollection.Version) < 2.0m)
                {
                    foreach (WBidState state in wBidStateCollection.StateList)
                    {
                        if (state.Constraints.BulkOvernightCity == null)
                        {
                            state.Constraints.BulkOvernightCity = new BulkOvernightCityCx() { OverNightNo = new List<int>(), OverNightYes = new List<int>() };

                        }
                        if (state.Weights.OvernightCitybulk == null)
                        {
                            state.Weights.OvernightCitybulk = new List<Wt2Parameter>();

                        }
                        if (state.CxWtState.BulkOC == null)
                        {
                            state.CxWtState.BulkOC = new StateStatus() { Cx = false, Wt = false };

                        }
                        {

                        }



                    }

                    XmlHelper.SerializeToXml<WBidStateCollection>(wBidStateCollection, StatefileName);



                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

			return wBidStateCollection;

		}
		private static Object readlLock = new Object();
		private static Object writecLock = new Object();
		public static bool SerializeToXmlForUserFile<T>(T configType, string filePath)
		{

			bool status = false;
			try
			{
				lock (writecLock)
				{
					XmlWriterSettings xmlWriterSettings;
					XmlSerializerNamespaces xmlSerializerNamespaces;

					xmlWriterSettings = new XmlWriterSettings
					{
						Indent = true,
						OmitXmlDeclaration = false,
						NamespaceHandling = NamespaceHandling.OmitDuplicates,
						Encoding = Encoding.UTF8,
						CloseOutput = true


					};
					xmlSerializerNamespaces = new XmlSerializerNamespaces();
					xmlSerializerNamespaces.Add("", "");

					if (!Directory.Exists(Path.GetDirectoryName(filePath)))
						Directory.CreateDirectory(Path.GetDirectoryName(filePath));

					using (FileStream configurationFileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
					{

						using (XmlWriter xmlWriter = XmlWriter.Create(configurationFileStream, xmlWriterSettings))
						{
							XmlSerializer serializer = new XmlSerializer(typeof(T));
							serializer.Serialize(xmlWriter, configType, xmlSerializerNamespaces);
							xmlWriter.Close();
						}
					}

					status = true;
				}

			}
			catch (Exception ex)
			{
				status = false;
			}

			return status;
		}

		#endregion


	}

}

