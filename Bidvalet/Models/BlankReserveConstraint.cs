using System;

namespace Bidvalet
{
	public class BlankReserveConstraint
	{
		/// <summary>
		/// if isBlank is false => reserve before blank
		/// if isBlank is true => Blank before Reserve
		/// </summary>
		public bool IsBlank{get;set;}

	}
}

