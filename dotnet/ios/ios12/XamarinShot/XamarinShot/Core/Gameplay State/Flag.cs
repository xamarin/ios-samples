namespace XamarinShot.Models;

public class SimulationData
{
        public SimulationData (SCNVector3 wind)
        {
                Wind = wind;
        }

        public SCNVector3 Wind { get; private set; }
}

public class ClothData : IDisposable
{
        public ClothData (SCNNode clothNode, ClothSimMetalNode meshData)
        {
                ClothNode = clothNode;
                MeshData = meshData;
        }

        ~ClothData ()
        {
                Dispose (false);
        }

        public SCNNode? ClothNode { get; private set; }

        public ClothSimMetalNode? MeshData { get; private set; }

        #region IDisposable

        private bool disposed;

        public void Dispose ()
        {
                Dispose (true);
                GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
                if (!disposed)
                {
                        if (disposing)
                        {
                                ClothNode?.Dispose ();
                                ClothNode = null;

                                MeshData?.Dispose ();
                                MeshData = null;
                        }

                        disposed = true;
                }
        }

        #endregion
}

public class ClothSimMetalNode : IDisposable
{
        public ClothSimMetalNode (IMTLDevice device, uint width, uint height)
        {
                var vector3Size = System.Runtime.InteropServices.Marshal.SizeOf<OpenTK.NVector3> ();

                var vertices = new List<SCNVector3> ();
                var normals = new List<SCNVector3> ();
                var uvs = new List<OpenTK.Vector2> ();
                var indices = new List<byte> ();

                for (var y = 0; y < height; y++)
                {
                        for (var x = 0; x < width; x++)
                        {
                                var p = new SCNVector3 (x, 0, y);
                                vertices.Add (p);
                                normals.Add (SCNVector3.UnitY);
                                uvs.Add (new OpenTK.Vector2 (p.X / width, p.Z / height));
                        }
                }

                for (var y = 0; y < (height - 1); y++)
                {
                        for (var x = 0; x < (width - 1); x++)
                        {
                                // make 2 triangles from the 4 vertices of a quad
                                var i0 = (byte)(y * width + x);
                                var i1 = (byte)(i0 + 1);
                                var i2 = (byte)(i0 + width);
                                var i3 = (byte)(i2 + 1);

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
                if (vertexBuffer1 is null)
                        throw new Exception ("unable to create vertex buffer");
                var vertexBuffer2 = device.CreateBuffer ((nuint)(vertices.Count * vector3Size),
                                                        MTLResourceOptions.CpuCacheModeWriteCombined);
                if (vertexBuffer2 is null)
                        throw new Exception ("unable to create second vertex buffer");

                var vertexSource = SCNGeometrySource.FromMetalBuffer (vertexBuffer1,
                                                                     MTLVertexFormat.Float3,
                                                                     SCNGeometrySourceSemantics.Vertex,
                                                                     vertices.Count,
                                                                     0,
                                                                     vector3Size);


                var normalBuffer = device.CreateBuffer (normals.ToArray (), MTLResourceOptions.CpuCacheModeWriteCombined);
                if (normalBuffer is null)
                        throw new Exception ("unable to create normal buffer");
                var normalWorkBuffer = device.CreateBuffer ((nuint)(normals.Count * vector3Size),
                                                           MTLResourceOptions.CpuCacheModeWriteCombined);

                var normalSource = SCNGeometrySource.FromMetalBuffer (normalBuffer,
                                                                     MTLVertexFormat.Float3,
                                                                     SCNGeometrySourceSemantics.Normal,
                                                                     normals.Count,
                                                                     0,
                                                                     vector3Size);


                var uvBuffer = device.CreateBuffer (uvs.ToArray (), MTLResourceOptions.CpuCacheModeWriteCombined);
                if (uvBuffer is null)
                        throw new Exception ("unable to create uv buffer");

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
                var velocityBuffer1 = device.CreateBuffer ((nuint)(vertices.Count * vector3Size),
                                                          MTLResourceOptions.CpuCacheModeWriteCombined);
                if (velocityBuffer1 is null)
                        throw new Exception ("unable to allocate velocity buffer");

                var velocityBuffer2 = device.CreateBuffer ((nuint)(vertices.Count * vector3Size),
                                                          MTLResourceOptions.CpuCacheModeWriteCombined);
                if (velocityBuffer2 is null)
                        throw new Exception ("unable to allocate second velocity buffer");

                Geometry = geometry;
                VertexCount = vertices.Count;
                Vb1 = vertexBuffer1;
                Vb2 = vertexBuffer2;
                NormalBuffer = normalBuffer;
                NormalWorkBuffer = normalWorkBuffer;
                VelocityBuffers = new List<IMTLBuffer> { velocityBuffer1, velocityBuffer2 };
        }

        ~ClothSimMetalNode ()
        {
                Dispose (false);
        }

        public SCNGeometry? Geometry { get; private set; }

        public IMTLBuffer? Vb1 { get; private set; }

        public IMTLBuffer? Vb2 { get; private set; }

        public IMTLBuffer? NormalBuffer { get; private set; }

        public IMTLBuffer? NormalWorkBuffer { get; private set; }

        public int VertexCount { get; private set; }

        public List<IMTLBuffer> VelocityBuffers { get; } = new List<IMTLBuffer> ();

        public int CurrentBufferIndex { get; set; }

        #region IDisposable

        bool disposed;

        public void Dispose ()
        {
                Dispose (true);
                GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
                if (!disposed)
                {
                        if (disposing)
                        {
                                Geometry?.Dispose ();
                                Geometry = null;

                                Vb1?.Dispose ();
                                Vb1 = null;

                                Vb2?.Dispose ();
                                Vb2 = null;

                                NormalBuffer?.Dispose ();
                                NormalBuffer = null;

                                NormalWorkBuffer?.Dispose ();
                                NormalWorkBuffer = null;

                                foreach (var item in VelocityBuffers)
                                {
                                        item.Dispose ();
                                }

                                VelocityBuffers.Clear ();
                        }

                        disposed = true;
                }
        }

        #endregion
}

/// <summary>
/// Encapsulate the 'Metal stuff' within a single class to handle setup and execution of the compute shaders.
/// </summary>
public class MetalClothSimulator : IDisposable
{
        const uint Width = 32;
        const uint Height = 20;

        readonly List<ClothData> clothData = new List<ClothData> ();

        readonly IMTLDevice device;

        IMTLCommandQueue? commandQueue;
        IMTLLibrary defaultLibrary;
        IMTLFunction functionClothSim;
        IMTLFunction functionNormalUpdate;
        IMTLFunction functionNormalSmooth;
        IMTLComputePipelineState pipelineStateClothSim;
        IMTLComputePipelineState pipelineStateNormalUpdate;
        IMTLComputePipelineState pipelineStateNormalSmooth;

        public MetalClothSimulator (IMTLDevice device)
        {
                this.device = device;
                commandQueue = this.device.CreateCommandQueue ();

                defaultLibrary = this.device.CreateDefaultLibrary ();
                functionClothSim = defaultLibrary.CreateFunction ("updateVertex");
                functionNormalUpdate = defaultLibrary.CreateFunction ("updateNormal");
                functionNormalSmooth = defaultLibrary.CreateFunction ("smoothNormal");

                pipelineStateClothSim = device.CreateComputePipelineState (functionClothSim, out NSError pipelineStateClothSimError);
                pipelineStateNormalUpdate = device.CreateComputePipelineState (functionNormalUpdate, out NSError pipelineStateNormalUpdateError);
                pipelineStateNormalSmooth = device.CreateComputePipelineState (functionNormalSmooth, out NSError pipelineStateNormalSmoothError);

                if (pipelineStateClothSimError is not null ||
                        pipelineStateNormalUpdateError is not null ||
                        pipelineStateNormalSmoothError is not null)
                {
                        throw new Exception ("error");
                }
        }

        ~MetalClothSimulator ()
        {
                Dispose (false);
        }

        public void CreateFlagSimulationFromNode (SCNNode node)
        {
                var meshData = new ClothSimMetalNode (device, Width, Height);
                var clothNode = SCNNode.FromGeometry (meshData.Geometry);

                var flag = node.FindChildNode ("flagStaticWave", true);
                if (flag is not null)
                {
                        var boundingBoxMax = SCNVector3.Zero;
                        var boundingBoxMin = SCNVector3.Zero;
                        flag.GetBoundingBox (ref boundingBoxMin, ref boundingBoxMax);
                        var existingFlagBV = boundingBoxMax - boundingBoxMin;

                        var rescaleToMatchSizeMatrix = SCNMatrix4.Scale (existingFlagBV.X / (float)Width);

                        var rotation = SCNQuaternion.FromAxisAngle (SCNVector3.UnitX, (float)Math.PI / 2f);
                        var localTransform = rescaleToMatchSizeMatrix * SCNMatrix4.Rotate (rotation.ToQuaternion ());

                        localTransform.Transpose ();
                        var currentTransform = SCNMatrix4.Transpose (flag.Transform);
                        var newTransform = currentTransform * localTransform;

                        clothNode.Transform = SCNMatrix4.Transpose (newTransform);// flag.Transform * localTransform;
                        if (clothNode.Geometry is not null)
                        {
                                clothNode.Geometry.FirstMaterial = flag.Geometry?.FirstMaterial;
                                if (clothNode.Geometry.FirstMaterial is not null)
                                {
                                        clothNode.Geometry.FirstMaterial.DoubleSided = true;
                                }
                        }

                        flag.ParentNode?.ReplaceChildNode (flag, clothNode);
                        clothNode.Geometry?.SetupPaintColorMask ("flag_flagA");
                        clothNode.SetPaintColors ();
                        clothNode.FixNormalMaps ();

                        clothData.Add (new ClothData (clothNode, meshData));
                }
        }

        public void Update (SCNNode node)
        {
                foreach (var cloth in clothData)
                {
                        var wind = new SCNVector3 (1.8f, 0f, 0f);

                        // The multiplier is to rescale ball to flag model space.
                        // The correct value should be passed in.
                        var simData = new SimulationData (wind);
                        Deform (cloth.MeshData!, simData);
                }
        }

        public void Deform (ClothSimMetalNode mesh, SimulationData simData)
        {
                var w = pipelineStateClothSim.ThreadExecutionWidth;
                var threadsPerThreadgroup = new MTLSize ((nint)w, 1, 1);

                var threadgroupsPerGrid = new MTLSize ((mesh.VertexCount + (int)w - 1) / (int)w, 1, 1);

                var clothSimCommandBuffer = commandQueue?.CommandBuffer ();
                var clothSimCommandEncoder = clothSimCommandBuffer?.ComputeCommandEncoder;
                if (clothSimCommandEncoder is not null)
                {
                        clothSimCommandEncoder.SetComputePipelineState (pipelineStateClothSim);

                        clothSimCommandEncoder.SetBuffer (mesh.Vb1!, 0, 0);
                        clothSimCommandEncoder.SetBuffer (mesh.Vb2!, 0, 1);
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

                var normalComputeCommandBuffer = commandQueue?.CommandBuffer ();
                var normalComputeCommandEncoder = normalComputeCommandBuffer?.ComputeCommandEncoder;
                if (normalComputeCommandEncoder is not null)
                {
                        normalComputeCommandEncoder.SetComputePipelineState (pipelineStateNormalUpdate);
                        normalComputeCommandEncoder.SetBuffer (mesh.Vb2!, 0, 0);
                        normalComputeCommandEncoder.SetBuffer (mesh.Vb1!, 0, 1);
                        normalComputeCommandEncoder.SetBuffer (mesh.NormalWorkBuffer!, 0, 2);
                        normalComputeCommandEncoder.DispatchThreadgroups (threadgroupsPerGrid, threadsPerThreadgroup);

                        normalComputeCommandEncoder.EndEncoding ();
                }

                normalComputeCommandBuffer?.Commit ();

                //

                var normalSmoothComputeCommandBuffer = commandQueue?.CommandBuffer ();
                var normalSmoothComputeCommandEncoder = normalSmoothComputeCommandBuffer?.ComputeCommandEncoder;
                if (normalSmoothComputeCommandEncoder is not null)
                {
                        normalSmoothComputeCommandEncoder.SetComputePipelineState (pipelineStateNormalSmooth);
                        normalSmoothComputeCommandEncoder.SetBuffer (mesh.NormalWorkBuffer!, 0, 0);
                        normalSmoothComputeCommandEncoder.SetBuffer (mesh.NormalBuffer!, 0, 1);
                        normalSmoothComputeCommandEncoder.DispatchThreadgroups (threadgroupsPerGrid, threadsPerThreadgroup);

                        normalSmoothComputeCommandEncoder.EndEncoding ();
                }

                normalSmoothComputeCommandBuffer?.Commit ();
        }

        #region IDisposable

        bool disposed;

        public void Dispose ()
        {
                Dispose (true);
                GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
                if (!this.disposed)
                {
                        if (disposing)
                        {
                                commandQueue?.Dispose ();
                                commandQueue = null;

                                defaultLibrary.Dispose ();

                                functionClothSim.Dispose ();

                                functionNormalUpdate.Dispose ();

                                functionNormalSmooth.Dispose ();

                                pipelineStateClothSim.Dispose ();

                                pipelineStateNormalUpdate.Dispose ();

                                pipelineStateNormalSmooth.Dispose () ;

                                foreach (var item in clothData)
                                {
                                        item.ClothNode?.RemoveFromParentNode ();
                                        item.Dispose ();
                                }

                                clothData.Clear ();
                        }

                        disposed = true;
                }
        }

        #endregion

}
