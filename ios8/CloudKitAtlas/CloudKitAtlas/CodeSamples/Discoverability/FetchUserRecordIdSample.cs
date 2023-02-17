using System.Threading.Tasks;

using CloudKit;

namespace CloudKitAtlas {
	public class FetchUserRecordIdSample : CodeSample {
		public FetchUserRecordIdSample ()
			: base (title: "FetchUserRecordId",
					className: "CKContainer",
					methodName: ".FetchUserRecordId()",
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
