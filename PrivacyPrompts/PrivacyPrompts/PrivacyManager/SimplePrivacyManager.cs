using System;
using System.Threading.Tasks;

namespace PrivacyPrompts
{
	public class SimplePrivacyManager : IPrivacyManager
	{
		public Task RequestAccess ()
		{
			return Task.FromResult<object> (true);
		}

		public string CheckAccess ()
		{
			return "allright";
		}
	}
}

