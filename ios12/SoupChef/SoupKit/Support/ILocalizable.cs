using System;

namespace SoupKit.Support
{
    public interface ILocalizable
    {
        string LocalizedString { get; }
    }

    public interface ILocalizableCurrency
    {
        string LocalizedCurrencyValue { get; }
    }
}
