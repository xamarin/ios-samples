using System;
using System.Threading.Tasks;
using CloudKit;

namespace CloudKitAtlas
{
	public class FetchUserRecordIdSample : CodeSample
	{
		public FetchUserRecordIdSample ()
			: base (title: "fetchUserRecordIDWithCompletionHandler",
					className: "CKContainer",
					methodName: ".fetchUserRecordIDWithCompletionHandler()", // TODO: fix method name
					descriptionKey: "Discoverability.FetchUserRecordID")
		{
		}

		public async override Task<Results> Run ()
		{
			var container = CKContainer.DefaultContainer;
			var recordId = await container.FetchUserRecordIdAsync ();
			return new Results (new IResult [] { new CKRecordIdWrapper (recordId) });
		}
	}
}