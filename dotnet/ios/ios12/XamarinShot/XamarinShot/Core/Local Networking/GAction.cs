namespace XamarinShot.Models;

public class GAction
{
        public BoardSetupAction? BoardSetup { get; set; }

        public GameActionType? GameAction { get; set; }

        public PhysicsSyncData? Physics { get; set; }

        public StartGameMusicTime? StartGameMusic { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string Description
        {
                get
                {
                        if (GameAction is not null)
                        {
                                switch (GameAction.Type)
                                {
                                        case GameActionType.GActionType.GrabStart:
                                                return "grabStart";
                                        case GameActionType.GActionType.CatapultRelease:
                                                return "catapultRelease";
                                        case GameActionType.GActionType.TryGrab:
                                                return "tryGrab";
                                        case GameActionType.GActionType.GrabMove:
                                                return "grabMove";
                                        case GameActionType.GActionType.GrabbableStatus:
                                                return "grabbableStatus";
                                        case GameActionType.GActionType.HitCatapult:
                                                return "catapultKnockOut";
                                        case GameActionType.GActionType.OneHitKOAnimate:
                                                return "oneHitKOPrepareAnimation";
                                        case GameActionType.GActionType.TryRelease:
                                                return "tryRelease";
                                        case GameActionType.GActionType.LeverMove:
                                                return "levelMove";
                                        case GameActionType.GActionType.ReleaseEnd:
                                                return "releaseEnd";
                                        case GameActionType.GActionType.KnockoutSync:
                                                return "requestKnockoutSync";
                                }
                        } else if (BoardSetup is not null) {
                                switch (BoardSetup.Type)
                                {
                                        case BoardSetupAction.BoardSetupActionType.RequestBoardLocation:
                                                return "requestBoardLocation";
                                        case BoardSetupAction.BoardSetupActionType.BoardLocation:
                                                return "boardLocation";
                                }
                        } else if (Physics is not null) {
                                return "physics";
                        } else if (StartGameMusic is not null) {
                                return "startGameMusic";
                        }

                        throw new NotImplementedException ();
                }
        }
}

public class GameActionType
{
        public GrabInfo? TryGrab { get; set; }
        public GrabInfo? GrabStart { get; set; }
        public GrabInfo? GrabMove { get; set; }
        public GrabInfo? TryRelease { get; set; }
        public GrabInfo? ReleaseEnd { get; set; }

        public SlingData? CatapultRelease { get; set; }
        public GrabInfo? GrabbableStatus { get; set; }

        public HitCatapult? CatapultKnockOut { get; set; }
        public LeverMove? LeverMove { get; set; }

        public GActionType Type { get; set; }

        public enum GActionType
        {
                OneHitKOAnimate,

                TryGrab,
                GrabStart,
                GrabMove,
                TryRelease,
                ReleaseEnd,

                CatapultRelease,
                GrabbableStatus,
                KnockoutSync,
                HitCatapult,
                LeverMove,
        }
}

public class BoardSetupAction
{
        public GameBoardLocation BoardLocation { get; set; }

        public BoardSetupActionType Type { get; set; }

        public enum BoardSetupActionType
        {
                RequestBoardLocation,
                BoardLocation
        }
}

public class GameBoardLocation
{
        [Newtonsoft.Json.JsonIgnore]
        public NSData? WorldMapData { get; set; }

        public byte []? WorldMapBytes { get; set; }

        public GameBoardLocationType Type { get; set; }

        [OnSerializing]
        internal void OnSerializingMethod (StreamingContext context)
        {
                WorldMapBytes = WorldMapData?.ToArray ();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod (StreamingContext context)
        {
                WorldMapData = NSData.FromArray (WorldMapBytes!);
                WorldMapBytes = null;
        }

        public enum GameBoardLocationType
        {
                WorldMapData,
                Manual,
        }
}
