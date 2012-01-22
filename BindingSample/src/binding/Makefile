BINDDIR=/src/binding
XBUILD=/Developer/usr/bin/xcodebuild
PROJECT_ROOT=XMBindingLibrarySample
PROJECT=$(PROJECT_ROOT)/XMBindingLibrarySample.xcodeproj
TARGET=XMBindingLibrarySample
BTOUCH=/Developer/MonoTouch/usr/bin/btouch


all: XMBindingLibrary.dll

libXMBindingLibrarySample-i386.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphonesimulator -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphonesimulator/lib$(TARGET).a $@

libXMBindingLibrarySample-armv6.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch armv6 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

libXMBindingLibrarySample-armv7.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch armv7 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

libXMBindingLibrarySampleUniversal.a: libXMBindingLibrarySample-armv7.a libXMBindingLibrarySample-armv6.a libXMBindingLibrarySample-i386.a
	lipo -create -output $@ $^

XMBindingLibrary.dll: AssemblyInfo.cs XMBindingLibrarySample.cs libXMBindingLibrarySampleUniversal.a
	make libXMBindingLibrarySample-i386.a
	make libXMBindingLibrarySample-armv6.a
	make libXMBindingLibrarySample-armv7.a
	make libXMBindingLibrarySampleUniversal.a
	$(BTOUCH)  XMBindingLibrarySample.cs AssemblyInfo.cs --out=$@ --link-with=libXMBindingLibrarySampleUniversal.a,libXMBindingLibrarySampleUniversal.a -x=extras.cs
clean:
	-rm -f *.a *.dll
