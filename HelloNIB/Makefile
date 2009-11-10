APPDIR = HelloNIB.app
EXE = hello.exe
SOURCES = hello.cs
APP = $(APPDIR)/hellonib
EXTRA_DEPS = HelloWorld.nib MainWindow.nib
DATA = Background.png $(EXTRA_DEPS)

%.nib: %.xib
	ibtool $< --compile $@

include ../Make.common

