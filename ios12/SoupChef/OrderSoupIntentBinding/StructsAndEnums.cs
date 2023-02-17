using System;
using ObjCRuntime;

namespace SoupChef {
	// [Watch (5,0), iOS (12,0)]
	[Native]
	public enum OrderSoupIntentResponseCode : long {
		Unspecified = 0,
		Ready,
		ContinueInApp,
		InProgress,
		Success,
		Failure,
		FailureRequiringAppLaunch,
		FailureSoupUnavailable = 100
	}
}
