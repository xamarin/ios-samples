Streaming Audio Sample
======================

This sample illustrates how to use AudioToolbox's AudioFileStream to 
parse an audio stream progressively and play the audio back.
 
The audio is a creative commons MP3 file that is downloaded from a
website using Mono's HTTP stack.
 
There are two samples:

* One plays as it streams, and does not attempt to buffer more than the audio buffers that are used for AudioToolbox.
 
* The second sample shows how to save a copy of the data as it is being downloaded (for example, a podcasting application would stream audio and retain a copy).

Preview
-------
![iPhone running Streaming Audio sample](http://farm7.static.flickr.com/6144/6002389235_5c1d164cc1.jpg)

Authors
-------

Miguel de Icaza
