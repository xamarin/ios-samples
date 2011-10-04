using System;
using System.IO;
using System.Threading;
using System.Net;

//
// Exposes the data pushed into the stream and uses the
// @storage for persisting the data.
//
class QueueStream : Stream {
	Stream writeStream;
	Stream readStream;
	long size;
	bool done;
	object plock = new object ();
	
	public QueueStream (string storage)
	{
		writeStream = new FileStream (storage, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 4096);
		readStream = new FileStream (storage, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096);
	}

	public override bool CanRead {
		get { return true; }
	}
	
	public override bool CanSeek {
		get { return false; }
	}
	
	public override bool CanWrite {
		get { return false; }
	}
	
	public override long Length {
		get { return readStream.Length; }
	}
	
	public override long Position {
		get { return readStream.Position; }
		set { throw new NotImplementedException (); }
	}

	public override void Flush ()
	{
	}

	public override int Read (byte [] buffer, int offset, int count)
	{
		lock (plock){
			while (true) {
				if (Position < size){
					int n = readStream.Read (buffer, offset, count);
					return n;
				} else if (done)
					return 0;
					
				try {
					Console.WriteLine ("Waiting for data");
					Monitor.Wait (plock);
					Console.WriteLine ("Waking up, data available");
				} catch {
				}
			}
		}
	}

	public void Push (byte [] buffer, int offset, int count)
	{
		lock (plock){
			writeStream.Write (buffer, offset, count);
			size += count;
			Monitor.Pulse (plock);
		}
	}

	public void Done ()
	{
		lock (plock){
			Monitor.Pulse (plock);
			done = true;
		}
	}
	
	public override long Seek (long position, SeekOrigin origin)
	{
		throw new NotImplementedException ();
	}

	public override void SetLength (long newLength)
	{
		throw new NotImplementedException ();
	}

	public override void Write (byte [] buffer, int offset, int count)
	{
		throw new NotImplementedException ();
	}
	
	protected override void Dispose (bool disposing)
	{	
		if(disposing)
		{
			readStream.Close();
			readStream.Dispose();
			writeStream.Close();
			writeStream.Dispose();
		}
		base.Dispose (disposing);
	}
}
#if TEST
class Test {
	const string url = "http://ccmixter.org/content/bradstanfield/bradstanfield_-_People_Let_s_Stop_The_War.mp3";
	
	static void Main ()
	{
		var dump = File.Create ("/tmp/queued-result", 8192);
		var qs = new QueueStream ("/tmp/storage");
		byte [] buffer = new byte [8192];
		int count;

		var t = new Thread ((o) => {
			var request = (HttpWebRequest) WebRequest.Create (url);
			var response = request.GetResponse ();
			var webStream = response.GetResponseStream ();
			var webBuffer = new byte [8192];
			int c;
			
			while ((c = webStream.Read (webBuffer, 0, webBuffer.Length)) != 0){
				Console.Write (".");
				var start = DateTime.UtcNow;
				qs.Push (webBuffer, 0, c);
			}
			Console.WriteLine ("Done reading from the web");
			qs.Done ();
		});
		t.Start ();

		long total = 0;
		while ((count = qs.Read (buffer, 0, buffer.Length)) != 0){
			total += count;
			Console.WriteLine ("Slowly reading {0}, total={1}", count, total);
			dump.Write (buffer, 0, count);
			Thread.Sleep (1000);
		}
			
	}		
}
#endif