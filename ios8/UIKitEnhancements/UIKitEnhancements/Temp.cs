// Create a Accept action
UIMutableUserNotificationAction acceptAction = new UIMutableUserNotificationAction (){
	Identifier = "ACCEPT_ID",
	Title = "Accept",
	ActivationMode = UIUserNotificationActivationMode.Background,
	Destructive = false,
	AuthenticationRequired = false
};

// Create a Reply action
UIMutableUserNotificationAction replyAction = new UIMutableUserNotificationAction () {
	Identifier = "REPLY_ID",
	Title = "Reply",
	ActivationMode = UIUserNotificationActivationMode.Foreground,
	Destructive = false,
	AuthenticationRequired = true
};

// Create a Trash action
UIMutableUserNotificationAction trashAction = new UIMutableUserNotificationAction (){
	Identifier = "TRASH_ID",
	Title = "Trash",
	ActivationMode = UIUserNotificationActivationMode.Background,
	Destructive = true,
	AuthenticationRequired = true
};