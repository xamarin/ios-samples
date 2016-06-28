using System;
using System.Runtime.InteropServices;

using CloudKit;
using ObjCRuntime;

namespace CloudKitAtlas
{
	// TODO: This is a workaround for https://bugzilla.xamarin.com/show_bug.cgi?id=42163
	// CKFetchRecordChangesOperation doesn't accept null for CKServerChangeToken – this is bug
	// We have to create CKFetchRecordChangesOperation w/o using ctor
	// This is a factory class to workaround this issue

	public static class CKFetchRecordChangesOperationFactory
	{
		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend (IntPtr receiver, IntPtr selector);

		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		public static extern IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		public static CKFetchRecordChangesOperation GetOperation (CKRecordZoneID recordZoneID, CKServerChangeToken previousServerChangeToken)
		{
			IntPtr classPtr = Class.GetHandle ("CKFetchRecordChangesOperation");
			IntPtr allocSel = Selector.GetHandle ("alloc");
			IntPtr objPtr = IntPtr_objc_msgSend (classPtr, allocSel);

			IntPtr ret = IntPtr_objc_msgSend_IntPtr_IntPtr (objPtr, Selector.GetHandle ("initWithRecordZoneID:previousServerChangeToken:"), recordZoneID.Handle, previousServerChangeToken == null ? IntPtr.Zero : previousServerChangeToken.Handle);
			return Runtime.GetNSObject<CKFetchRecordChangesOperation> (ret);
		}
	}
}
