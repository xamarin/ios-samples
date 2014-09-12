using System;
using HealthKit;

namespace Fit
{
	public interface IHealthStore
	{
		HKHealthStore HealthStore { get; set; }
	}
}

