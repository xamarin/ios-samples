APPDIR = QuartzDemo.app
EXE = quartzdemo.exe
APP = $(APPDIR)/quartzdemo

SOURCES = \
	main.cs			\
	main.g.cs		\
	blend.cs		\
	blend.g.cs		\
	controller.cs		\
	controller.g.cs		\
	linedrawing.cs		\
	curvedrawing.cs		\
	rendereddrawing.cs	\
	polydrawing.cs		\
	imagedrawing.cs

IMAGES = \
	images/Demo.png				\
	images/QuartzImageDrawing.pdf 		\
	images/Icon.png

EXTRA_DEPS = $(IMAGES) \
	xibs/BlendView.nib	\
	xibs/DemoView.nib	\
	xibs/MainWindow.nib

DATA = $(EXTRA_DEPS)
MFLAGS=-icon=Icon.png -bundleid=com.mono-project.quartzsample

xibs/%.nib: xibs/%.xib
	ibtool $< --compile $@

include ../Make.common

