
namespace XamarinShot.Models.Enums;

public enum SessionState
{
        Setup,
        LookingForSurface,
        AdjustingBoard,
        PlacingBoard,
        WaitingForBoard,
        LocalizingToBoard,
        SetupLevel,
        GameInProgress,
}

public static class SessionStateExtensions
{
        public static string? LocalizedInstruction (this SessionState self)
        {
                string? result = null;
                if (!UserDefaults.DisableInGameUI)
                {
                        switch (self)
                        {
                                case SessionState.LookingForSurface:
                                        result = NSBundle.MainBundle.GetLocalizedString ("Find a flat surface to place the game.");
                                        break;
                                case SessionState.PlacingBoard:
                                        result = NSBundle.MainBundle.GetLocalizedString ("Scale, rotate or move the board.");
                                        break;
                                case SessionState.AdjustingBoard:
                                        result = NSBundle.MainBundle.GetLocalizedString ("Make adjustments and tap to continue.");
                                        break;
                                case SessionState.GameInProgress:
                                        if (!UserDefaults.HasOnboarded && !UserDefaults.Spectator)
                                        {
                                                result = NSBundle.MainBundle.GetLocalizedString ("Move closer to a slingshot.");
                                        }
                                        break;
                                case SessionState.WaitingForBoard:
                                        result = NSBundle.MainBundle.GetLocalizedString ("Synchronizing world map…");
                                        break;
                                case SessionState.LocalizingToBoard:
                                        result = NSBundle.MainBundle.GetLocalizedString ("Point the camera towards the table.");
                                        break;
                                case SessionState.SetupLevel:
                                case SessionState.Setup:
                                        result = null;
                                        break;
                        }
                }

                return result;
        }
}
