using System;
using System.Text.RegularExpressions;

namespace Bidvalet.Business
{
	public class ValidationHelper
	{
		public static bool EmailValidation(string value)
		{
			string matchpattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

			if (!Regex.IsMatch(value, matchpattern))
			{
				return false;
			}
			return true;
		}
		private bool EmployeeNumberValidation(string empNumber)
		{
			if (!Regex.Match(empNumber, "^[e,E,x,X,0-9][0-9]*$").Success)
			{
				return false;
			}
			return true;
		}
	}
}

