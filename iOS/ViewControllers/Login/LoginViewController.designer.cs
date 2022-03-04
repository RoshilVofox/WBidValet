// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Bidvalet.iOS
{
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		UIKit.UIButton btLogin { get; set; }

		[Outlet]
		UIKit.UITextField edPassword { get; set; }

		[Outlet]
		UIKit.UITextField edUsername { get; set; }

		[Outlet]
		UIKit.UILabel lbLoginTitle { get; set; }

		[Action ("OnLoginClicked:")]
		partial void OnLoginClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btLogin != null) {
				btLogin.Dispose ();
				btLogin = null;
			}

			if (edPassword != null) {
				edPassword.Dispose ();
				edPassword = null;
			}

			if (edUsername != null) {
				edUsername.Dispose ();
				edUsername = null;
			}

			if (lbLoginTitle != null) {
				lbLoginTitle.Dispose ();
				lbLoginTitle = null;
			}
		}
	}
}
