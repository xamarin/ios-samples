using System;
using System.Threading.Tasks;

namespace PrivacyPrompts
{
	public interface IPrivacyManager
	{
		Task RequestAccess();
		string CheckAccess();
	}
}

