namespace CloudKitAtlas
{
	public class MarkNotificationsReadSample : CodeSample
	{

		//var cache = NotificationsCache ()


	public MarkNotificationsReadSample ()
			: base (
		{

			super.init (
				title: "CKMarkNotificationsReadOperation",
				className: "CKMarkNotificationsReadOperation",
				methodName: ".init(notificationIDsToMarkRead:)",
				descriptionKey: "Notifications.MarkAsRead"
			)

	}

		override func run (completionHandler: (Results, NSError!) -> Void) {
        
        let ids = cache.newNotificationIDs

		var nsError: NSError?
        
        if ids.count > 0 {
            let operation = CKMarkNotificationsReadOperation (notificationIDsToMarkRead: ids)


			operation.markNotificationsReadCompletionBlock = {
                (notificationIDsMarkedRead, operationError) in
                
                if let notificationIDs = notificationIDsMarkedRead {

					self.cache.notificationIDsToBeMarkedAsRead = notificationIDs
					completionHandler (self.cache.results, nsError)

				}

	nsError = operationError
}

operation.start()
            
        } else {

			completionHandler (self.cache.results, nsError)
        }
        
        
    }
    
}}