SUBDIRS = \
	GLPaint		\
	Hello		\
	HelloNIB	\
	Metronome	\
	MonoTouchCells	\
	OpenGLESSample	\
	monocatalog	\
	quartz		\
	Sample		\
	twitter		\
	wayup

all:
	for dir in $(SUBDIRS); do	\
		(cd $$dir && make);	\
	done

clean:
	for dir in $(SUBDIRS); do		\
		(cd $$dir && make clean);	\
	done

install:
	for dir in $(SUBDIRS); do		\
		(cd $$dir && make install);	\
	done
