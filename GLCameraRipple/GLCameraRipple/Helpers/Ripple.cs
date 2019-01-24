using CoreGraphics;
using System;
using System.Runtime.InteropServices;

namespace GLCameraRipple
{
    public class RippleModel
    {
        private readonly CGSize screenSize;
        private readonly int poolHeight, poolWidth;
        private readonly int touchRadius;

        private readonly float texCoordFactorS;
        private readonly float texCoordOffsetS;
        private readonly float texCoordFactorT;
        private readonly float texCoordOffsetT;

        // ripple coefficients
        private readonly float[,] rippleCoeff;

        // ripple simulation buffers
        private float[,] rippleSource;
        private float[,] rippleDest;

        // data passed to GL
        private unsafe float* rippleVertices;
        private unsafe float* rippleTexCoords;
        private unsafe ushort* rippleIndicies;

        public RippleModel(CGSize screenSize, int meshFactor, int touchRadius, CGSize textureSize)
        {
            Console.WriteLine("New 'Ripple Model'");

            this.screenSize = screenSize;
            this.touchRadius = touchRadius;
            this.poolWidth = (int)this.screenSize.Width / meshFactor;
            this.poolHeight = (int)this.screenSize.Height / meshFactor;

            if ((float)this.screenSize.Height / this.screenSize.Width < (float)textureSize.Width / textureSize.Height)
            {
                this.texCoordFactorS = (float)((textureSize.Height * this.screenSize.Height) / (this.screenSize.Width * textureSize.Width));
                this.texCoordOffsetS = (1 - this.texCoordFactorS) / 2f;

                this.texCoordFactorT = 1;
                this.texCoordOffsetT = 0;
            }
            else
            {
                this.texCoordFactorS = 1;
                this.texCoordOffsetS = 0;

                this.texCoordFactorT = (float)((this.screenSize.Width * textureSize.Width) / (textureSize.Height * this.screenSize.Height));
                this.texCoordOffsetT = (1 - this.texCoordFactorT) / 2f;
            }

            this.rippleCoeff = new float[touchRadius * 2 + 1, touchRadius * 2 + 1];

            // +2 for padding the border
            this.rippleSource = new float[this.poolWidth + 2, this.poolHeight + 2];
            this.rippleDest = new float[this.poolWidth + 2, this.poolHeight + 2];

            unsafe
            {
                int poolsize2 = this.poolWidth * this.poolHeight * 2;
                rippleVertices = (float*)Marshal.AllocHGlobal(poolsize2 * sizeof(float));
                rippleTexCoords = (float*)Marshal.AllocHGlobal(poolsize2 * sizeof(float));
                rippleIndicies = (ushort*)Marshal.AllocHGlobal((this.poolHeight - 1) * (this.poolWidth * 2 + 2) * sizeof(ushort));
            }

            this.InitRippleCoef();
            this.InitMesh();
        }

        private void InitRippleCoef()
        {
            for (int y = 0; y <= 2 * this.touchRadius; y++)
            {
                for (int x = 0; x <= 2 * this.touchRadius; x++)
                {
                    float distance = (float)Math.Sqrt((x - this.touchRadius) * (x - this.touchRadius) + (y - this.touchRadius) * (y - this.touchRadius));

                    if (distance <= this.touchRadius)
                    {
                        var factor = (distance / this.touchRadius);

                        // goes from -512 -> 0
                        this.rippleCoeff[x, y] = -((float)Math.Cos(factor * Math.PI) + 1f) * 256f;
                    }
                    else
                    {
                        this.rippleCoeff[x, y] = 0f;
                    }
                }
            }
        }

        private unsafe void InitMesh()
        {
            for (int i = 0; i < this.poolHeight; i++)
            {
                for (int j = 0; j < this.poolWidth; j++)
                {
                    this.rippleVertices[(i * this.poolWidth + j) * 2 + 0] = -1f + j * (2f / (this.poolWidth - 1));
                    this.rippleVertices[(i * this.poolWidth + j) * 2 + 1] = 1f - i * (2f / (this.poolHeight - 1));

                    this.rippleTexCoords[(i * this.poolWidth + j) * 2 + 0] = (float)i / (this.poolHeight - 1) * this.texCoordFactorS + this.texCoordOffsetS;
                    this.rippleTexCoords[(i * this.poolWidth + j) * 2 + 1] = (1f - (float)j / (this.poolWidth - 1)) * this.texCoordFactorT + this.texCoordFactorT;
                }
            }

            uint index = 0;
            for (int i = 0; i < this.poolHeight - 1; i++)
            {
                for (int j = 0; j < this.poolWidth; j++)
                {
                    if (i % 2 == 0)
                    {
                        // emit extra index to create degenerate triangle
                        if (j == 0)
                        {
                            this.rippleIndicies[index] = (ushort)(i * this.poolWidth + j);
                            index++;
                        }

                        this.rippleIndicies[index] = (ushort)(i * this.poolWidth + j);
                        index++;
                        this.rippleIndicies[index] = (ushort)((i + 1) * this.poolWidth + j);
                        index++;

                        // emit extra index to create degenerate triangle
                        if (j == (this.poolWidth - 1))
                        {
                            this.rippleIndicies[index] = (ushort)((i + 1) * this.poolWidth + j);
                            index++;
                        }
                    }
                    else
                    {
                        // emit extra index to create degenerate triangle
                        if (j == 0)
                        {
                            this.rippleIndicies[index] = (ushort)((i + 1) * this.poolWidth + j);
                            index++;
                        }

                        this.rippleIndicies[index] = (ushort)((i + 1) * this.poolWidth + j);
                        index++;
                        this.rippleIndicies[index] = (ushort)(i * this.poolWidth + j);
                        index++;

                        // emit extra index to create degenerate triangle
                        if (j == (this.poolWidth - 1))
                        {
                            this.rippleIndicies[index] = (ushort)(i * this.poolWidth + j);
                            index++;
                        }
                    }
                }
            }
        }

        public IntPtr Vertices
        {
            get { unsafe { return (IntPtr)this.rippleVertices; } }
        }

        public IntPtr TexCoords
        {
            get { unsafe { return (IntPtr)this.rippleTexCoords; } }
        }

        public IntPtr Indices
        {
            get { unsafe { return (IntPtr)this.rippleIndicies; } }
        }

        public int VertexSize
        {
            get { return this.poolWidth * this.poolHeight * 2 * sizeof(float); }
        }

        public int IndexSize
        {
            get { return (this.poolHeight - 1) * (this.poolWidth * 2 + 2) * sizeof(ushort); }
        }

        public int IndexCount
        {
            get { return this.IndexSize / sizeof(ushort); }
        }

        public unsafe void RunSimulation()
        {
            for (int y = 0; y < this.poolHeight; y++)
            {
                for (int x = 0; x < this.poolWidth; x++)
                {
                    // * - denotes current pixel
                    //
                    //       a
                    //     c * d
                    //       b

                    // +1 to both x/y values because the border is padded
                    float a = this.rippleSource[x + 1, y];
                    float b = this.rippleSource[x + 1, y + 2];
                    float c = this.rippleSource[x, y + 1];
                    float d = this.rippleSource[x + 2, y + 1];

                    float result = (a + b + c + d) / 2f - this.rippleDest[x + 1, y + 1];
                    result -= result / 32f;

                    this.rippleDest[x + 1, y + 1] = result;
                }
            }

            for (int y = 0; y < this.poolHeight; y++)
            {
                for (int x = 0; x < this.poolWidth; x++)
                {
                    // * - denotes current pixel
                    //
                    //       a
                    //     c * d
                    //       b

                    // +1 to both x/y values because the border is padded
                    float a = this.rippleDest[x + 1, y];
                    float b = this.rippleDest[x + 1, y + 2];
                    float c = this.rippleDest[x, y + 1];
                    float d = this.rippleDest[x + 2, y + 1];

                    float s_offset = ((b - a) / 2048f);
                    float t_offset = ((c - d) / 2048f);

                    // clamp
                    s_offset = (s_offset < -0.5f) ? -0.5f : s_offset;
                    t_offset = (t_offset < -0.5f) ? -0.5f : t_offset;
                    s_offset = (s_offset > 0.5f) ? 0.5f : s_offset;
                    t_offset = (t_offset > 0.5f) ? 0.5f : t_offset;

                    float s_tc = (float)y / (poolHeight - 1) * this.texCoordFactorS + this.texCoordOffsetS;
                    float t_tc = (1f - (float)x / (poolWidth - 1)) * this.texCoordFactorT + this.texCoordOffsetT;

                    this.rippleTexCoords[(y * this.poolWidth + x) * 2 + 0] = s_tc + s_offset;
                    this.rippleTexCoords[(y * this.poolWidth + x) * 2 + 1] = t_tc + t_offset;
                }
            }

            var tmp = this.rippleSource;
            this.rippleSource = this.rippleDest;
            this.rippleDest = tmp;
        }

        public void InitiateRippleAtLocation(CGPoint location)
        {
            int xIndex = (int)(location.X / this.screenSize.Width * this.poolWidth);
            int yIndex = (int)(location.Y / this.screenSize.Height * this.poolHeight);

            for (int y = yIndex - this.touchRadius; y <= yIndex + this.touchRadius; y++)
            {
                for (int x = xIndex - this.touchRadius; x <= xIndex + this.touchRadius; x++)
                {
                    if (x >= 0 && x < this.poolWidth && y >= 0 && y < this.poolHeight)
                    {
                        // +1 to both x/y values because the border is padded
                        this.rippleSource[x + 1, y + 1] += this.rippleCoeff[y - (yIndex - this.touchRadius), x - (xIndex - this.touchRadius)];
                    }
                }
            }
        }
    }
}