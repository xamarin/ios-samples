MonkeyBrowser
============

This is an example of using Handoff in a Xamarin.iOS app. The app has four tabs that the user can use to browse the web, each with a given activity type: Weather, Favorite, Coffee Break and Work.

On any tab, when the user enters a new URL and taps the **Go** button, a new `NSUserActivity` is created for that tab that contains the URL that the user is currently browsing.

If another of the user’s devices has the **MonkeyBrowser** app installed, is signed into iCloud using the same user account, is on the same network and in close proximity to the above device, the Handoff Activity will be displayed on the home screen (in the lower left hand corner).

If the user drags upward on the Handoff icon, the app will be launched and the User Activity specified in the `NSUserActivity` will be continued on the new device.

---
**NOTE:** Testing Handoff only works on a real iOS Hardware Device and not in the iOS Simulator.

---



