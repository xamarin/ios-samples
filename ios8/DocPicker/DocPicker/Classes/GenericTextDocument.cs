using System;
using Foundation;
using UIKit;

namespace DocPicker
{
	/// <summary>
	/// The file representation is a simple text file with .txt extension.
	/// </summary>
	public class GenericTextDocument : UIDocument
	{
		#region Private Variable Storage
		private NSString _dataModel;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the contents of the document
		/// </summary>
		/// <value>The document contents.</value>
		public string Contents {
			get { return _dataModel.ToString (); }
			set { _dataModel = new NSString(value); }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="DocPicker.GenericTextDocument"/> class.
		/// </summary>
		public GenericTextDocument (NSUrl url) : base (url)
		{
			// Set the default document text
			this.Contents = "";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DocPicker.GenericTextDocument"/> class.
		/// </summary>
		/// <param name="contents">Contents.</param>
		public GenericTextDocument (NSUrl url, string contents) : base (url)
		{
			// Set the default document text
			this.Contents = contents;
		}
		#endregion

		#region Override Methods
		/// <Docs>To be added.</Docs>
		/// <param name="outError">To be added.</param>
		/// <returns>To be added.</returns>
		/// <para>(More documentation for this node is coming)</para>
		/// <summary>
		/// Loads from contents.
		/// </summary>
		/// <param name="contents">Contents.</param>
		/// <param name="typeName">Type name.</param>
		public override bool LoadFromContents (NSObject contents, string typeName, out NSError outError)
		{
			// Clear the error state
			outError = null;

			// Were any contents passed to the document?
			if (contents != null) {
				_dataModel = NSString.FromData( (NSData)contents, NSStringEncoding.UTF8 );
			}

			// Inform caller that the document has been modified
			RaiseDocumentModified (this);

			// Return success
			return true;
		}

		/// <Docs>To be added.</Docs>
		/// <summary>
		/// Application developers should override this method to return the document data to be saved.
		/// </summary>
		/// <remarks>(More documentation for this node is coming)</remarks>
		/// <returns>The for type.</returns>
		/// <param name="typeName">Type name.</param>
		/// <param name="outError">Out error.</param>
		public override NSObject ContentsForType (string typeName, out NSError outError)
		{
			// Clear the error state
			outError = null;

			// Convert the contents to a NSData object and return it
			NSData docData = _dataModel.Encode(NSStringEncoding.UTF8);
			return docData;
		}
		#endregion

		#region Events
		/// <summary>
		/// Document modified delegate.
		/// </summary>
		public delegate void DocumentModifiedDelegate(GenericTextDocument document);
		public event DocumentModifiedDelegate DocumentModified;

		/// <summary>
		/// Raises the document modified event.
		/// </summary>
		internal void RaiseDocumentModified(GenericTextDocument document) {
			// Inform caller
			if (this.DocumentModified != null) {
				this.DocumentModified (document);
			}
		}
		#endregion
	}
}

