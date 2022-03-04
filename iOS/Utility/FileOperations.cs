﻿using System;

using System.IO;

namespace Bidvalet.iOS
{
	public class FileOperations
	{
		#region Methods
		/// <summary>
		/// Write current bid details
		/// </summary>
		/// <param name="path"></param>
		/// <param name="bidDetails"></param>
		public static void WriteCurrentBidDetails(string path, string bidDetails)
		{

			try{
				File.WriteAllText(path, bidDetails);
			}
			catch(Exception ex) {
				throw ex;
			}
		}


		/// <summary>
		/// Read Current Bid details
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ReadCurrentBidDetails(string path)
		{

			string content = string.Empty;
			if (File.Exists(path))
			{
				content = File.ReadAllText(path);
			}

			return content;
		} 
		#endregion
	}
}

