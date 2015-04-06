using System;
using System.Threading.Tasks;

namespace PrivacyPrompts
{
	public interface IPrivacyManager : IDisposable
	{
		Task RequestAccess();
		string CheckAccess();
	}
}

