namespace Tandm;

public static class MKAnnotationWrapperExtensions
{
	public static MKClusterAnnotation UnwrapClusterAnnotation (IMKAnnotation annotation) {
		return ObjCRuntime.Runtime.GetNSObject (annotation.Handle) as MKClusterAnnotation ?? throw new ArgumentException ($"{nameof (annotation.Handle)} was not of type MKClusterAnnotation");
	}
}
