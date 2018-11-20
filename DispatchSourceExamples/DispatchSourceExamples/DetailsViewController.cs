using CoreFoundation;
using DispatchSourceExamples.Models;
using Foundation;
using System;
using System.IO;
using System.Linq;
using UIKit;

namespace DispatchSourceExamples
{
    public partial class DetailsViewController : UIViewController
    {
        private const long NanosecondsInSecond = 1000000000;

        private DispatchSource dispatchSource;

        private NSUrl testFileUrl;

        private bool isActive;

        public DetailsViewController(IntPtr handle) : base(handle) { }

        public DispatchSourceItem DispatchItem { get; internal set; }

        protected NSUrl TestFileUrl
        {
            get
            {
                if (testFileUrl == null)
                {
                    var fileUrl = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).FirstOrDefault();
                    if (fileUrl != null)
                    {
                        using (fileUrl)
                        {
                            testFileUrl = fileUrl.Append("test.txt", false);
                        }
                    }
                }

                return testFileUrl;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = DispatchItem.Title;
            UpdateButton();
        }

        partial void Execute(UIButton sender)
        {
            isActive = !isActive;
            UpdateButton();

            switch (DispatchItem.Type)
            {
                case DispatchSourceType.Timer:
                    if (isActive)
                    {
                        StartTimer();
                    }
                    else
                    {
                        CancelDispatchSource();
                    }
                    break;

                case DispatchSourceType.Vnode:
                    if (isActive)
                    {
                        StartVnodeMonitor();
                    }
                    else
                    {
                        TestVnodeMonitor();
                    }
                    break;

                case DispatchSourceType.MemoryPressure:
                    if (isActive)
                    {
                        StartMemoryMonitor();
                    }
                    else
                    {
                        TestMemoryMonitor();
                    }
                    break;

                case DispatchSourceType.ReadMonitor:
                    TestReadMonitor();
                    break;

                case DispatchSourceType.WriteMonitor:
                    StartWriteMonitor();
                    break;
            }
        }

        private void UpdateButton()
        {
            var title = string.Empty;
            switch (DispatchItem.Type)
            {
                case DispatchSourceType.Timer:
                    title = isActive ? "Stop Timer" : "Start Timer";
                    break;
                case DispatchSourceType.Vnode:
                    title = isActive ? "Test Vnode Monitor" : "Start Vnode Monitor";
                    break;
                case DispatchSourceType.MemoryPressure:
                    title = isActive ? "Test Memory Monitor" : "Start Memory Monitor";
                    break;
                case DispatchSourceType.ReadMonitor:
                    title = "Test Read Monitor";
                    break;
                case DispatchSourceType.WriteMonitor:
                    title = "Test Write Monitor";
                    break;
            }

            actionButton.SetTitle(title, UIControlState.Normal);
        }

        #region Actions

        #region Timer

        private void StartTimer()
        {
            var dispatchSourceTimer = new DispatchSource.Timer(DispatchQueue.MainQueue);

            var delay = 2 * NanosecondsInSecond;
            var leeway = 5 * NanosecondsInSecond;

            dispatchSourceTimer.SetTimer(DispatchTime.Now, delay, leeway);
            dispatchSourceTimer.SetRegistrationHandler(() => PrintResult("Timer registered"));
            dispatchSourceTimer.SetEventHandler(() => PrintResult("Timer tick"));
            dispatchSourceTimer.SetCancelHandler(() => PrintResult("Timer stopped"));

            dispatchSource = dispatchSourceTimer;
            dispatchSource.Resume();
        }

        #endregion

        #region Vnode

        private void StartVnodeMonitor()
        {
            var fileUrl = TestFileUrl;

            var testFileStream = File.Create(fileUrl.Path);
            int fileDescriptor = GetFileDescriptor(testFileStream);

            var dispatchSourceMonitor = new DispatchSource.VnodeMonitor(fileDescriptor,
                                                                        VnodeMonitorKind.Delete | VnodeMonitorKind.Extend | VnodeMonitorKind.Write,
                                                                        DispatchQueue.MainQueue);

            dispatchSourceMonitor.SetRegistrationHandler(() => PrintResult("Vnode monitor registered"));
            dispatchSourceMonitor.SetEventHandler(() =>
            {
                var observedEvents = dispatchSourceMonitor.ObservedEvents;
                var message = $"Vnode monitor event for {fileUrl.LastPathComponent}: {observedEvents}";
                PrintResult(message);

                CancelDispatchSource();
                testFileStream.Close();
            });

            dispatchSourceMonitor.SetCancelHandler(() => PrintResult("Vnode monitor cancelled"));

            dispatchSource = dispatchSourceMonitor;
            dispatchSource.Resume();
        }

        private void TestVnodeMonitor()
        {
            File.Delete(TestFileUrl.Path);
        }

        #endregion

        #region Memory Monitor

        private void StartMemoryMonitor()
        {
            var dispatchSourcePressure = new DispatchSource.MemoryPressure(MemoryPressureFlags.Critical | MemoryPressureFlags.Warn | MemoryPressureFlags.Normal,
                                                                           DispatchQueue.MainQueue);

            dispatchSourcePressure.SetRegistrationHandler(() => PrintResult("Memory monitor registered"));
            dispatchSourcePressure.SetEventHandler(() =>
            {
                var pressureLevel = dispatchSourcePressure.PressureFlags;
                PrintResult($"Memory worning of level: {pressureLevel}");
                CancelDispatchSource();
            });

            dispatchSourcePressure.SetCancelHandler(() => PrintResult("Memory monitor cancelled"));

            dispatchSource = dispatchSourcePressure;
            dispatchSource.Resume();
        }

        private void TestMemoryMonitor()
        {
            if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR)
            {
                PrintResult("Press: Debug -> Simulate Memory Warning");
            }
            else
            {
                PrintResult("This test available on simulator only");
                CancelDispatchSource();
            }
        }

        #endregion

        private void TestReadMonitor()
        {
            var fileUrl = TestFileUrl;
            int fileDescriptor = 0;

            using (var stream = new FileStream(fileUrl.Path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
            {
                fileDescriptor = GetFileDescriptor(stream);
            }

            dispatchSource = new DispatchSource.ReadMonitor(fileDescriptor, DispatchQueue.MainQueue);
            dispatchSource.SetRegistrationHandler(() => PrintResult("Read monitor registered"));
            dispatchSource.SetEventHandler(() =>
            {
                PrintResult($"Read monitor: {fileUrl.LastPathComponent} was opened in read mode");
                CancelDispatchSource();
            });

            dispatchSource.SetCancelHandler(() => PrintResult("Read monitor cancelled"));
            dispatchSource.Resume();
        }

        private void StartWriteMonitor()
        {
            var fileUrl = TestFileUrl;
            var stream = new FileStream(fileUrl.Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            int fileDescriptor = GetFileDescriptor(stream);

            dispatchSource = new DispatchSource.WriteMonitor(fileDescriptor, DispatchQueue.MainQueue);
            dispatchSource.SetRegistrationHandler(() => PrintResult("Write monitor registered"));
            dispatchSource.SetEventHandler(() =>
            {
                PrintResult($"Write monitor: {fileUrl.LastPathComponent} was opened in write mode");
                CancelDispatchSource();

                stream.Dispose();
                stream = null;
            });

            dispatchSource.SetCancelHandler(() => PrintResult("Write monitor cancelled"));
            dispatchSource.Resume();
        }

        private void PrintResult(string message)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                textView.Text = $"{textView.Text}\n{message}";
                textView.ScrollRangeToVisible(new NSRange(0, textView.Text.Length));
            });
        }

        private int GetFileDescriptor(FileStream stream)
        {
            var safeHandle = stream.SafeFileHandle;
            return safeHandle.DangerousGetHandle().ToInt32();
        }

        private void CancelDispatchSource()
        {
            if (dispatchSource != null)
            {
                dispatchSource.Cancel();
                dispatchSource.Dispose();
                dispatchSource = null;
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (testFileUrl != null)
            {
                testFileUrl.Dispose();
                testFileUrl = null;
            }

            if (dispatchSource != null)
            {
                dispatchSource.Dispose();
                dispatchSource = null;
            }
        }
    }
}