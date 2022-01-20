using XamarinShot.Models.GameplayState;

namespace XamarinShot.Models;

public class GameObject : GKEntity
{
        static int IndexCounter = 0;

        Dictionary<string, object> properties = new Dictionary<string, object> ();

        /// <summary>
        /// call this before loading a level, all nodes loaded will share an index since nodes always load in the same order.
        /// </summary>
        public static void ResetIndexCounter ()
        {
                IndexCounter = 0;
        }

        /// <summary>
        /// Init with index that can be used to replace an old node
        /// </summary>
        public GameObject (SCNNode node, int? index, Dictionary<string, object> gamedefs, bool alive, bool server) : base ()
        {
                ObjectRootNode = node;
                IsAlive = alive;

                if (index.HasValue)
                {
                        Index = index.Value;
                }
                else
                {
                        Index = GameObject.IndexCounter;
                        GameObject.IndexCounter += 1;
                }

                GeometryNode = node.FindNodeWithGeometry ();
                PhysicsNode = node.FindNodeWithPhysicsBody ();

                // set the gameObject onto the node
                node.SetGameObject (this);
                IsServer = server;

                if (!string.IsNullOrEmpty (node.Name))
                {
                        InitGameComponents (gamedefs, node.Name);
                }
        }

        public GameObject () : base () { }

        public GameObject (NSCoder coder) => throw new NotImplementedException ("init(coder:) has not been implemented");

        public bool Categorize { get; set; }

        public string Category { get; set; } = string.Empty;

        public bool UsePredefinedPhysics { get; set; }

        public bool IsBlockObject { get; set; } = true;

        public float Density { get; set; } = 0f;

        public bool IsAlive { get; set; }

        public bool IsServer { get; set; }

        public int Index { get; private set; } = 0;

        public SCNNode? GeometryNode { get; private set; }

        public SCNNode? ObjectRootNode { get; private set; }

        public SCNNode? PhysicsNode { get; set; }

        public static T Create<T> (SCNNode node, int? index, Dictionary<string, object> gamedefs, bool alive, bool server) where T : GameObject, new()
        {
                return (Activator.CreateInstance (typeof (T), node, index, gamedefs, alive, server) as T)!;
        }

        public static T Create<T> (SCNNode node) where T : GameObject, new() { return GameObject.Create<T> (node, new Dictionary<string, object> ()); }

        public static T Create<T> (SCNNode node, Dictionary<string, object> gamedefs) where T : GameObject, new() { return GameObject.Create<T> (node, null, gamedefs, false, false); }

        #region helper for common root-level property tuning parameters

        public double? PropDouble (string name)
        {
                double? result = null;
                if (properties.TryGet (name, out object? value) && double.TryParse (value?.ToString () ?? "", out double parsed))
                {
                        result = parsed;
                }

                return result;
        }

        protected string? PropString (string name)
        {
                return properties [name]?.ToString ();
        }

        public bool PropBool (string name)
        {
                var result = default (bool);
                if (properties.TryGet (name, out object? value) && bool.TryParse (value?.ToString () ?? "", out bool parsed))
                {
                        result = parsed;
                }

                return result;
        }

        public int? PropInt (string name)
        {
                int? result = null;
                if (properties.TryGet (name, out object? value) && int.TryParse (value?.ToString () ?? "", out int parsed))
                {
                        result = parsed;
                }

                return result;
        }

        public OpenTK.Vector2? PropFloat2 (string name)
        {
                var valueString = PropString (name);
                if (!string.IsNullOrEmpty (valueString))
                {
                        var strings = valueString.Split (' ');
                        if (strings.Length >= 2)
                        {
                                if (float.TryParse (strings [0], out float x) &&
                                   float.TryParse (strings [1], out float y))
                                {
                                        return new OpenTK.Vector2 (x, y);
                                }
                        }
                }

                return null;
        }

        public SCNVector3? PropFloat3 (string name)
        {
                var valueString = PropString (name);
                if (!string.IsNullOrEmpty (valueString))
                {
                        var strings = valueString.Split (' ');
                        if (strings.Length >= 3)
                        {
                                if (float.TryParse (strings [0], out float x) &&
                                    float.TryParse (strings [1], out float y) &&
                                    float.TryParse (strings [2], out float z))
                                {
                                        return new SCNVector3 (x, y, z);
                                }
                        }
                }

                return null;
        }

        #endregion

        private void InitGameComponents (Dictionary<string, object> gamedefs, string def)
        {
                if (gamedefs.TryGetValue ("entityDefs", out object? objectValue) && objectValue is Dictionary<string, object> entityDefs)
                {
                        // always remove trailing integers from def name just in case class name is just a clone of something
                        var digits = NSCharacterSet.DecimalDigits;

                        var defPrefix = def;
                        foreach (var uni in def.Reverse ())
                        {
                                if (digits.Contains (uni))
                                {
                                        defPrefix.Remove (defPrefix.Length - 1, 1);
                                }
                                else
                                {
                                        break;
                                }
                        }

                        var baseDefinitions = new Dictionary<string, object> ();

                        // set up basedef, just in case nothing was found
                        if (entityDefs ["base"] is Dictionary<string, object> baseDictionary)
                        {
                                baseDefinitions = baseDictionary;
                                Category = "base";
                        }

                        // if we have a physics body, use that as the base def
                        if (GeometryNode is not null && PhysicsNode is not null)
                        {
                                if (entityDefs ["basePhysics"] is Dictionary<string, object> basePhysicsDictionary)
                                {
                                        baseDefinitions = basePhysicsDictionary;
                                        Category = "basePhysics";
                                }
                        }

                        // check up to the first underscore
                        var type = ObjectRootNode!.GetTypeIdentifier ();
                        if (!string.IsNullOrEmpty (type) && entityDefs.TryGetValue (type, out object? typeValue) &&
                                typeValue is Dictionary<string, object> typeDictionary)
                        {
                                baseDictionary = typeDictionary;
                                baseDefinitions = baseDictionary;
                                Category = type;
                        }

                        // check the name without the last number
                        if (entityDefs.TryGetValue (defPrefix, out object? defPrefixValue) &&
                                defPrefixValue is Dictionary<string, object> defPrefixDictionary)
                        {
                                baseDictionary = defPrefixDictionary;
                                baseDefinitions = baseDictionary;
                                Category = defPrefix;
                        }

                        // now check for the actual name
                        if (entityDefs.TryGetValue (def, out object? defValue) &&
                                defValue is Dictionary<string, object> defDictionary)
                        {
                                baseDictionary = defDictionary;
                                baseDefinitions = baseDictionary;
                                Category = def;
                        }

                        properties = baseDefinitions;
                        foreach (var (key, value) in baseDefinitions)
                        {
                                switch (key)
                                {
                                        case "smoothPhysics":
                                                SetupSmoothPhysics (value);
                                                break;

                                        case "audio":
                                                SetupAudio (value, ObjectRootNode!);
                                                break;

                                        case "properties":
                                                SetupProperties (value);
                                                break;

                                        case "slingshot":
                                                SetupSlingshot (value);
                                                break;

                                        case "resetSwitch":
                                                SetupResetSwitch (value);
                                                break;

                                        case "category":
                                                SetupCategory (value);
                                                break;

                                        case "animWaypoints":
                                                SetupWaypoints (value);
                                                break;

                                        case "constrainPhysics":
                                                SetupConstrainPhysics (value);
                                                break;

                                        case "blockObject":
                                                UpdateBlockObject (value);
                                                break;

                                        case "predefinedPhysics":
                                                UpdatePredefinedPhysics (value);
                                                break;

                                        case "density":
                                                UpdateDensity (value);
                                                break;

                                        default:
                                                //os_log(.info, "Unknown component %s", key)
                                                continue;
                                }
                        }
                }
        }

        /// <summary>
        /// help correct for hitches if needed
        /// </summary>
        void SetupSmoothPhysics (object value)
        {
                if (value is bool doSmooth && doSmooth && GeometryNode is not null && PhysicsNode is not null)
                {
                        var physicsComponent = new GamePhysicsSmoothComponent (PhysicsNode, GeometryNode);
                        AddComponent (physicsComponent);
                }
        }

        /// <summary>
        /// initialize audio features for collisions, etc
        /// </summary>
        void SetupAudio (object value, SCNNode node)
        {
                if (value is Dictionary<string, object> properties)
                {
                        AddComponent (new GameAudioComponent (node, properties));
                }
        }

        /// <summary>
        /// generic properties on this object
        /// </summary>
        void SetupProperties (object value)
        {
                if (value is Dictionary<string, object> properties)
                {
                        this.properties = properties;
                }
        }

        /// <summary>
        /// component to update the slingshot on this object, if it has one
        /// </summary>
        void SetupSlingshot (object value)
        {
                if (value is bool doSmooth && doSmooth)
                {
                        var catapultPull = ObjectRootNode!.FindChildNode ("pull", true);
                        if (catapultPull is not null)
                        {
                                AddComponent (new SlingshotComponent (catapultPull));
                        }
                }
        }

        /// <summary>
        /// special features when the object is a reset switch
        /// </summary>
        private void SetupResetSwitch (object value)
        {
                if (value is bool resetSwitch && resetSwitch)
                {
                        var leverObj = ObjectRootNode!.FindChildNode ("resetSwitch_lever", true);
                        if (leverObj is not null)
                        {
                                AddComponent (new ResetSwitchComponent (this, leverObj));
                        }
                }
        }

        /// <summary>
        /// categories let you group like objects together under a similar container
        /// </summary>
        private void SetupCategory (object value)
        {
                if (value is Dictionary<string, object> properties &&
                        properties.TryGetValue ("enabled", out object? enabled) &&
                        (bool)enabled)
                {
                        if (properties.TryGetValue ("header", out object? header))
                        {
                                var headerStr = header.ToString ();
                                if (!string.IsNullOrEmpty (headerStr))
                                {
                                        Categorize = true;
                                        Category = headerStr;
                                }
                        }
                }
        }

        void SetupWaypoints (object value)
        {
                if (value is Dictionary<string, object> properties)
                {
                        var animComponent = new AnimWaypointComponent (ObjectRootNode!, properties);
                        if (animComponent.HasWaypoints)
                        {
                                AddComponent (animComponent);
                        }
                }
        }

        void SetupConstrainPhysics (object value)
        {
                if (value is bool doConstrain && doConstrain && ObjectRootNode!.HasConstraints ())
                {
                        AddComponent (new ConstrainHierarchyComponent ());
                }
        }

        void UpdateBlockObject (object value)
        {
                if (value is bool doBlockObject)
                {
                        IsBlockObject = doBlockObject;
                }
        }

        void UpdatePredefinedPhysics (object value)
        {
                if (value is bool predefinedPhysics)
                {
                        UsePredefinedPhysics = predefinedPhysics;
                }
        }

        void UpdateDensity (object value)
        {
                if (float.TryParse (value?.ToString (), out float density))
                {
                        Density = density;
                }
        }

        /// <summary>
        /// Load the entity definitions from the specified file
        /// </summary>
        public static Dictionary<string, object> LoadGameDefs (string file)
        {
                var gameDefs = new Dictionary<string, object> ();
                var url = NSBundle.MainBundle.GetUrlForResource (file, "json");
                if (url is not null)
                {
                        // read data
                        var json = NSString.FromData (NSData.FromUrl (url), NSStringEncoding.UTF8).ToString ();

                        // strip comments out of the file with a regex
                        var commentRemovalRegex = "//.*\\n\\s*|/\\*.*?\\n?.*?\\*/\\n?\\s*";
                        json = System.Text.RegularExpressions.Regex.Replace (json, commentRemovalRegex, string.Empty);

                        // parse
                        gameDefs = json.Parse<Dictionary<string, object>> ();
                }
                if (gameDefs is null)
                        throw new Exception ("failed to parse game definition");

                // correct for inheritance
                if (gameDefs.TryGet ("entityDefs", out Dictionary<string, object>? defs))
                {
                        var newDefs = new Dictionary<string, object> ();

                        // if a def has an inheritance key, apply the inheritance
                        foreach (var (key, _) in defs)
                        {
                                if (defs.TryGet (key, out Dictionary<string, object>? def))
                                {
                                        var updated = UpdateDefInheritance (defs, def!);
                                        newDefs [key] = UpdateDictionary (updated);
                                }
                        }

                        gameDefs ["entityDefs"] = newDefs;

                        return gameDefs;

                        Dictionary<string, object> UpdateDictionary (Dictionary<string, object> dictionary)
                        {
                                var result = new Dictionary<string, object> ();

                                foreach (var (key, value) in dictionary)
                                {
                                        result [key] = dictionary.TryGet (key, out Dictionary<string, object>? parsedValue) ? UpdateDictionary (parsedValue) : value;
                                }

                                return result;
                        }
                } else {
                        return new Dictionary<string, object> ();
                }
        }

        /// <summary>
        /// Search for inheritance if available, and copy those properties over, then overwrite
        /// </summary>
        public static Dictionary<string, object> UpdateDefInheritance (Dictionary<string, object> defs, Dictionary<string, object> def)
        {
                var result = def;

                if (def.TryGet ("inherit", out string inheritProp) &&
                        !string.IsNullOrEmpty (inheritProp) &&
                        defs.TryGet (inheritProp, out Dictionary<string, object>? inheritDef))
                {
                        result = UpdateDefInheritance (defs, inheritDef);

                        // copy new keys over top
                        foreach (var (key, value) in def)
                        {
                                if (key != "inherit")
                                {
                                        result [key] = value;
                                }
                        }
                }

                return result;
        }

        #region Runtime methods

        public void Disable ()
        {
                IsAlive = false;
                PhysicsNode?.RemoveAllParticleSystems ();
                ObjectRootNode?.RemoveFromParentNode ();
                RemoveComponent (typeof (RemoveWhenFallenComponent));
        }

        public void Apply (PhysicsNodeData nodeData, bool isHalfway)
        {
                if (PhysicsNode is not null)
                {
                        // if we're not alive, avoid applying physics updates.
                        // this will allow objects on clients to get culled properly
                        if (IsAlive)
                        {
                                if (isHalfway)
                                {
                                        PhysicsNode.WorldPosition = (nodeData.Position + PhysicsNode.WorldPosition) * 0.5f;
                                        PhysicsNode.Orientation = SCNQuaternion.Slerp (PhysicsNode.Orientation, nodeData.Orientation, 0.5f);
                                } else {
                                        PhysicsNode.WorldPosition = nodeData.Position;
                                        PhysicsNode.Orientation = nodeData.Orientation;
                                }

                                if (PhysicsNode.PhysicsBody is not null)
                                {
                                        PhysicsNode.PhysicsBody.ResetTransform ();
                                        PhysicsNode.PhysicsBody.Velocity = nodeData.Velocity;
                                        PhysicsNode.PhysicsBody.AngularVelocity = nodeData.AngularVelocity;
                                }
                        }
                }
        }

        public virtual PhysicsNodeData? GeneratePhysicsData ()
        {
                return new PhysicsNodeData (PhysicsNode!, IsAlive);
        }

        #endregion
}
