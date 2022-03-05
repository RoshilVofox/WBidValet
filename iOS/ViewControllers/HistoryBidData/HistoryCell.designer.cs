// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Bidvalet.iOS.ViewControllers.HistoryBidData
{
	[Register ("HistoryCell")]
	partial class HistoryCell
	{
		[Outlet]
		UIKit.UILabel cellTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (cellTitle != null) {
				cellTitle.Dispose ();
				cellTitle = null;
			}
		}
	}
}
