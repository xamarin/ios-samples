APPDIR = Reachability.app
EXE = reachability.exe
APP = $(APPDIR)/Reachability
MFLAGS=-icon=Icon.png
EXTRA_DEPS = MainWindow.nib
DATA =  $(EXTRA_DEPS)

SOURCES = \
	main.cs 		\
	MainWindow.xib.designer.cs

%.nib: %.xib
	ibtool $< --compile $@

include ../Make.common

MainWindow.xib.designer.cs: MainWindow.xib
	$(MIBTOOL) MainWindow.xib

