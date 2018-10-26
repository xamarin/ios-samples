using System;

namespace SoupKit.Support
{
    public interface ILocalizableShortcut
    {
        string ShortcutLocalizationKey { get; }
    }

    public interface ILocalizableCurrency
    {
        string LocalizedCurrencyValue { get; }
    }
}
