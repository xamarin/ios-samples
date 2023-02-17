
ifndef MS_PLATFORM_DLL
$(error MS_PLATFORM_DLL is not set. See the package README.md for more information.)
endif

PACKAGE=Plugin.Permissions
VERSION=6.0.1
FULLPACKAGE=$(PACKAGE).$(VERSION).nupkg
NUGETAPI=https://nuget.org/api/v2/package
XAMARIN_PLATFORM_DLL=/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/Xamarin.iOS.dll

all: prepare

prepare: ./nuget nuget/$(FULLPACKAGE)
	dotnet nnyeah \
		--input nuget/lib/xamarinios10/Plugin.Permissions.dll \
		--output nuget/Plugin.Permissions.dll \
		--xamarin-assembly=$(XAMARIN_PLATFORM_DLL) \
		--microsoft-assembly=$(MS_PLATFORM_DLL) \
		--force-overwrite
nuget:
	mkdir $@

nuget/$(FULLPACKAGE):
	wget $(NUGETAPI)/$(PACKAGE)/$(VERSION) -O nuget/$(FULLPACKAGE)
	unzip nuget/$(FULLPACKAGE) -d nuget

clean:
	rm -rf ./nuget

