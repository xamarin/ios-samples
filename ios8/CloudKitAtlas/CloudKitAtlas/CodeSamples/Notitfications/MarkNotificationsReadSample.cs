namespace CloudKitAtlas
{
	public class NotificationsCache
	{
		Results results = new Results (null, alwaysShowAsList: true);


		public void AddNotification (CKNotification notification)
		{
			results.Items.Add (notification);

		results.added.insert (results.items.count - 1)

	}

		var addedIndices: Set<Int> {
        return results.added
	}

	var newNotificationIDs: [CKNotificationID] {
        var ids = [CKNotificationID] ()
        for index in results.added {
            if let notification = results.items [index] as? CKNotification, id = notification.notificationID {
                ids.append(id)
            }
        }
        return ids
    }
    
    func markAsRead ()
{
	let notificationIDs = notificationIDsToBeMarkedAsRead

		for notificationID in notificationIDs {
		if let index = results.items.indexOf ({
			result in
                if let notification = result as? CKNotification {
				return notification.notificationID == notificationID

				} else {
				return false

				}
		}) {
		results.added.remove (index)

			}
}
UIApplication.sharedApplication().applicationIconBadgeNumber = results.added.count
    }
    
    var notificationIDsToBeMarkedAsRead: [CKNotificationID] = []
    
}

	/*
	public class MarkNotificationsReadSample : CodeSample
	{

		//var cache = NotificationsCache ()


		public MarkNotificationsReadSample ()
				: base (title: "CKMarkNotificationsReadOperation",
						className: "CKMarkNotificationsReadOperation",
						methodName: ".init(notificationIDsToMarkRead:)", // TODO: use C# name instead
						descriptionKey: "Notifications.MarkAsRead")
		{
		}

		public override void Run (Action<Results, NSError> completionHandler)
		{

			//var ids = cache.newNotificationIDs

			NSError nsError = null;
        
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
    
}
*/
}