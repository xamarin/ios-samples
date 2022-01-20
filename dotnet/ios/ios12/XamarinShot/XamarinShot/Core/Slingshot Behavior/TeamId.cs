namespace SwiftShot.Models;

public enum TeamId
{
        None = 0, // default

        Blue,

        Yellow,
}

public static class TeamIdExtensions
{
        public static UIColor GetColor (this TeamId teamId)
        {
                switch (teamId)
                {
                        case TeamId.None:
                                return UIColor.White;
                        case TeamId.Blue:
                                return UIColor.FromRGB (45, 128, 208);// srgb
                        case TeamId.Yellow:
                                return UIColor.FromRGB (239, 153, 55);
                }

                throw new System.NotSupportedException ();
        }
}
