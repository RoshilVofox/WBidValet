using System;
using System.Text.RegularExpressions;

namespace Bidvalet.iOS
{
	public static class InputTextValidator
	{
		
		const string validEmailPattern = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
							@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

		static Regex ValidEmailRegex = CreateValidEmailRegex();

		private static Regex CreateValidEmailRegex(){
			return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
		}

		public static bool EmailIsValid(string emailAddress){
			bool isValid = ValidEmailRegex.IsMatch(emailAddress);
			return isValid;
		}
		//or 
		public static bool IsValidEmail(string email){
			try {
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch {
				return false;
			}
		}
		//input text not empty

	}
}

