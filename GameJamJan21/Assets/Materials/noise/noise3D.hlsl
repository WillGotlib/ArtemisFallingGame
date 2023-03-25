#ifndef NOISEFUNCTIONS
#define NOISEFUNCTIONS

#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"
// #include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise3D.hlsl"

void Simplex_float(const float3 v, const float scale, out float3 o)
{
    o.x = SimplexNoise(v * scale);
    o.y = SimplexNoise((v+ 168) * scale);
    o.z = SimplexNoise((v+ 37) * scale);
    o = (o+1)/2;
}

#endif
