Calabash enables you to write and execute automated acceptance tests of mobile apps. It provides APIs that are specialized to interactive with native iOS and Android apps running on touch screen devices. When combined with Xamarin Test Cloud, you can automatically test your app on hundreds of mobile devices.

Calabash consists of libraries that enable test-code to programmatically interact with native and hybrid apps. The interaction consists of a number of end-user actions. Each action can be one of:

**Gestures**  

 - Touches or gestures (e.g., tap, swipe and rotate).

**Assertions**  

 - For example: there should be a "Login" button or the web view should contain an "h1" tag with the text "Hello".

**Screenshots**  

 - Screendump the current view on the current device model

### So Easy ###

It couldn't be any easier to incorporate the Calabash server into your application. All it takes is just one line of code:

```csharp
Xamarin.Calabash.Start();
```

When your app runs, it now is listening for commands issued by a test runner, such as the one used by the [Xamarin Test Cloud](http://xamarin.com/test-cloud).

### Note ###

This component currently only provides the Calabash server library, which is required to run custom tests using either the [Xamarin Test Cloud](http://xamarin.com/test-cloud) or Cucumber. For information on installing and configuring Calabash to work with a local Cucumber runner, check out [the detailed guide](https://github.com/calabash/calabash-ios/wiki/01-Getting-started-guide) in the Calabash repository on github.
