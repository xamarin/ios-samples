APPDIR = Wayup.app
EXE = wayup.exe
SOURCES = wayup.cs wayup.g.cs
APP = $(APPDIR)/hello
MFLAGS=-icon=Icon.png
EXTRA_DEPS = MainWindow.nib
DATA = crate.png Icon.png $(EXTRA_DEPS)

%.nib: %.xib
	ibtool $< --compile $@

include ../Make.common

