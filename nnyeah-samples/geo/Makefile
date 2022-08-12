
ifndef MS_PLATFORM_DLL
$(error MS_PLATFORM_DLL is not set. See the package README.md for more information.)
endif

PACKAGE=xam.plugin.geolocator
VERSION=4.5.0.6
FULLPACKAGE=$(PACKAGE).$(VERSION).nupkg
NUGETAPI=https://api.nuget.org/v3-flatcontainer
XAMARIN_PLATFORM_DLL=/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/Xamarin.iOS.dll

all: prepare build


prepare: permissions ./nuget nuget/$(FULLPACKAGE)
	dotnet nnyeah \
		--input nuget/lib/xamarinios10/Plugin.Geolocator.dll \
		--output nuget/Plugin.Geolocator.dll \
		--xamarin-assembly=$(XAMARIN_PLATFORM_DLL) \
		--microsoft-assembly=$(MS_PLATFORM_DLL) \
		--force-overwrite

permissions: ../perm/nuget/Plugin.Permissions.dll

../perm/nuget/Plugin.Permissions.dll:
	cd ../perm && make

build:
	dotnet build

nuget:
	mkdir $@

nuget/$(FULLPACKAGE):
	wget $(NUGETAPI)/$(PACKAGE)/$(VERSION)/$(FULLPACKAGE) -O nuget/$(FULLPACKAGE)
	unzip nuget/$(FULLPACKAGE) -d nuget

clean:
	rm -rf ./nuget ./bin ./obj
	cd ../perm && make clean
