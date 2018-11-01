
namespace SimpleWatchConnectivity
{
    using CoreFoundation;
    using Foundation;
    using System;
    using System.Linq;

    // Logger is a debug utility, used to write logs into a log file.
    // WKWatchConnectivityRefreshBackgroundTask is mostly triggered when the watch app is in the background
    // and background task budget is limited, we hence can't use Xcode debugger to attach the process.
    // Mostly for debugging purpose, the class writes logs into a file. Clients thus can tranfer the log file
    // and view it on iOS side.
    public class Logger
    {
        private static Logger shared;

        private NSFileHandle fileHandle;

        private Logger()
        {
            this.fileHandle = NSFileHandle.OpenUpdateUrl(this.FileUrl, out NSError error);
            System.Diagnostics.Debug.Assert(this.fileHandle != null, "Failed to create the file handle!");
        }

        public static Logger Shared => shared ?? (shared = new Logger());

        // Return folder URL, create it if not existing yet.
        // Return nil to trigger a crash if the folder creation fails.
        // Not using lazy because we need to recreate when clearLogs is called.
        private NSUrl folderUrl;
        protected NSUrl FolderUrl
        {
            get
            {
                if (this.folderUrl == null)
                {
                    this.folderUrl = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).Last();
                    this.folderUrl = this.folderUrl.Append("Logs", true);

                    if (!NSFileManager.DefaultManager.FileExists(this.folderUrl.Path))
                    {
                        NSFileManager.DefaultManager.CreateDirectory(this.folderUrl, true, null, out NSError error);
                        if (error != null)
                        {
                            Console.WriteLine($"Failed to create the log folder: {this.folderUrl} \n{error.LocalizedDescription}");
                            this.folderUrl = null;
                        }
                    }
                }

                return this.folderUrl;
            }
        }

        // Return file URL, create it if not existing yet.
        // Return nil to trigger a crash if the file creation fails.
        // Not using lazy because we need to recreate when clearLogs is called.
        private NSUrl fileUrl;
        protected NSUrl FileUrl
        {
            get
            {
                if (this.fileUrl == null)
                {
                    var dateFormatter = new NSDateFormatter { DateStyle = NSDateFormatterStyle.Medium };
                    var dateString = dateFormatter.StringFor(new NSDate());

                    this.fileUrl = this.FolderUrl;
                    this.fileUrl = this.fileUrl.Append($"{dateString}.log", false);

                    if (!NSFileManager.DefaultManager.FileExists(this.fileUrl.Path))
                    {
                        if (!NSFileManager.DefaultManager.CreateFile(this.fileUrl.Path, NSData.FromString(string.Empty), (NSDictionary)null))
                        {
                            Console.WriteLine($"Failed to create the log file: {this.fileUrl}!");
                            this.fileUrl = null;
                        }
                    }
                }

                return this.fileUrl;
            }
        }

        #region logging

        // Avoid creating DateFormatter for time stamp as Logger may count into execution budget.
        private readonly NSDateFormatter timeStampFormatter = new NSDateFormatter { TimeStyle = NSDateFormatterStyle.Medium };

        // Use this dispatch queue to make the log file access is thread-safe.
        // Public methods use performBlockAndWait to access the resource; private methods don't.
        private readonly DispatchQueue ioQueue = new DispatchQueue("ioQueue");

        /// <summary>
        /// Get the current log file URL.
        /// </summary>
        public NSUrl GetFileURL()
        {
            return PerformBlockAndWait(() => fileUrl);
        }

        /// <summary>
        /// Append a line of text to the end of the file.
        /// Use FileHandle so that we can see to the end directly.
        /// </summary>
        public void Append(string line)
        {
            var timeStamp = this.timeStampFormatter.StringFor(new NSDate());
            var timedLine = $"{timeStamp}: {line} \n";

            var data = NSData.FromString(timedLine, NSStringEncoding.UTF8);
            using (data)
            {
                this.PerformBlockAndWait(() =>
                {
                    this.fileHandle.SeekToEndOfFile();
                    this.fileHandle.WriteData(data);
                });
            }
        }

        /// <summary>
        /// Read the file content and return it as a string.
        /// </summary>
        public string Content()
        {
            return this.PerformBlockAndWait(() =>
            {
                this.fileHandle.SeekToFileOffset(0); // Read from the very beginning.
                return NSString.FromData(this.fileHandle.AvailableData(), NSStringEncoding.UTF8);
            });
        }

        /// <summary>
        /// Clear logs. Reset the folder and file URL for later use.
        /// </summary>
        public void ClearLogs()
        {
            this.PerformBlockAndWait(() =>
            {
                this.fileHandle.CloseFile();
                this.fileHandle.Dispose();
                this.fileHandle = null;

                NSFileManager.DefaultManager.Remove(this.folderUrl, out NSError error);
                if (error != null)
                {
                    Console.WriteLine($"Failed to clear the log folder!\n{error.LocalizedDescription ?? string.Empty}");
                }

                // Create a new file handle.
                this.fileUrl = null;
                this.folderUrl = null;
                this.fileHandle = NSFileHandle.OpenUpdateUrl(this.FileUrl, out NSError urlError);
                System.Diagnostics.Debug.Assert(this.fileHandle != null, "Failed to create the file handle!");
            });
        }

        private T PerformBlockAndWait<T>(Func<T> block)
        {
            return block();
            // TODO: https://github.com/xamarin/xamarin-macios/issues/5002
            //return this.ioQueue.DispatchSync(() => return block());
        }

        private void PerformBlockAndWait(Action block)
        {
            this.ioQueue.DispatchSync(block);
        }

        #endregion
    }
}