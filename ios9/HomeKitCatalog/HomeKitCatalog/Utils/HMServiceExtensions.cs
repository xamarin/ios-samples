using System;

using Foundation;
using HomeKit;
using ObjCRuntime;

namespace HomeKitCatalog
{
	public static class HMServiceExtensions
	{

		// returns: `true` if this service supports the `associatedServiceType` property; `false` otherwise.
		public static bool SupportsAssociatedServiceType (this HMService self)
		{
			return self.ServiceType == HMServiceType.Outlet || self.ServiceType == HMServiceType.Switch;
		}

		// returns: `true` if this service is a 'control type'; `false` otherwise.
		public static bool IsControllType (this HMService self)
		{
			return self.ServiceType != HMServiceType.AccessoryInformation && self.ServiceType != HMServiceType.LockManagement;
		}
	}

	// TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=33909
	internal static class HMServiceKeys
	{
		public static readonly IntPtr Handle = Dlfcn.dlopen ("/System/Library/Frameworks/HomeKit.framework/HomeKit", 0);

		static NSString _AccessoryInformation;

		static NSString _MotionSensor;

		static NSString _OccupancySensor;

		static NSString _Outlet;

		static NSString _SecuritySystem;

		static NSString _SmokeSensor;

		static NSString _StatefulProgrammableSwitch;

		static NSString _WindowCovering;

		static NSString _Window;

		static NSString _Thermostat;

		static NSString _TemperatureSensor;

		static NSString _Switch;

		static NSString _StatelessProgrammableSwitch;

		static NSString _LockMechanism;

		static NSString _Door;

		static NSString _ContactSensor;

		static NSString _CarbonMonoxideSensor;

		static NSString _CarbonDioxideSensor;

		static NSString _Battery;

		static NSString _AirQualitySensor;

		static NSString _Fan;

		static NSString _LockManagement;

		static NSString _LightSensor;

		static NSString _LightBulb;

		static NSString _LeakSensor;

		static NSString _HumiditySensor;

		static NSString _GarageDoorOpener;

		//
		// Static Properties
		//
		public static NSString AccessoryInformation {
			get {
				if (HMServiceKeys._AccessoryInformation == null) {
					HMServiceKeys._AccessoryInformation = Dlfcn.GetStringConstant (Handle, "HMServiceTypeAccessoryInformation");
				}
				return HMServiceKeys._AccessoryInformation;
			}
		}

		public static NSString AirQualitySensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._AirQualitySensor == null) {
					HMServiceKeys._AirQualitySensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeAirQualitySensor");
				}
				return HMServiceKeys._AirQualitySensor;
			}
		}

		public static NSString Battery {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._Battery == null) {
					HMServiceKeys._Battery = Dlfcn.GetStringConstant (Handle, "HMServiceTypeBattery");
				}
				return HMServiceKeys._Battery;
			}
		}

		public static NSString CarbonDioxideSensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._CarbonDioxideSensor == null) {
					HMServiceKeys._CarbonDioxideSensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeCarbonDioxideSensor");
				}
				return HMServiceKeys._CarbonDioxideSensor;
			}
		}

		public static NSString CarbonMonoxideSensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._CarbonMonoxideSensor == null) {
					HMServiceKeys._CarbonMonoxideSensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeCarbonMonoxideSensor");
				}
				return HMServiceKeys._CarbonMonoxideSensor;
			}
		}

		public static NSString ContactSensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._ContactSensor == null) {
					HMServiceKeys._ContactSensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeContactSensor");
				}
				return HMServiceKeys._ContactSensor;
			}
		}

		public static NSString Door {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._Door == null) {
					HMServiceKeys._Door = Dlfcn.GetStringConstant (Handle, "HMServiceTypeDoor");
				}
				return HMServiceKeys._Door;
			}
		}

		public static NSString Fan {
			get {
				if (HMServiceKeys._Fan == null) {
					HMServiceKeys._Fan = Dlfcn.GetStringConstant (Handle, "HMServiceTypeFan");
				}
				return HMServiceKeys._Fan;
			}
		}

		public static NSString GarageDoorOpener {
			get {
				if (HMServiceKeys._GarageDoorOpener == null) {
					HMServiceKeys._GarageDoorOpener = Dlfcn.GetStringConstant (Handle, "HMServiceTypeGarageDoorOpener");
				}
				return HMServiceKeys._GarageDoorOpener;
			}
		}

		public static NSString HumiditySensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._HumiditySensor == null) {
					HMServiceKeys._HumiditySensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeHumiditySensor");
				}
				return HMServiceKeys._HumiditySensor;
			}
		}

		public static NSString LeakSensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._LeakSensor == null) {
					HMServiceKeys._LeakSensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeLeakSensor");
				}
				return HMServiceKeys._LeakSensor;
			}
		}

		public static NSString LightBulb {
			get {
				if (HMServiceKeys._LightBulb == null) {
					HMServiceKeys._LightBulb = Dlfcn.GetStringConstant (Handle, "HMServiceTypeLightbulb");
				}
				return HMServiceKeys._LightBulb;
			}
		}

		public static NSString LightSensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._LightSensor == null) {
					HMServiceKeys._LightSensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeLightSensor");
				}
				return HMServiceKeys._LightSensor;
			}
		}

		public static NSString LockManagement {
			get {
				if (HMServiceKeys._LockManagement == null) {
					HMServiceKeys._LockManagement = Dlfcn.GetStringConstant (Handle, "HMServiceTypeLockManagement");
				}
				return HMServiceKeys._LockManagement;
			}
		}

		public static NSString LockMechanism {
			get {
				if (HMServiceKeys._LockMechanism == null) {
					HMServiceKeys._LockMechanism = Dlfcn.GetStringConstant (Handle, "HMServiceTypeLockMechanism");
				}
				return HMServiceKeys._LockMechanism;
			}
		}

		public static NSString MotionSensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._MotionSensor == null) {
					HMServiceKeys._MotionSensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeMotionSensor");
				}
				return HMServiceKeys._MotionSensor;
			}
		}

		public static NSString OccupancySensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._OccupancySensor == null) {
					HMServiceKeys._OccupancySensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeOccupancySensor");
				}
				return HMServiceKeys._OccupancySensor;
			}
		}

		public static NSString Outlet {
			get {
				if (HMServiceKeys._Outlet == null) {
					HMServiceKeys._Outlet = Dlfcn.GetStringConstant (Handle, "HMServiceTypeOutlet");
				}
				return HMServiceKeys._Outlet;
			}
		}

		public static NSString SecuritySystem {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._SecuritySystem == null) {
					HMServiceKeys._SecuritySystem = Dlfcn.GetStringConstant (Handle, "HMServiceTypeSecuritySystem");
				}
				return HMServiceKeys._SecuritySystem;
			}
		}

		public static NSString SmokeSensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._SmokeSensor == null) {
					HMServiceKeys._SmokeSensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeSmokeSensor");
				}
				return HMServiceKeys._SmokeSensor;
			}
		}

		public static NSString StatefulProgrammableSwitch {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._StatefulProgrammableSwitch == null) {
					HMServiceKeys._StatefulProgrammableSwitch = Dlfcn.GetStringConstant (Handle, "HMServiceTypeStatefulProgrammableSwitch");
				}
				return HMServiceKeys._StatefulProgrammableSwitch;
			}
		}

		public static NSString StatelessProgrammableSwitch {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._StatelessProgrammableSwitch == null) {
					HMServiceKeys._StatelessProgrammableSwitch = Dlfcn.GetStringConstant (Handle, "HMServiceTypeStatelessProgrammableSwitch");
				}
				return HMServiceKeys._StatelessProgrammableSwitch;
			}
		}

		public static NSString Switch {
			get {
				if (HMServiceKeys._Switch == null) {
					HMServiceKeys._Switch = Dlfcn.GetStringConstant (Handle, "HMServiceTypeSwitch");
				}
				return HMServiceKeys._Switch;
			}
		}

		public static NSString TemperatureSensor {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._TemperatureSensor == null) {
					HMServiceKeys._TemperatureSensor = Dlfcn.GetStringConstant (Handle, "HMServiceTypeTemperatureSensor");
				}
				return HMServiceKeys._TemperatureSensor;
			}
		}

		public static NSString Thermostat {
			get {
				if (HMServiceKeys._Thermostat == null) {
					HMServiceKeys._Thermostat = Dlfcn.GetStringConstant (Handle, "HMServiceTypeThermostat");
				}
				return HMServiceKeys._Thermostat;
			}
		}

		public static NSString Window {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._Window == null) {
					HMServiceKeys._Window = Dlfcn.GetStringConstant (Handle, "HMServiceTypeWindow");
				}
				return HMServiceKeys._Window;
			}
		}

		public static NSString WindowCovering {
			[Availability (Introduced = Platform.iOS_9_0)]
			get {
				if (HMServiceKeys._WindowCovering == null) {
					HMServiceKeys._WindowCovering = Dlfcn.GetStringConstant (Handle, "HMServiceTypeWindowCovering");
				}
				return HMServiceKeys._WindowCovering;
			}
		}

		public static NSString Convert (HMServiceType s)
		{
			if (s == HMServiceType.LightBulb)
				return HMServiceKeys.LightBulb;
			if (s == HMServiceType.Switch)
				return HMServiceKeys.Switch;
			if (s == HMServiceType.Thermostat)
				return HMServiceKeys.Thermostat;
			if (s == HMServiceType.GarageDoorOpener)
				return HMServiceKeys.GarageDoorOpener;
			if (s == HMServiceType.AccessoryInformation)
				return HMServiceKeys.AccessoryInformation;
			if (s == HMServiceType.Fan)
				return HMServiceKeys.Fan;
			if (s == HMServiceType.Outlet)
				return HMServiceKeys.Outlet;
			if (s == HMServiceType.LockMechanism)
				return HMServiceKeys.LockMechanism;
			if (s == HMServiceType.LockManagement)
				return HMServiceKeys.LockManagement;
			// iOS 9
			if (s == HMServiceType.AirQualitySensor)
				return HMServiceKeys.AirQualitySensor;
			if (s == HMServiceType.Battery)
				return HMServiceKeys.Battery;
			if (s == HMServiceType.CarbonDioxideSensor)
				return HMServiceKeys.CarbonDioxideSensor;
			if (s == HMServiceType.CarbonMonoxideSensor)
				return HMServiceKeys.CarbonMonoxideSensor;
			if (s == HMServiceType.ContactSensor)
				return HMServiceKeys.ContactSensor;
			if (s == HMServiceType.Door)
				return HMServiceKeys.Door;
			if (s == HMServiceType.HumiditySensor)
				return HMServiceKeys.HumiditySensor;
			if (s == HMServiceType.LeakSensor)
				return HMServiceKeys.LeakSensor;
			if (s == HMServiceType.LightSensor)
				return HMServiceKeys.LightSensor;
			if (s == HMServiceType.MotionSensor)
				return HMServiceKeys.MotionSensor;
			if (s == HMServiceType.OccupancySensor)
				return HMServiceKeys.OccupancySensor;
			if (s == HMServiceType.SecuritySystem)
				return HMServiceKeys.SecuritySystem;
			if (s == HMServiceType.StatefulProgrammableSwitch)
				return HMServiceKeys.StatefulProgrammableSwitch;
			if (s == HMServiceType.StatelessProgrammableSwitch)
				return HMServiceKeys.StatelessProgrammableSwitch;
			if (s == HMServiceType.SmokeSensor)
				return HMServiceKeys.SmokeSensor;
			if (s == HMServiceType.TemperatureSensor)
				return HMServiceKeys.TemperatureSensor;
			if (s == HMServiceType.Window)
				return HMServiceKeys.Window;
			if (s == HMServiceType.WindowCovering)
				return HMServiceKeys.WindowCovering;

			return null;
		}

		public static HMServiceType Convert (NSString s)
		{
			if (s == HMServiceKeys.LightBulb)
				return HMServiceType.LightBulb;
			if (s == HMServiceKeys.Switch)
				return HMServiceType.Switch;
			if (s == HMServiceKeys.Thermostat)
				return HMServiceType.Thermostat;
			if (s == HMServiceKeys.GarageDoorOpener)
				return HMServiceType.GarageDoorOpener;
			if (s == HMServiceKeys.AccessoryInformation)
				return HMServiceType.AccessoryInformation;
			if (s == HMServiceKeys.Fan)
				return HMServiceType.Fan;
			if (s == HMServiceKeys.Outlet)
				return HMServiceType.Outlet;
			if (s == HMServiceKeys.LockMechanism)
				return HMServiceType.LockMechanism;
			if (s == HMServiceKeys.LockManagement)
				return HMServiceType.LockManagement;
			// iOS 9
			if (s == HMServiceKeys.AirQualitySensor)
				return HMServiceType.AirQualitySensor;
			if (s == HMServiceKeys.Battery)
				return HMServiceType.Battery;
			if (s == HMServiceKeys.CarbonDioxideSensor)
				return HMServiceType.CarbonDioxideSensor;
			if (s == HMServiceKeys.CarbonMonoxideSensor)
				return HMServiceType.CarbonMonoxideSensor;
			if (s == HMServiceKeys.ContactSensor)
				return HMServiceType.ContactSensor;
			if (s == HMServiceKeys.Door)
				return HMServiceType.Door;
			if (s == HMServiceKeys.HumiditySensor)
				return HMServiceType.HumiditySensor;
			if (s == HMServiceKeys.LeakSensor)
				return HMServiceType.LeakSensor;
			if (s == HMServiceKeys.LightSensor)
				return HMServiceType.LightSensor;
			if (s == HMServiceKeys.MotionSensor)
				return HMServiceType.MotionSensor;
			if (s == HMServiceKeys.OccupancySensor)
				return HMServiceType.OccupancySensor;
			if (s == HMServiceKeys.SecuritySystem)
				return HMServiceType.SecuritySystem;
			if (s == HMServiceKeys.StatefulProgrammableSwitch)
				return HMServiceType.StatefulProgrammableSwitch;
			if (s == HMServiceKeys.StatelessProgrammableSwitch)
				return HMServiceType.StatelessProgrammableSwitch;
			if (s == HMServiceKeys.SmokeSensor)
				return HMServiceType.SmokeSensor;
			if (s == HMServiceKeys.TemperatureSensor)
				return HMServiceType.TemperatureSensor;
			if (s == HMServiceKeys.Window)
				return HMServiceType.Window;
			if (s == HMServiceKeys.WindowCovering)
				return HMServiceType.WindowCovering;

			return HMServiceType.None;
		}
	}
}

