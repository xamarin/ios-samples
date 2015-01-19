using System;
using Security;

namespace KeychainTouchID
{
	public static class Extensions
	{
		public static string GetDescription(this SecStatusCode code)
		{
			string description = string.Empty;
			switch (code) {
			case SecStatusCode.Success:
				description = Text.SUCCESS;
				break;
			case SecStatusCode.DuplicateItem:
				description = Text.ERROR_ITEM_ALREADY_EXISTS;
				break;
			case SecStatusCode.ItemNotFound:
				description = Text.ERROR_ITEM_NOT_FOUND;
				break;
			case SecStatusCode.AuthFailed:
				description = Text.ERROR_ITEM_AUTHENTICATION_FAILED;
				break;
			default:
				description = code.ToString ();
				break;
			}

			return description;
		}
	}
}

