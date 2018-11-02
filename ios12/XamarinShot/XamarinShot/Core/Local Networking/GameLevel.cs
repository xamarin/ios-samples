
namespace XamarinShot.Models
{
    using CoreGraphics;
    using Foundation;
    using SceneKit;
    using XamarinShot.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GameLevel : IDisposable
    {
        private const string LevelsPath = "art.scnassets/levels/";
        private const string DefaultLevelName = "gateway";
        private const string LevelStrings = "levels";

        private readonly CGSize defaultSize = new CGSize(1.5f, 2.7f);

        private readonly NSLock @lock = new NSLock();

        private readonly Definition definition;

        private SCNNode levelNodeTemplate;

        private SCNNode levelNodeClone;

        private SCNScene scene;

        private GameLevel(Definition definition)
        {
            this.definition = definition;
            this.TargetSize = this.defaultSize;
        }

        public string Name => this.definition.Name;

        public string Key => this.definition.Key;

        private string Identifier => this.definition.Identifier;

        public bool Placed { get; private set; }

        /// <summary>
        /// Size of the level in meters
        /// </summary>
        public CGSize TargetSize { get; private set; }

        public float LodScale { get; private set; } = 1f;

        public string Path => LevelsPath + this.Identifier;

        public static GameLevel DefaultLevel { get; } = GameLevel.Level(DefaultLevelName);

        /// <summary>
        /// An instance of the active level
        /// </summary>
        public SCNNode ActiveLevel
        {
            get
            {
                if (this.levelNodeTemplate == null)
                {
                    return null;
                }

                if (this.levelNodeClone != null)
                {
                    return this.levelNodeClone;
                }

                this.levelNodeClone = this.levelNodeTemplate.Clone();
                return this.levelNodeClone;
            }
        }

        /// <summary>
        /// Scale factor to assign to the level to make it appear 1 unit wide.
        /// </summary>
        public float NormalizedScale
        {
            get
            {
                if (this.levelNodeTemplate == null)
                {
                    return 1f;
                }

                var levelSize = this.levelNodeTemplate.GetHorizontalSize().X;
                if (levelSize > 0)
                {
                    return 1f / levelSize;
                }
                else
                {
                    throw new Exception("Level size is 0. This might indicate something is wrong with the assets");
                }
            }
        }

        private static List<GameLevel> allLevels;
        public static List<GameLevel> AllLevels
        {
            get
            {
                if (allLevels == null)
                {
                    var url = NSBundle.MainBundle.GetUrlForResource("art.scnassets/data/levels", "json");
                    if (url == null)
                    {
                        throw new Exception("Could not find levels.json");
                    }

                    var json = NSString.FromData(NSData.FromUrl(url), NSStringEncoding.UTF8).ToString();
                    var definitions = json.Parse<List<Definition>>();
                    if (definitions != null)
                    {
                        allLevels = definitions.Select(definition => new GameLevel(definition)).ToList();
                    }
                    else
                    {
                        throw new Exception($"Could not find level information at {url}");
                    }
                }

                return allLevels;
            }
        }

        public static GameLevel Level(int index)
        {
            return index < AllLevels.Count ? AllLevels[index] : null;
        }

        public static GameLevel Level(string key)
        {
            return AllLevels.FirstOrDefault(level => level.Key == key);
        }

        public void Load()
        {
            // have to do this
            this.@lock.Lock();

            // only load once - can be called from preload on another thread, or regular load
            if (this.scene == null)
            {
                var sceneUrl = NSBundle.MainBundle.GetUrlForResource(this.Path, "scn");
                if (sceneUrl == null)
                {
                    throw new Exception($"Level {this.Path} not found");
                }

                var scene = SCNScene.FromUrl(sceneUrl, (SCNSceneLoadingOptions)null, out NSError error);
                if (error != null)
                {
                    throw new Exception($"Could not load level {sceneUrl}: {error.LocalizedDescription}");
                }

                // start with animations and physics paused until the board is placed
                // we don't want any animations or things falling over while ARSceneView
                // is driving SceneKit and the view.
                scene.Paused = true;

                // walk down the scenegraph and update the children
                scene.RootNode.FixMaterials();

                this.scene = scene;

                // this may not be the root, but lookup the identifier
                // will clone the tree done from this node
                this.levelNodeTemplate = this.scene.RootNode.FindChildNode("_" + Identifier, true);
            }

            this.@lock.Unlock();
        }

        public void Reset()
        {
            this.Placed = false;
            this.ActiveLevel.RemoveFromParentNode();
            if (this.levelNodeClone != null)
            {
                this.levelNodeClone.Dispose();
                this.levelNodeClone = null;
            }
        }

        public void PlaceLevel(SCNNode node, SCNScene gameScene, float boardScale)
        {
            if (this.ActiveLevel != null)
            {
                if (this.scene != null)
                {
                    // set the environment onto the SCNView
                    gameScene.LightingEnvironment.Contents = this.scene.LightingEnvironment.Contents;
                    gameScene.LightingEnvironment.Intensity = this.scene.LightingEnvironment.Intensity;

                    // set the cloned nodes representing the active level
                    node.AddChildNode(this.ActiveLevel);

                    this.Placed = true;

                    // the lod system doesn't honor the scaled camera,
                    // so have to fix this manually in fixLevelsOfDetail with inverse scale
                    // applied to the screenSpaceRadius
                    this.LodScale = this.NormalizedScale * boardScale;
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
            public string Key { get; set; }

            public string Identifier { get; set; }

            public string Name =>  NSBundle.MainBundle.GetLocalizedString(this.Key);
        }

        #endregion

        #region IDisposable

        private bool isDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (this.levelNodeTemplate != null)
                    {
                        this.levelNodeTemplate.Dispose();
                        this.levelNodeTemplate = null;
                    }

                    if (this.levelNodeClone != null)
                    {
                        this.levelNodeClone.Dispose();
                        this.levelNodeClone = null;
                    }

                    if (this.scene != null)
                    {
                        this.scene.Dispose();
                        this.scene = null;
                    }

                    this.@lock.Dispose();
                }

                isDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(true);
        }

        #endregion
    }
}