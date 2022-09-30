namespace XamarinShot.Models;

public class GameLevel : IDisposable
{
        const string LevelsPath = "art.scnassets/levels/";
        const string DefaultLevelName = "gateway";
        const string LevelStrings = "levels";

        readonly CGSize defaultSize = new CGSize (1.5f, 2.7f);

        readonly NSLock @lock = new NSLock ();

        readonly Definition definition;

        SCNNode? levelNodeTemplate;

        SCNNode? levelNodeClone;

        SCNScene? scene;

        GameLevel (Definition definition)
        {
                this.definition = definition;
                TargetSize = defaultSize;
        }

        public string? Name => definition.Name;

        public string? Key => definition.Key;

        string? Identifier => definition.Identifier;

        public bool Placed { get; private set; }

        /// <summary>
        /// Size of the level in meters
        /// </summary>
        public CGSize TargetSize { get; private set; }

        public float LodScale { get; private set; } = 1f;

        public string Path => LevelsPath + Identifier;

        public static GameLevel? DefaultLevel { get; } = GameLevel.Level (DefaultLevelName);

        /// <summary>
        /// An instance of the active level
        /// </summary>
        public SCNNode? ActiveLevel
        {
                get
                {
                        if (levelNodeTemplate is null)
                        {
                                return null;
                        }

                        if (levelNodeClone is not null)
                        {
                                return levelNodeClone;
                        }

                        return levelNodeTemplate.Clone ();
                }
        }

        /// <summary>
        /// Scale factor to assign to the level to make it appear 1 unit wide.
        /// </summary>
        public float NormalizedScale
        {
                get
                {
                        if (levelNodeTemplate is null)
                        {
                                return 1f;
                        }

                        var levelSize = levelNodeTemplate.GetHorizontalSize ().X;
                        if (levelSize > 0)
                        {
                                return 1f / levelSize;
                        }
                        else
                        {
                                throw new Exception ("Level size is 0. This might indicate something is wrong with the assets");
                        }
                }
        }

        static List<GameLevel>? allLevels;
        public static List<GameLevel> AllLevels
        {
                get
                {
                        if (allLevels is null)
                        {
                                var url = NSBundle.MainBundle.GetUrlForResource ("art.scnassets/data/levels", "json");
                                if (url is null)
                                {
                                        throw new Exception ("Could not find levels.json");
                                }

                                var json = NSString.FromData (NSData.FromUrl (url), NSStringEncoding.UTF8)?.ToString ();
                                var definitions = json?.Parse<List<Definition>> ();
                                if (definitions is not null)
                                {
                                        allLevels = definitions.Select (definition => new GameLevel (definition)).ToList ();
                                } else {
                                        throw new Exception ($"Could not find level information at {url}");
                                }
                        }

                        return allLevels;
                }
        }

        public static GameLevel? Level (int index)
        {
                return index < AllLevels.Count ? AllLevels [index] : null;
        }

        public static GameLevel? Level (string key)
        {
                return AllLevels.FirstOrDefault (level => level.Key == key);
        }

        public void Load ()
        {
                // have to do this
                @lock.Lock ();

                // only load once - can be called from preload on another thread, or regular load
                if (scene is null)
                {
                        var sceneUrl = NSBundle.MainBundle.GetUrlForResource (Path, "scn");
                        if (sceneUrl is null)
                        {
                                throw new Exception ($"Level {Path} not found");
                        }

                        var scene = SCNScene.FromUrl (sceneUrl, (SCNSceneLoadingOptions)null, out NSError error);
                        if (error is not null || scene is null)
                        {
                                throw new Exception ($"Could not load level {sceneUrl}: {error.LocalizedDescription}");
                        }

                        // start with animations and physics paused until the board is placed
                        // we don't want any animations or things falling over while ARSceneView
                        // is driving SceneKit and the view.
                        scene.Paused = true;

                        // walk down the scenegraph and update the children
                        scene.RootNode.FixMaterials ();

                        this.scene = scene;

                        // this may not be the root, but lookup the identifier
                        // will clone the tree done from this node
                        levelNodeTemplate = this.scene.RootNode.FindChildNode ("_" + Identifier, true);
                }

                @lock.Unlock ();
        }

        public void Reset ()
        {
                Placed = false;
                ActiveLevel?.RemoveFromParentNode ();
                levelNodeClone?.Dispose ();
                levelNodeClone = null;
        }

        public void PlaceLevel (SCNNode node, SCNScene gameScene, float boardScale)
        {
                if (ActiveLevel is not null)
                {
                        if (scene is not null)
                        {
                                // set the environment onto the SCNView
                                gameScene.LightingEnvironment.Contents = scene.LightingEnvironment.Contents;
                                gameScene.LightingEnvironment.Intensity = scene.LightingEnvironment.Intensity;

                                // set the cloned nodes representing the active level
                                node.AddChildNode (ActiveLevel);

                                Placed = true;

                                // the lod system doesn't honor the scaled camera,
                                // so have to fix this manually in fixLevelsOfDetail with inverse scale
                                // applied to the screenSpaceRadius
                                LodScale = NormalizedScale * boardScale;
                        }
                }
        }

        #region helpers

        public enum GameLevelKey
        {
                Gateway,
                Bridge,
                Farm,
                ArchFort,
                Towers,
        }

        class Definition
        {
                public string? Key { get; set; }

                public string? Identifier { get; set; }

                public string? Name => NSBundle.MainBundle.GetLocalizedString (Key);
        }

        #endregion

        #region IDisposable

        bool isDisposed = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing)
        {
                if (!isDisposed)
                {
                        if (disposing)
                        {
                                levelNodeTemplate?.Dispose ();
                                levelNodeTemplate = null;

                                levelNodeClone?.Dispose () ;
                                levelNodeClone = null;

                                scene?.Dispose ();
                                scene = null;

                                @lock.Dispose ();
                        }

                        isDisposed = true;
                }
        }

        public void Dispose ()
        {
                Dispose (true);
                GC.SuppressFinalize (this);
        }

        #endregion
}
