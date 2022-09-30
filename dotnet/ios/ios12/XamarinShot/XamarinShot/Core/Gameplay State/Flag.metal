/*
 Dynamic Flag Simulation
 Copyright (c) 1995, 2018 Apple, Inc.
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
 
 Acknowledgments:
 Portions of this Dynamic Flag Simulation utilize the following copyrighted material acknowledged below:
 
 Flag.c, VecF.h, vecM.h, vecP.h, Tmat.h and Tmat.c
 Copyright (c) 1988 Brandyn Webb
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
 */

#include <metal_stdlib>
using namespace metal;

/*
 * Weight of a point is always 1. !!!
 * Prefered length of horizontal or vertical spring is always 1. !!!
 * Strength of springs: ( force = strength * displacement )
 * Friction: ( force = fric * vel )
 *
 * NOTE: statements involving fric have been
 * commented out on the assumption that fric is 1.!!!!
 */
//constant float fric = 1;
constant float strength = 10;
constant float strength2 = 20;    // "strength" of tortional constraints
constant float timestep = 1.0f/10.0f;
constant float G = 0.025; // gravity

// We set these as constants to give the compiler more opportunity to optimize.
// Another choice would be to pass these values into the shader at runtime.
constant int WIDTH = 32;
constant int HEIGHT = 20;

uint getIndex(uint x, uint y) { return x + y*WIDTH; }

float3 GetPosition(const device float3* positions, uint x, uint y)
{
    return positions[getIndex(x,y)];
}

float3 GetVelocity(const device float3* velocities, uint x, uint y)
{
    return velocities[getIndex(x,y)];
}

#define APPLY_FORCE(condition, dx, dy, str) \
if(condition) \
{ \
force += ApplyForce(inVertices, inVelocities, pos, vel, x, y, dx, dy, str); \
}

float3 ApplyForce(const device float3* positions,
                  const device float3* velocities,
                  const float3 pos,
                  const float3 vel,
                  const uint x,
                  const uint y,
                  const uint dx,
                  const uint dy,
                  const float constraintStrength)
{
    // Get the distance between the point and its neighbour of interest
    float3 deltaP = GetPosition(positions, x+dx, y+dy) - pos;
    
    float len = length(deltaP);
    
    float nominalLen = sqrt( float(dx*dx+dy*dy) );
    float sf = (len - nominalLen) * constraintStrength; // Spring force
    
    // Get the velocity difference between the same two points:
    float3 deltaV = GetVelocity(velocities, x+dx, y+dy) - vel;
    
    float invLen = 1/len;
    
    // Dot that with the positional delta to get spring motion:
    float sv = dot(deltaP, deltaV) * invLen;
    
    // Force = -(sf + sv * fric) * del/invLen.
    sf += sv;    // friction = 1
    sf *= invLen;
    
    deltaP *= sf;
    
    return deltaP;
}

struct SimulationData
{
    float3  wind;
};

kernel void updateVertex(const device float3* inVertices [[ buffer(0) ]],
                         device float3* outVertices [[ buffer(1) ]],
                         const device float3 *inVelocities [[ buffer(2) ]],
                         device float3 *outVelocities [[ buffer(3) ]],
                         constant SimulationData &simData [[ buffer(4)]],
                         uint id [[ thread_position_in_grid ]])
{
    const uint x = id % WIDTH;
    const uint y = id / WIDTH;
    
    // Point location, velocity, force (= acceleration, as mass is 1.).
    float3 pos = inVertices[id];
    float3 vel = inVelocities[id];
    
    // Start with 0 force.
    float3 force(0,0,0);
    
    APPLY_FORCE(x < WIDTH-1,  1, 0, strength);
    APPLY_FORCE(x > 0,       -1, 0, strength);
    
    APPLY_FORCE(y < HEIGHT-1, 0,  1, strength);
    APPLY_FORCE(y > 0,        0, -1, strength);
    
    APPLY_FORCE(x < WIDTH-1 && y < HEIGHT-1,  1,  1, strength);
    APPLY_FORCE(x > 0 && y > 0,              -1, -1, strength);
    
    APPLY_FORCE(x < WIDTH-2,  2, 0, strength2);
    APPLY_FORCE(x > 1      , -2, 0, strength2);
    
    APPLY_FORCE(y < HEIGHT-2, 0,  2, strength2);
    APPLY_FORCE(y > 1,        0, -2, strength2);
    
    // gravity
    force.y += G;
    
    // Add wind contribution
    float3 wind = simData.wind;
    
    if (x < WIDTH-1 && y < HEIGHT-1)
    {
        float3 right = GetPosition(inVertices, x+1, y) - pos;
        float3 up = GetPosition(inVertices, x, y+1) - pos;
        float3 n = normalize(cross(right, up));
        
        float3 relativeWind = wind - vel;
        
        float f = dot(relativeWind, n) * 0.25;
        
        force += n*f;
    }
    
    // Pin the two left-most corners to the flagpole
    if ((x == 0 && y == 0) || ((x == 0) && (y == HEIGHT-1)))
        force = 0;
    
    // Cancel force on the right edge of the flag
    if(x == WIDTH-1)
        force = 0;
    
    // move flag
    float3 v = vel + (force*timestep);
    outVelocities[id] = v;
    outVertices[id] = pos + v*timestep;
}

// This function computes the plane normals for the 6 planes touching the
// vertex and averages them as the vertex normal

kernel void updateNormal(const device float3* inVertices [[ buffer(0) ]],
                         device float3* outVertices [[ buffer(1) ]],
                         device float3* outNormals [[ buffer(2) ]],
                         uint id [[ thread_position_in_grid ]])
{
    const uint x = id % WIDTH;
    const uint y = id / WIDTH;
    
    const float3 v1 = inVertices[id];
    
    float3 normal(0, 0, 0);
    if (x < WIDTH-1)
    {
        if (y < HEIGHT-1)
        {
            float3 right = GetPosition(inVertices, x+1, y+1) - v1;
            float3 up = GetPosition(inVertices, x+1, y) - v1;
            float3 n = normalize(cross(right, up));
            
            normal += n;
            
            right = GetPosition(inVertices, x, y+1) - v1;
            up = GetPosition(inVertices, x+1, y+1) - v1;
            n = normalize(cross(right, up));
            
            normal += n;
        }
        if (y > 0)
        {
            float3 right = GetPosition(inVertices, x+1, y) - v1;
            float3 up = GetPosition(inVertices, x, y-1) - v1;
            float3 n = normalize(cross(right, up));
            
            normal += n;
        }
    }
    if (x > 0)
    {
        if (y < HEIGHT-1)
        {
            float3 right = GetPosition(inVertices, x-1, y) - v1;
            float3 up = GetPosition(inVertices, x, y+1) - v1;
            float3 n = normalize(cross(right, up));
            
            normal += n;
        }
        if (y > 0)
        {
            float3 right = GetPosition(inVertices, x-1, y-1) - v1;
            float3 up = GetPosition(inVertices, x-1, y) - v1;
            float3 n = normalize(cross(right, up));
            
            normal += n;
            
            right = GetPosition(inVertices, x, y-1) - v1;
            up = GetPosition(inVertices, x-1, y-1) - v1;
            n = normalize(cross(right, up));
            
            normal += n;
        }
    }
    
    outNormals[id] = normalize(normal);
    outVertices[id] = v1;
}


float3 GetNormal(const device float3* normals, uint x, uint y)
{
    return normals[getIndex(x,y)];
}

#define SMOOTH_FILTER_SIZE 3

kernel void smoothNormal(const device float3* inNormals [[ buffer(0) ]],
                         device float3* outNormals [[ buffer(1) ]],
                         uint id [[ thread_position_in_grid ]])
{
    const int x = id % WIDTH;
    const int y = id / WIDTH;
    
    float3 n(0,0,0);
    
    for(int i = max(0, x - SMOOTH_FILTER_SIZE); i < min(WIDTH, x + SMOOTH_FILTER_SIZE); i++)
    {
        for(int j = max(0, y - SMOOTH_FILTER_SIZE); j < min(HEIGHT, y + SMOOTH_FILTER_SIZE); j++)
        {
            n += GetNormal(inNormals, i, j);
        }
    }
    
    outNormals[id] = normalize(n);
}
