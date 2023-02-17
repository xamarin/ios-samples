
namespace XamarinShot.Utils {
	using Foundation;
	using SceneKit;
	using XamarinShot.Models;
	using XamarinShot.Models.GameplayState;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIKit;

	public static class SCNNodeExtensions {
		#region game object

		private static Dictionary<SCNNode, GameObject> map = new Dictionary<SCNNode, GameObject> ();

		public static GameObject GetGameObject (this SCNNode node)
		{
			map.TryGetValue (node, out GameObject @object);
			return @object;
		}

		public static void SetGameObject (this SCNNode node, GameObject value)
		{
			if (!map.ContainsKey (node)) {
				map.Add (node, value);
			}
		}

		#endregion

		#region fix materials

		public static void FixMaterials (this SCNNode node)
		{
			// walk down the scenegraph and update all children
			node.FixNormalMaps ();

			// establish paint colors
			node.CopyGeometryForPaintColors ();
			node.SetPaintColors ();
		}

		private static void FixNormalMap (this SCNGeometry geometry)
		{
			foreach (var material in geometry.Materials) {
				var prop = material.Normal;
				// astc needs gggr and .ag to compress to L+A,
				//   but will compress rg01 to RGB single plane (less quality)
				// bc/eac/explicit/no compression use rg01 and .rg
				//   rg01 is easier to see and edit in texture editors

				// set the normal to RED | GREEN (rg01 compression)
				// uses single plane on ASTC, dual on BC5/EAC_RG11/Explicit
				prop.TextureComponents = SCNColorMask.Red | SCNColorMask.Green;

				// set the normal to ALPHA | GREEN (gggr compression)
				// uses dual plane for ASTC, BC3nm
				// prop.textureComponents = [.alpha, .green]
			}
		}

		/// <summary>
		/// Fix the normal map reconstruction on scn files for compressed textures
		/// </summary>
		public static void FixNormalMaps (this SCNNode node)
		{
			if (node.Geometry != null) {
				node.Geometry.FixNormalMap ();

				// these will often just have the same material applied
				if (node.Geometry.LevelsOfDetail != null) {
					foreach (var lod in node.Geometry.LevelsOfDetail) {
						if (lod.Geometry != null) {
							lod.Geometry.FixNormalMap ();
						}
					}
				}
			}

			foreach (var child in node.ChildNodes) {
				child.FixNormalMaps ();
			}
		}

		// We load all nodes as references which means they share the same
		// geometry and materials.  For team colors, we need to set geometry overrides
		// and so need unique geometry with shadable overrides for each node created.

		public static void CopyGeometryForPaintColors (this SCNNode node)
		{
			// neutral blocks also need to be tinted
			if (node.Geometry != null && !string.IsNullOrEmpty (node.Name)) {
				// does this copy the LOD as well ?
				if (node.Geometry.Copy () is SCNGeometry geometryCopy) {
					SetupPaintColorMask (geometryCopy, node.Name);

					// this may already done by the copy() above, but just be safe
					if (node.Geometry.LevelsOfDetail != null) {
						var lodsNew = new List<SCNLevelOfDetail> ();
						foreach (var lod in node.Geometry.LevelsOfDetail) {
							if (lod.Geometry != null) {
								if (lod.ScreenSpaceRadius > 0) {
									if (lod.Geometry.Copy () is SCNGeometry lodGeometryCopy) {
										lodGeometryCopy.SetupPaintColorMask (node.Name);
										lodsNew.Add (SCNLevelOfDetail.CreateWithScreenSpaceRadius (lodGeometryCopy, lod.ScreenSpaceRadius));
									}
								} else {
									if (lod.Geometry.Copy () is SCNGeometry lodGeometryCopy) {
										SetupPaintColorMask (lodGeometryCopy, node.Name);
										lodsNew.Add (SCNLevelOfDetail.CreateWithWorldSpaceDistance (lodGeometryCopy, lod.WorldSpaceDistance));
									}
								}
							}
						}

						geometryCopy.LevelsOfDetail = lodsNew.ToArray ();
					}

					// set the new geometry and LOD
					node.UpdateGeometry (geometryCopy);
				}
			}

			foreach (var child in node.ChildNodes) {
				child.CopyGeometryForPaintColors ();
			}
		}

		public static void UpdateGeometry (this SCNNode node, SCNGeometry geometry)
		{
			var oldGeometry = node.Geometry;
			node.Geometry = geometry;

			oldGeometry.Dispose ();
			oldGeometry = null;
		}

		#endregion

		public static string GetTypeIdentifier (this SCNNode node)
		{
			string result = null;
			if (!string.IsNullOrEmpty (node.Name) && !node.Name.StartsWith ("_", StringComparison.Ordinal)) {
				result = node.Name.Split ('_').FirstOrDefault ();
			}

			return result;
		}

		public static GameObject NearestParentGameObject (this SCNNode node)
		{
			var result = GetGameObject (node);
			if (result == null) {
				if (node.ParentNode != null) {
					node.ParentNode.NearestParentGameObject ();
				}
			}

			return result;
		}

		// Returns the size of the horizontal parts of the node's bounding box.
		// x is the width, y is the depth.
		public static OpenTK.Vector2 GetHorizontalSize (this SCNNode node)
		{
			var minBox = SCNVector3.Zero;
			var maxBox = SCNVector3.Zero;
			node.GetBoundingBox (ref minBox, ref maxBox);

			// Scene is y-up, horizontal extent is calculated on x and z
			var sceneWidth = Math.Abs (maxBox.X - minBox.X);
			var sceneLength = Math.Abs (maxBox.Z - minBox.Z);
			return new OpenTK.Vector2 (sceneWidth, sceneLength);
		}

		#region find node

		public static SCNNode FindNodeWithPhysicsBody (this SCNNode node)
		{
			return FindNodeWithPhysicsBodyHelper (node);
		}

		public static SCNNode FindNodeWithGeometry (this SCNNode node)
		{
			return FindNodeWithGeometryHelper (node);
		}

		private static SCNNode FindNodeWithPhysicsBodyHelper (SCNNode node)
		{
			SCNNode result = null;
			if (node.PhysicsBody != null) {
				result = node;
			} else {
				foreach (var child in node.ChildNodes) {
					if (ShouldContinueSpecialNodeSearch (child)) {
						var childWithPhysicsBody = FindNodeWithPhysicsBodyHelper (child);
						if (childWithPhysicsBody != null) {
							result = childWithPhysicsBody;
						}
					}
				}
			}

			return result;
		}

		private static SCNNode FindNodeWithGeometryHelper (SCNNode node)
		{
			SCNNode result = null;
			if (node.Geometry != null) {
				result = node;
			} else {
				foreach (var child in node.ChildNodes) {
					if (ShouldContinueSpecialNodeSearch (child)) {
						var childWithGeosBody = FindNodeWithGeometryHelper (child);
						if (childWithGeosBody != null) {
							result = childWithGeosBody;
						}
					}
				}
			}

			return result;
		}

		private static bool ShouldContinueSpecialNodeSearch (SCNNode node)
		{
			bool result = true;

			// end geo + physics search when a system collection is found
			var value = node.ValueForKey (new NSString ("isEndpoint"));
			if (value != null) {
				if (Convert.ToBoolean (value)) {
					result = false;
				}

			}

			return result;
		}

		#endregion

		public static SCNNode ParentWithPrefix (this SCNNode node, string prefix)
		{
			SCNNode result = null;
			if (!string.IsNullOrEmpty (node.Name) && node.Name.StartsWith (prefix, StringComparison.Ordinal)) {
				result = node;
			} else if (node.ParentNode != null) {
				result = node.ParentNode.ParentWithPrefix (prefix);
			}

			return result;
		}

		public static bool HasConstraints (this SCNNode node)
		{
			var balljoints = FindAllJoints (node, ConstrainHierarchyComponent.JointName);
			var hingeJoints = FindAllJoints (node, ConstrainHierarchyComponent.HingeName);
			return balljoints.Any () && hingeJoints.Any ();
		}

		public static List<SCNNode> FindAllJoints (this SCNNode node, string prefix)
		{
			var result = new List<SCNNode> ();

			var physicsNode = FindNodeWithPhysicsBody (node);
			if (physicsNode != null) {
				// ball joints have the correct prefix and are first generation children of entity node
				foreach (var child in physicsNode.ChildNodes) {
					if (!string.IsNullOrEmpty (child.Name) && child.Name.StartsWith (prefix, StringComparison.Ordinal)) {
						result.Add (child);
					}
				}
			}

			return result;
		}

		public static Team GetTeam (this SCNNode node)
		{
			var parent = node.ParentNode;
			while (parent != null) {
				if (parent.Name == "_teamA") {
					return Team.TeamA;
				} else if (parent.Name == "_teamB") {
					return Team.TeamB;
				}

				parent = parent.ParentNode;
			}

			return Team.None;
		}

		#region paint mask

		private static readonly string PaintMaskColorKey = "paintMaskColor";

		/// <summary>
		/// Recursively set team color into any nodes that use it
		/// </summary>
		public static void SetPaintColors (this SCNNode node)
		{
			var geometry = node?.Geometry;
			if (geometry != null) {
				// paintColor can be UIColor or SCNVector4
				var paintColor = node.GetTeam ().GetColor ();

				if (geometry.HasUniform (PaintMaskColorKey)) {
					geometry.SetColor (PaintMaskColorKey, paintColor);
				}

				var lods = geometry.LevelsOfDetail;
				if (lods != null) {
					foreach (var lod in lods) {
						if (lod?.Geometry != null && lod.Geometry.HasUniform (PaintMaskColorKey)) {
							lod.Geometry.SetColor (PaintMaskColorKey, paintColor);
						}
					}
				}
			}

			foreach (var child in node.ChildNodes) {
				child.SetPaintColors ();
			}
		}

		#endregion

		#region copy

		/// <summary>
		/// must copy geometry and materials to set unique data on both here we only want
		/// </summary>
		public static SCNGeometry CopyGeometryAndMaterials (this SCNGeometry geometry)
		{
			var result = geometry;
			if (geometry.Copy () is SCNGeometry geometryCopy) {
				var materialsCopy = new List<SCNMaterial> ();
				foreach (var material in geometryCopy.Materials) {
					if (material.Copy () is SCNMaterial materialCopy) {
						materialsCopy.Add (materialCopy);
					}
				}

				geometryCopy.Materials = materialsCopy.ToArray ();

				result = geometryCopy;
			}

			return result;
		}

		public static void CopyGeometryAndMaterials (this SCNNode node)
		{
			// this copies the material, but not the lod
			if (node.Geometry != null) {
				node.UpdateGeometry (node.Geometry.CopyGeometryAndMaterials ());

				if (node.Geometry.LevelsOfDetail != null) {
					var lodsNew = new List<SCNLevelOfDetail> ();
					foreach (var lod in node.Geometry.LevelsOfDetail) {
						if (lod.Geometry != null) {
							var lodGeometryCopy = lod.Geometry.CopyGeometryAndMaterials ();
							lodsNew.Add (SCNLevelOfDetail.CreateWithScreenSpaceRadius (lodGeometryCopy, lod.ScreenSpaceRadius));
						}
					}

					node.Geometry.LevelsOfDetail = lodsNew.ToArray ();
				}
			}

			foreach (var child in node.ChildNodes) {
				child.CopyGeometryAndMaterials ();
			}
		}

		#endregion

		/// <summary>
		/// For lod, set the radius based on a percentage of the screen size
		/// </summary>
		public static float ComputeScreenSpaceRadius (float screenSpacePercent)
		{
			// Needs adjusted for screen size, and original bounds of shape
			// eventually also look at physical size of device
			var screenSize = UIScreen.MainScreen.NativeBounds; // main.bounds
			var screenWidth = screenSize.Width;
			var screenHeight = screenSize.Height;
			var minDimension = (float) (Math.Min (screenWidth, screenHeight));

			// work in percentages of screen area for low/high-res devices
			// 10% of screen size
			var screenSpaceDiameter = screenSpacePercent * minDimension;
			return screenSpaceDiameter * 0.5f;
		}

		public static void FixLevelsOfDetail (this SCNNode node, float screenSpaceRadius, bool showLOD)
		{
			// find the boundingRadius of the node, and scale to that
			var lods = node?.Geometry?.LevelsOfDetail;
			if (lods != null) {
				var lodsNew = new List<SCNLevelOfDetail> ();
				foreach (var lod in lods) {
					if (lod.Geometry != null) {
						if (showLOD) {
							// visualize the lod
							var lodGeometryCopy = lod.Geometry.CopyGeometryAndMaterials ();

							// override the emission
							// this is not removed currently
							foreach (var material in lodGeometryCopy.Materials) {
								material.Emission.Contents = UIColor.Red;
							}

							lodsNew.Add (SCNLevelOfDetail.CreateWithScreenSpaceRadius (lodGeometryCopy, screenSpaceRadius));
						} else {
							lodsNew.Add (SCNLevelOfDetail.CreateWithScreenSpaceRadius (lod.Geometry, screenSpaceRadius));
						}
					}
				}

				node.Geometry.LevelsOfDetail = lodsNew.ToArray ();
			}

			foreach (var child in node.ChildNodes) {
				child.FixLevelsOfDetail (screenSpaceRadius, showLOD);
			}
		}

		public static void SetNodeToOccluder (this SCNNode node)
		{
			var material = SCNMaterialExtensions.Create (UIColor.White);
			material.ColorBufferWriteMask = SCNColorMask.None;
			material.WritesToDepthBuffer = true;

			if (node.Geometry == null) {
				throw new Exception ("Node has no geometry");
			}

			node.Geometry.Materials = new SCNMaterial [] { material };
			node.RenderingOrder = -10;
			node.CastsShadow = false;
		}

		public static SCNNode LoadSCNAsset (string modelFileName)
		{
			var assetPaths = new string []
			{
				"art.scnassets/models/",
				"art.scnassets/blocks/",
				"art.scnassets/projectiles/",
				"art.scnassets/catapults/",
				"art.scnassets/levels/",
				"art.scnassets/effects/"
			};

			var assetExtensions = new string [] { "scn", "scnp" };

			SCNReferenceNode nodeRefSearch = null;
			foreach (var path in assetPaths) {
				foreach (var ext in assetExtensions) {
					var url = NSBundle.MainBundle.GetUrlForResource (path + modelFileName, ext);
					if (url != null) {
						nodeRefSearch = SCNReferenceNode.CreateFromUrl (url);
						if (nodeRefSearch != null) {
							break;
						}
					}
				}

				if (nodeRefSearch != null) {
					break;
				}
			}

			if (nodeRefSearch != null) {
				// this does the load, default policy is load immediate
				nodeRefSearch.Load ();

				// log an error if geo not nested under a physics shape
				var node = nodeRefSearch.ChildNodes.FirstOrDefault ();
				if (node == null) {
					throw new Exception ($"model {modelFileName} has no child nodes");
				}

				if (nodeRefSearch.ChildNodes.Count () > 1) {
					//os_log(.error, "model %s should have a single root node", modelFileName)
				}

				// walk down the scenegraph and update all children
				node.FixMaterials ();

				return node;
			} else {
				throw new Exception ($"couldn't load {modelFileName}");
			}
		}

		public static void SetPaintColors (this SCNNode node, Team team)
		{
			if (node.Geometry != null) {
				// paintColor can be UIColor or SCNVector4
				var paintColor = team.GetColor ();

				if (node.Geometry.HasUniform (PaintMaskColorKey)) {
					node.Geometry.SetColor (PaintMaskColorKey, paintColor);
				}

				if (node.Geometry.LevelsOfDetail != null) {
					foreach (var lod in node.Geometry.LevelsOfDetail) {
						if (lod.Geometry != null && lod.Geometry.HasUniform (PaintMaskColorKey)) {
							lod.Geometry.SetColor (PaintMaskColorKey, paintColor);
						}
					}
				}
			}

			foreach (var child in node.ChildNodes) {
				child.SetPaintColors (team);
			}
		}

		#region paint color mask

		/// <summary>
		/// until pipeline is ready, use a map of material name to paintMask texture
		/// </summary>
		private static Dictionary<string, string> PaintColorMaskTextures = new Dictionary<string, string>
		{
			{"geom_block_boxB", "block_boxBMaterial_PaintMask"},
			{"geom_block_cylinderC", "block_cylinderCMaterial_Paintmask"},
			{"geom_block_halfCylinderA", "block_halfCylinderAMaterial_Paintmask"},
			{"flag_flagA", "flag_flagAMaterial_PaintMask"},
			{"catapultBase", "catapultBase_PaintMask"},
			{"catapultProngs", "catapultBase_PaintMask"},
			{"catapultStrap", "catapultSling_PaintMask"},
			{"catapultStrapInactive", "catapultSling_PaintMask"},

			{"V", "letters_lettersMaterial_PaintMask"},
			{"I", "letters_lettersMaterial_PaintMask"},
			{"C", "letters_lettersMaterial_PaintMask"},
			{"T", "letters_lettersMaterial_PaintMask"},
			{"O", "letters_lettersMaterial_PaintMask"},
			{"R", "letters_lettersMaterial_PaintMask"},
			{"Y", "letters_lettersMaterial_PaintMask"},
			{"ExclamationPoint", "letters_lettersMaterial_PaintMask"}
		};

		public static void SetupPaintColorMask (this SCNGeometry geometry, string name)
		{
			// if we've already set it up, don't do it again
			if (geometry.ValueForKey (new NSString (PaintMaskColorKey)) == null) {
				if (PaintColorMaskTextures.TryGetValue (name, out string paintMask)) {
					// all textures are absolute paths from the app folder
					var texturePath = $"art.scnassets/textures/{paintMask}.ktx";

					if (name.Contains ("catapult")) {
						//os_log(.debug, "visited %s for texture", name)
					}

					var surfaceScript = @"
                    #pragma arguments

                    texture2d<float> paintMaskTexture;
                    //sampler paintMaskSampler;
                    float4 paintMaskColor;

                    #pragma body

                    // 0 is use diffuse texture.rgb, 1 is use paintMaskColor.rgb
                    float paintMaskSelector = paintMaskTexture.sample(
                        sampler(filter::linear), // paintMaskSampler,
                        _surface.diffuseTexcoord.xy).r;

                    _surface.diffuse.rgb = mix(_surface.diffuse.rgb, paintMaskColor.rgb, paintMaskSelector);
                    ";

					geometry.ShaderModifiers = new SCNShaderModifiers () { EntryPointSurface = surfaceScript };

					// mask pro
					var prop = SCNMaterialProperty.Create (new NSString (texturePath));
					prop.MinificationFilter = SCNFilterMode.Linear;
					prop.MagnificationFilter = SCNFilterMode.Nearest;
					prop.MipFilter = SCNFilterMode.Nearest;
					prop.MaxAnisotropy = 1;

					// set the uniforms, these will be overridden in the runtime
					geometry.SetTexture ("paintMaskTexture", prop);
					geometry.SetFloat4 (PaintMaskColorKey, SCNVector4.One);
				}
			}
		}

		#endregion

		#region animations

		public static void PlayAllAnimations (this SCNNode node)
		{
			node.EnumerateChildNodes ((SCNNode child, out bool stop) => {
				stop = false;
				foreach (var key in child.GetAnimationKeys ()) {
					var animationPlayer = child.GetAnimationPlayer (key);
					if (animationPlayer != null) {
						animationPlayer.Play ();
					}
				}
			});
		}

		public static void StopAllAnimations (this SCNNode node)
		{
			node.EnumerateChildNodes ((SCNNode child, out bool stop) => {
				stop = false;
				foreach (var key in child.GetAnimationKeys ()) {
					var animationPlayer = child.GetAnimationPlayer (key);
					if (animationPlayer != null) {
						animationPlayer.Stop ();
					}
				}
			});
		}

		#endregion

		public static void CalculateMassFromDensity (this SCNNode node, string name, float density)
		{
			if (node.PhysicsBody != null) {
				// our naming convention lets us parse the shape geometry
				var boundingBoxMin = SCNVector3.Zero;
				var boundingBoxMax = SCNVector3.Zero;
				node.GetBoundingBox (ref boundingBoxMin, ref boundingBoxMax);
				var bounds = (boundingBoxMax - boundingBoxMin);
				// calculate as a cylinder going up
				if (node.Name.StartsWith ("block_cylinder", StringComparison.Ordinal)) {
					var radius = bounds.X / 2f;
					var mass = (float) Math.PI * radius * radius * bounds.Y;
					node.PhysicsBody.Mass = density * mass;
				} else if (node.Name.StartsWith ("block_halfCylinder", StringComparison.Ordinal)) {
					// half cylinder going up
					var radius = Math.Min (bounds.X, bounds.Z);
					var mass = (float) Math.PI * radius * radius * bounds.Y / 2f;
					node.PhysicsBody.Mass = density * mass;
				} else if (node.Name.StartsWith ("block_quarterCylinder", StringComparison.Ordinal)) {
					// this is a cylinder lying sideways
					var radius = Math.Min (bounds.Y, bounds.Z);
					var mass = (float) Math.PI * radius * radius * bounds.X / 4f;
					node.PhysicsBody.Mass = density * mass;
				} else {
					// for now, treat as box shape
					node.PhysicsBody.Mass = density * bounds.X * bounds.Y * bounds.Z;
				}
			}
		}
	}
}
