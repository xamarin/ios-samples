
namespace SoupChef.Support
{
    using System;

    /// <summary>
    /// A type with a localized string that will load the appropriate localized value for a shortcut.
    /// </summary>
    public interface ILocalizableShortcut
    {
        string ShortcutLocalizationKey { get; }
    }

    /// <summary>
    /// A type with a localized currency string that is appropiate to display in UI.
    /// </summary>
    public interface ILocalizableCurrency
    {
        string LocalizedCurrencyValue { get; }
    }
}