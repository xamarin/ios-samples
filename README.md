# iOS/tvOS/watchOS Samples

The samples in this directory use the Xamarin.iOS toolchain and
Xamarin project files to illustrate the basics of how to
work with iOS and the SDK. Visit the [iOS Sample Gallery](https://docs.microsoft.com/samples/browse/?term=Xamarin.iOS)
to download individual samples.

## License

See the repo's [license file](LICENSE) and any additional license information attached to each sample.

## Samples Layout

### Version Specific Samples

Starting with iOS 8, we started adding samples that showcase specific
features introduced in a release into a directory for the release.  So
all the new iOS 8 feature samples live in the ios8 directory, the
iOS 9 features live in the ios9 directory and the iOS 10 feature lives
in the ios10 directory, and so on for ios11, ios12, ios13...

### watchOS

watchOS samples are in the watchOS directory, including:

- [watchOS/WatchKitCatalog](https://github.com/xamarin/ios-samples/tree/master/watchOS/WatchKitCatalog): contains a sample that shows all the UI elements available in WatchKit.
- [watchOS/ActivityRings](https://github.com/xamarin/ios-samples/tree/master/watchOS/ActivityRings): Health Kit integration for Apple Watch.
- [watchOS/WatchTables](https://github.com/xamarin/ios-samples/tree/master/watchOS/WatchTables): the sample app used in our tutorials to show how to build WatchKit apps.
- [and more...](https://github.com/xamarin/ios-samples/tree/master/watchOS)

> NOTE: The [WatchKit samples](https://github.com/xamarin/ios-samples/tree/master/WatchKit) are deprecated, please refer to these samples in the **watchOS** directory.

### tvOS

tvOS samples are in the tvos directory, including the [tvOS UI Catalog](https://github.com/xamarin/ios-samples/tree/master/tvos/UICatalog).

## Contributing

Before adding a sample to the repository, please run either install-hook.bat
or install-hook.sh depending on whether you're on Windows or a Posix system.
This will install a Git hook that runs the Xamarin code sample validator before
a commit, to ensure that all samples are good to go.

## Samples Submission Guidelines

### Galleries

We love samples! Application samples show off our platform and provide a great way for people to learn our stuff. And we even promote them as a first-class feature of the docs site. You can find the sample galleries here:

- [Xamarin.Forms Samples](https://docs.microsoft.com/samples/browse/?term=Xamarin.Forms)

- [iOS Samples](https://docs.microsoft.com/samples/browse/?term=Xamarin.iOS)

- [Mac Samples](https://docs.microsoft.com/samples/browse/?term=Xamarin.Mac)

- [Android Samples](https://docs.microsoft.com/samples/browse/?term=Xamarin.Android)

## Sample GitHub Repositories

These sample galleries are populated by samples in these GitHub repos:

- [https://github.com/xamarin/xamarin-forms-samples](https://github.com/xamarin/xamarin-forms-samples)

- [https://github.com/xamarin/mobile-samples](https://github.com/xamarin/mobile-samples)

- [https://github.com/xamarin/monotouch-samples](https://github.com/xamarin/ios-samples)

- [https://github.com/xamarin/mac-samples](https://github.com/xamarin/mac-samples)

- [https://github.com/xamarin/monodroid-samples](https://github.com/xamarin/monodroid-samples)

- [https://github.com/xamarin/mac-ios-samples](https://github.com/xamarin/mac-ios-samples)

The [mobile-samples](https://github.com/xamarin/mobile-samples) repository is for samples that are cross-platform.
The [mac-ios-samples](https://github.com/xamarin/mac-ios-samples) repository is for samples that are Mac/iOS only.

## Sample Requirements

We welcome sample submissions, please start by creating an issue with your proposal.

Because the sample galleries are powered by the github sample repos, each sample needs to have the following things:

- **Screenshots** - a folder called Screenshots that has at least one screen shot of the sample on each platform (preferably a screen shot for every page or every major piece of functionality). For an example of this, see [ios11/MapKitSample](https://github.com/xamarin/ios-samples/tree/master/ios11/MapKitSample/Screenshots).

- **Readme** - a `README.md` file that explains the sample, and contains metadata to help customers find it. For an example of this, see [ios11/MapKitSample](https://github.com/xamarin/ios-samples/tree/master/ios11/MapKitSample/README.md). The README file should begin with a YAML header (delimited by `---`) with the following keys/values:

  - **name** - must begin with `Xamarin.iOS -`

    - **description** - brief description of the sample (&lt; 150 chars) that appears in the sample code browser search

    - **page_type** - must be the string `sample`.

    - **languages** - coding language/s used in the sample, such as: `csharp`, `fsharp`, `vb`, `objc`

    - **products**: should be `xamarin` for every sample in this repo

    - **urlFragment**: although this can be auto-generated, please supply an all-lowercase value that represents the sample's path in this repo, except directory separators are replaced with dashes (`-`) and no other punctuation.

    Here is a working example from [_ios11/MapKitSample_ README raw view](https://raw.githubusercontent.com/xamarin/ios-samples/master/ios11/MapKitSample/README.md).

    ```yaml
    ---
    name: Xamarin.iOS - MapKit Sample
    description: "Demo of new iOS 11 features in MapKit, grouping and splitting markers based on zoom-level. Tandm is a fictional bike sharing... (iOS11)"
    page_type: sample
    languages:
    - csharp
    products:
    - xamarin
    urlFragment: ios11-mapkitsample
    ---
    # Heading 1

    rest of README goes here, including screenshot images and requirements/instructions to get it running
    ```

    > NOTE: This must be valid YAML, so some characters in the name or description will require the entire string to be surrounded by " or ' quotes.

- **Buildable solution and .csproj file** - the project _must_ build and have the appropriate project scaffolding (solution + .csproj files).

This approach ensures that all samples integrate with the Microsoft [sample code browser](https://docs.microsoft.com/samples/browse/?term=Xamarin.iOS).

A good example of this stuff is here in the iOS 11 maps sample: [https://github.com/xamarin/ios-samples/tree/master/ios11/MapKitSample](https://github.com/xamarin/ios-samples/tree/master/ios11/MapKitSample)

For a cross-platform sample, please see: https://github.com/xamarin/mobile-samples/tree/master/Tasky

## GitHub Integration

We integrate tightly with Git to make sure we always provide working samples to our customers. This is achieved through a pre-commit hook that runs before your commit goes through, as well as a post-receive hook on GitHub's end that notifies our samples gallery server when changes go through.

To you, as a sample committer, this means that before you push to the repos, you should run the "install-hook.bat" or "install-hook.sh" (depending on whether you're on Windows or macOS/Linux, respectively). These will install the Git pre-commit hook. Now, whenever you try to make a Git commit, all samples in the repo will be validated. If any sample fails to validate, the commit is aborted; otherwise, your commit goes through and you can go ahead and push.

This strict approach is put in place to ensure that the samples we present to our customers are always in a good state, and to ensure that all samples integrate correctly with the sample gallery (README.md, Metadata.xml, etc). Note that the master branch of each sample repo is what we present to our customers for our stable releases, so they must *always* Just Work.

Should you wish to invoke validation of samples manually, simply run "validate.windows" or "validate.posix" (again, Windows vs macOS/Linux, respectively). These must be run from a Bash shell (i.e. a terminal on macOS/Linux or the Git Bash terminal on Windows).

If you have any questions, don't hesitate to ask on [Xamarin Forums](https://forums.xamarin.com/categories/ios)!
