using UIKit;

namespace Contacts.Helpers
{
    public static class Helper
    {
        /// <summary>
        /// The formatted name of a contact if there is one and "No Name", otherwise.
        /// </summary>
        public static void ShowAlert(this UIViewController controller, string message)
        {
            var alert = UIAlertController.Create("Status", message, UIAlertControllerStyle.ActionSheet);
            alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
            controller.PresentViewController(alert, true, null);
        }
    }
}