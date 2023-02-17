
namespace XamarinShot.Models {
	using Foundation;
	using Metal;
	using SceneKit;
	using XamarinShot.Utils;
	using System;
	using System.Collections.Generic;

	public class SimulationData {
		public SimulationData (SCNVector3 wind)
		{
			this.Wind = wind;
		}

		public SCNVector3 Wind { get; private set; }
	}

	public class ClothData : IDisposable {
		public ClothData (SCNNode clothNode, ClothSimMetalNode meshData)
		{
			this.ClothNode = clothNode;
			this.MeshData = meshData;
		}

		~ClothData ()
		{
			this.Dispose (false);
		}

		public SCNNode ClothNode { get; private set; }

		public ClothSimMetalNode MeshData { get; private set; }

		#region IDisposable

		private bool disposed;

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!this.disposed) {
				if (disposing) {
					if (this.ClothNode != null) {
						this.ClothNode.Dispose ();
						this.ClothNode = null;
					}

					if (this.MeshData != null) {
						this.MeshData.Dispose ();
						this.MeshData = null;
					}
				}

				this.disposed = true;
			}
		}

		#endregion
	}

	public class ClothSimMetalNode : IDisposable {
		public ClothSimMetalNode (IMTLDevice device, uint width, uint height)
		{
			var vector3Size = System.Runtime.InteropServices.Marshal.SizeOf<OpenTK.NVector3> ();

			var vertices = new List<SCNVector3> ();
			var normals = new List<SCNVector3> ();
			var uvs = new List<OpenTK.Vector2> ();
			var indices = new List<byte> ();

			for (var y = 0; y < height; y++) {
				for (var x = 0; x < width; x++) {
					var p = new SCNVector3 (x, 0, y);
					vertices.Add (p);
					normals.Add (SCNVector3.UnitY);
					uvs.Add (new OpenTK.Vector2 (p.X / width, p.Z / height));
				}
			}

			for (var y = 0; y < (height - 1); y++) {
				for (var x = 0; x < (width - 1); x++) {
					// make 2 triangles from the 4 vertices of a quad
					var i0 = (byte) (y * width + x);
					var i1 = (byte) (i0 + 1);
					var i2 = (byte) (i0 + width);
					var i3 = (byte) (i2 + 1);

					// triangle 1
					indices.Add (i0);
					indices.Add (i2);
					indices.Add (i3);

					// triangle 2
					indices.Add (i0);
					indices.Add (i3);
					indices.Add (i1);
				}
			}

			var vertexBuffer1 = device.CreateBuffer (vertices.ToArray (), MTLResourceOptions.CpuCacheModeWriteCombined);
			var vertexBuffer2 = device.CreateBuffer ((nuint) (vertices.Count * vector3Size),
													MTLResourceOptions.CpuCacheModeWriteCombined);

			var vertexSource = SCNGeometrySource.FromMetalBuffer (vertexBuffer1,
																 MTLVertexFormat.Float3,
																 SCNGeometrySourceSemantics.Vertex,
																 vertices.Count,
																 0,
																 vector3Size);


			var normalBuffer = device.CreateBuffer (normals.ToArray (), MTLResourceOptions.CpuCacheModeWriteCombined);
			var normalWorkBuffer = device.CreateBuffer ((nuint) (normals.Count * vector3Size),
													   MTLResourceOptions.CpuCacheModeWriteCombined);

			var normalSource = SCNGeometrySource.FromMetalBuffer (normalBuffer,
																 MTLVertexFormat.Float3,
																 SCNGeometrySourceSemantics.Normal,
																 normals.Count,
																 0,
																 vector3Size);


			var uvBuffer = device.CreateBuffer (uvs.ToArray (), MTLResourceOptions.CpuCacheModeWriteCombined);

			var uvSource = SCNGeometrySource.FromMetalBuffer (uvBuffer,
															 MTLVertexFormat.Float2,
															 SCNGeometrySourceSemantics.Texcoord,
															 uvs.Count,
															 0,
															 OpenTK.Vector2.SizeInBytes);

			var data = NSData.FromArray (indices.ToArray ());
			var indexElement = SCNGeometryElement.FromData (data, SCNGeometryPrimitiveType.Triangles, 1178, 4);
			var geometry = SCNGeometry.Create (new SCNGeometrySource [] { vertexSource, normalSource, uvSource },
											  new SCNGeometryElement [] { indexElement });

			// velocity buffers
			var velocityBuffer1 = device.CreateBuffer ((nuint) (vertices.Count * vector3Size),
													  MTLResourceOptions.CpuCacheModeWriteCombined);

			var velocityBuffer2 = device.CreateBuffer ((nuint) (vertices.Count * vector3Size),
													  MTLResourceOptions.CpuCacheModeWriteCombined);

			this.Geometry = geometry;
			this.VertexCount = vertices.Count;
			this.Vb1 = vertexBuffer1;
			this.Vb2 = vertexBuffer2;
			this.NormalBuffer = normalBuffer;
			this.NormalWorkBuffer = normalWorkBuffer;
			this.VelocityBuffers = new List<IMTLBuffer> { velocityBuffer1, velocityBuffer2 };
		}

		~ClothSimMetalNode ()
		{
			this.Dispose (false);
		}

		public SCNGeometry Geometry { get; private set; }

		public IMTLBuffer Vb1 { get; private set; }

		public IMTLBuffer Vb2 { get; private set; }

		public IMTLBuffer NormalBuffer { get; private set; }

		public IMTLBuffer NormalWorkBuffer { get; private set; }

		public int VertexCount { get; private set; }

		public List<IMTLBuffer> VelocityBuffers { get; } = new List<IMTLBuffer> ();

		public int CurrentBufferIndex { get; set; }

		#region IDisposable

		private bool disposed;

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!this.disposed) {
				if (disposing) {
					if (this.Geometry != null) {
						this.Geometry.Dispose ();
						this.Geometry = null;
					}

					if (this.Vb1 != null) {
						this.Vb1.Dispose ();
						this.Vb1 = null;
					}

					if (this.Vb2 != null) {
						this.Vb2.Dispose ();
						this.Vb2 = null;
					}

					if (this.NormalBuffer != null) {
						this.NormalBuffer.Dispose ();
						this.NormalBuffer = null;
					}

					if (this.NormalWorkBuffer != null) {
						this.NormalWorkBuffer.Dispose ();
						this.NormalWorkBuffer = null;
					}

					foreach (var item in this.VelocityBuffers) {
						item.Dispose ();
					}

					this.VelocityBuffers.Clear ();
				}

				this.disposed = true;
			}
		}

		#endregion
	}

	/// <summary>
	/// Encapsulate the 'Metal stuff' within a single class to handle setup and execution of the compute shaders.
	/// </summary>
	public class MetalClothSimulator : IDisposable {
		private const uint Width = 32;
		private const uint Height = 20;

		private readonly List<ClothData> clothData = new List<ClothData> ();

		private readonly IMTLDevice device;

		private IMTLCommandQueue commandQueue;
		private IMTLLibrary defaultLibrary;
		private IMTLFunction functionClothSim;
		private IMTLFunction functionNormalUpdate;
		private IMTLFunction functionNormalSmooth;
		private IMTLComputePipelineState pipelineStateClothSim;
		private IMTLComputePipelineState pipelineStateNormalUpdate;
		private IMTLComputePipelineState pipelineStateNormalSmooth;

		public MetalClothSimulator (IMTLDevice device)
		{
			this.device = device;
			this.commandQueue = this.device.CreateCommandQueue ();

			this.defaultLibrary = this.device.CreateDefaultLibrary ();
			this.functionClothSim = this.defaultLibrary.CreateFunction ("updateVertex");
			this.functionNormalUpdate = this.defaultLibrary.CreateFunction ("updateNormal");
			this.functionNormalSmooth = this.defaultLibrary.CreateFunction ("smoothNormal");

			this.pipelineStateClothSim = this.device.CreateComputePipelineState (this.functionClothSim, out NSError pipelineStateClothSimError);
			this.pipelineStateNormalUpdate = this.device.CreateComputePipelineState (this.functionNormalUpdate, out NSError pipelineStateNormalUpdateError);
			this.pipelineStateNormalSmooth = this.device.CreateComputePipelineState (this.functionNormalSmooth, out NSError pipelineStateNormalSmoothError);

			if (pipelineStateClothSimError != null ||
				pipelineStateNormalUpdateError != null ||
				pipelineStateNormalSmoothError != null) {
				throw new Exception ("error");
			}
		}

		~MetalClothSimulator ()
		{
			Dispose (false);
		}

		public void CreateFlagSimulationFromNode (SCNNode node)
		{
			var meshData = new ClothSimMetalNode (this.device, Width, Height);
			var clothNode = SCNNode.FromGeometry (meshData.Geometry);

			var flag = node.FindChildNode ("flagStaticWave", true);
			if (flag != null) {
				var boundingBoxMax = SCNVector3.Zero;
				var boundingBoxMin = SCNVector3.Zero;
				flag.GetBoundingBox (ref boundingBoxMin, ref boundingBoxMax);
				var existingFlagBV = boundingBoxMax - boundingBoxMin;

				var rescaleToMatchSizeMatrix = SCNMatrix4.Scale (existingFlagBV.X / (float) Width);

				var rotation = SCNQuaternion.FromAxisAngle (SCNVector3.UnitX, (float) Math.PI / 2f);
				var localTransform = rescaleToMatchSizeMatrix * SCNMatrix4.Rotate (rotation.ToQuaternion ());

				localTransform.Transpose ();
				var currentTransform = SCNMatrix4.Transpose (flag.Transform);
				var newTransform = currentTransform * localTransform;

				clothNode.Transform = SCNMatrix4.Transpose (newTransform);// flag.Transform * localTransform;
				if (clothNode.Geometry != null) {
					clothNode.Geometry.FirstMaterial = flag.Geometry?.FirstMaterial;
					if (clothNode.Geometry.FirstMaterial != null) {
						clothNode.Geometry.FirstMaterial.DoubleSided = true;
					}
				}

				flag.ParentNode.ReplaceChildNode (flag, clothNode);
				clothNode.Geometry.SetupPaintColorMask ("flag_flagA");
				clothNode.SetPaintColors ();
				clothNode.FixNormalMaps ();

				this.clothData.Add (new ClothData (clothNode, meshData));
			}
		}

		public void Update (SCNNode node)
		{
			foreach (var cloth in this.clothData) {
				var wind = new SCNVector3 (1.8f, 0f, 0f);

				// The multiplier is to rescale ball to flag model space.
				// The correct value should be passed in.
				var simData = new SimulationData (wind);
				this.Deform (cloth.MeshData, simData);
			}
		}

		public void Deform (ClothSimMetalNode mesh, SimulationData simData)
		{
			var w = this.pipelineStateClothSim.ThreadExecutionWidth;
			var threadsPerThreadgroup = new MTLSize ((nint) w, 1, 1);

			var threadgroupsPerGrid = new MTLSize ((mesh.VertexCount + (int) w - 1) / (int) w, 1, 1);

			var clothSimCommandBuffer = this.commandQueue.CommandBuffer ();
			var clothSimCommandEncoder = clothSimCommandBuffer?.ComputeCommandEncoder;
			if (clothSimCommandEncoder != null) {
				clothSimCommandEncoder.SetComputePipelineState (pipelineStateClothSim);

				clothSimCommandEncoder.SetBuffer (mesh.Vb1, 0, 0);
				clothSimCommandEncoder.SetBuffer (mesh.Vb2, 0, 1);
				clothSimCommandEncoder.SetBuffer (mesh.VelocityBuffers [mesh.CurrentBufferIndex], 0, 2);

				mesh.CurrentBufferIndex = (mesh.CurrentBufferIndex + 1) % 2;
				clothSimCommandEncoder.SetBuffer (mesh.VelocityBuffers [mesh.CurrentBufferIndex], 0, 3);

				//var pointer = System.Runtime.InteropServices.Marshal.GetComInterfaceForObject(simData, typeof(SimulationData));
				//clothSimCommandEncoder?.SetBytes(pointer, (nuint)System.Runtime.InteropServices.Marshal.SizeOf<SimulationData>(), 4);

				clothSimCommandEncoder.DispatchThreadgroups (threadgroupsPerGrid, threadsPerThreadgroup: threadsPerThreadgroup);

				clothSimCommandEncoder.EndEncoding ();
			}

			clothSimCommandBuffer?.Commit ();

			//

			var normalComputeCommandBuffer = this.commandQueue.CommandBuffer ();
			var normalComputeCommandEncoder = normalComputeCommandBuffer?.ComputeCommandEncoder;
			if (normalComputeCommandEncoder != null) {
				normalComputeCommandEncoder.SetComputePipelineState (pipelineStateNormalUpdate);
				normalComputeCommandEncoder.SetBuffer (mesh.Vb2, 0, 0);
				normalComputeCommandEncoder.SetBuffer (mesh.Vb1, 0, 1);
				normalComputeCommandEncoder.SetBuffer (mesh.NormalWorkBuffer, 0, 2);
				normalComputeCommandEncoder.DispatchThreadgroups (threadgroupsPerGrid, threadsPerThreadgroup);

				normalComputeCommandEncoder.EndEncoding ();
			}

			normalComputeCommandBuffer?.Commit ();

			//

			var normalSmoothComputeCommandBuffer = this.commandQueue.CommandBuffer ();
			var normalSmoothComputeCommandEncoder = normalSmoothComputeCommandBuffer?.ComputeCommandEncoder;
			if (normalSmoothComputeCommandEncoder != null) {
				normalSmoothComputeCommandEncoder.SetComputePipelineState (pipelineStateNormalSmooth);
				normalSmoothComputeCommandEncoder.SetBuffer (mesh.NormalWorkBuffer, 0, 0);
				normalSmoothComputeCommandEncoder.SetBuffer (mesh.NormalBuffer, 0, 1);
				normalSmoothComputeCommandEncoder.DispatchThreadgroups (threadgroupsPerGrid, threadsPerThreadgroup);

				normalSmoothComputeCommandEncoder.EndEncoding ();
			}

			normalSmoothComputeCommandBuffer?.Commit ();
		}

		#region IDisposable

		private bool disposed;

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!this.disposed) {
				if (disposing) {
					if (this.commandQueue != null) {
						this.commandQueue.Dispose ();
						this.commandQueue = null;
					}

					if (this.defaultLibrary != null) {
						this.defaultLibrary.Dispose ();
						this.defaultLibrary = null;
					}

					if (this.functionClothSim != null) {
						this.functionClothSim.Dispose ();
						this.functionClothSim = null;
					}

					if (this.functionNormalUpdate != null) {
						this.functionNormalUpdate.Dispose ();
						this.functionNormalUpdate = null;
					}

					if (this.functionNormalSmooth != null) {
						this.functionNormalSmooth.Dispose ();
						this.functionNormalSmooth = null;
					}

					if (this.pipelineStateClothSim != null) {
						this.pipelineStateClothSim.Dispose ();
						this.pipelineStateClothSim = null;
					}

					if (this.pipelineStateNormalUpdate != null) {
						this.pipelineStateNormalUpdate.Dispose ();
						this.pipelineStateNormalUpdate = null;
					}

					if (this.pipelineStateNormalSmooth != null) {
						this.pipelineStateNormalSmooth.Dispose ();
						this.pipelineStateNormalSmooth = null;
					}

					foreach (var item in this.clothData) {
						item.ClothNode.RemoveFromParentNode ();
						item.Dispose ();
					}

					this.clothData.Clear ();
				}

				this.disposed = true;
			}
		}

		#endregion

	}
}
