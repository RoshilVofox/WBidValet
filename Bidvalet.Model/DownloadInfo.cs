using System;
using System.Collections.Generic;

namespace Bidvalet.Model
{
	public class DownloadInfo
	{
		#region Properties
		/// <summary>
		/// User id
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		/// Password
		/// </summary>
		public string Password { get; set; }


		/// <summary>
		/// Session Credentials
		/// </summary>
		public string SessionCredentials { get; set; }

		/// <summary>
		/// PreCredentials
		/// </summary>
		public string PreCredentials { get; set; }
		/// <summary>
		/// Download list
		/// </summary>
		public List<string> DownloadList { get; set; } 
		#endregion
	}
}

