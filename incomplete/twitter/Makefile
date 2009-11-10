APPDIR = TwitterSharp.app
EXE = TwitterSharp.exe
APP = $(APPDIR)/TwitterSharp

SOURCES = \
	main.cs		\
	main.g.cs

IMAGES = 

EXTRA_DEPS = $(IMAGES) \
	xibs/MainWindow.nib

DATA = $(EXTRA_DEPS)
MFLAGS=-bundleid=com.mono-project.com.twittersharp

xibs/%.nib: xibs/%.xib
	ibtool $< --compile $@

include ../Make.common

