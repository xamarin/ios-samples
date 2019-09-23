using System;
using Security;

namespace AddingTheSignInWithAppleFlowToYourApp {
	public class KeychainException : Exception {
		public SecStatusCode StatusCode { get; private set; }

		public KeychainException (SecStatusCode statusCode)
		{
			StatusCode = statusCode;
		}
	}

	public class KeychainNoPasswordException : KeychainException {
		public KeychainNoPasswordException (SecStatusCode statusCode) : base (statusCode)
		{
		}
	}

	public class KeychainUnexpectedPasswordDataException : KeychainException {
		public KeychainUnexpectedPasswordDataException (SecStatusCode statusCode) : base (statusCode)
		{
		}
	}

	public class KeychainUnexpectedItemDataException : KeychainException {
		public KeychainUnexpectedItemDataException (SecStatusCode statusCode) : base (statusCode)
		{
		}
	}

	public class KeychainUnhandledErrorException : KeychainException {
		public KeychainUnhandledErrorException (SecStatusCode statusCode) : base (statusCode)
		{
		}
	}
}
