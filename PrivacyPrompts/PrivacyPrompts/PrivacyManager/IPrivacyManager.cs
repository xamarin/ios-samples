using System;

namespace PrivacyPrompts
{
	public interface IPrivacyManager : IDisposable
	{
		void RequestAccess();
		string CheckAccess();
	}
}

