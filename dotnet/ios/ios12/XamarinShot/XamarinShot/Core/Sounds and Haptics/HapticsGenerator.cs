namespace XamarinShot.Models;

public class HapticsGenerator : IDisposable
{
        readonly UIImpactFeedbackGenerator impact = new UIImpactFeedbackGenerator (UIImpactFeedbackStyle.Medium);

        readonly UISelectionFeedbackGenerator selection = new UISelectionFeedbackGenerator ();

        readonly UINotificationFeedbackGenerator notification = new UINotificationFeedbackGenerator ();

        public void GenerateImpactFeedback ()
        {
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                        impact.ImpactOccurred ();
                });
        }

        public void GenerateSelectionFeedback ()
        {
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                        selection.SelectionChanged ();
                });
        }

        public void GenerateNotificationFeedback (UINotificationFeedbackType notificationType)
        {
                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                        notification.NotificationOccurred (notificationType);
                });
        }

        #region IDisposable 

        bool isDisposed = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
                if (!isDisposed)
                {
                        if (disposing)
                        {
                                impact.Dispose ();
                                selection.Dispose ();
                                notification.Dispose ();
                        }

                        isDisposed = true;
                }
        }

        public void Dispose ()
        {
                Dispose (true);
                GC.SuppressFinalize (this);
        }

        #endregion
}
