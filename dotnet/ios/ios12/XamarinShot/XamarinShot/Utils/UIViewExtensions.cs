namespace XamarinShot.Utils;

using System;
using UIKit;

/// <summary>
/// Convenience extension for animation on UIView.
/// </summary>
public static class UIViewExtensions
{
        public static void FadeInFadeOut (this UIView view, double duration)
        {
                UIView.AnimateNotify (duration, 0d, UIViewAnimationOptions.CurveEaseIn,
                        () => {
                                view.Alpha = 1f;
                        },
                        (finished) =>
                        {
                                if (finished)
                                {
                                        view.Hidden = false;
                                        UIView.Animate (duration, 1d, UIViewAnimationOptions.CurveEaseIn,
                                                () => {
                                                        view.Alpha = 0f;
                                                },
                                                () => {
                                                        view.Hidden = true;
                                                });
                                } else {
                                        view.Hidden = true;
                                }
                        });
        }
}
