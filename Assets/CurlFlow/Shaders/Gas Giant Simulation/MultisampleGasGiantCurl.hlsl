#include "../3D Simplex Noise/FBMSimplexNoise.hlsl"

// We get the gradient by taking multiple samples of the noise function along the sphere surface using manually calculated tangent and bitangent
// directions. The difference between the samples gives us the rate of change in those two directions which is enough to generate
// the gradient field. And the curl is just the gradient rotated by 90 degrees along the normal of the sphere so that it flows along the surface.

void getSamplingOffsetsInTangentDirs(float3 dir, out float samplingOffset, out float3 tangent, out float3 bitangent, out float3 offsetU, out float3 offsetV)
{
    float3 dpdx = ddx(dir);
    float3 dpdy = ddy(dir);
    
    samplingOffset = max(length(dpdx), length(dpdy));

    float3 up = float3(0, 1, 0);
    tangent = normalize(cross(up, dir));
    bitangent = normalize(cross(dir, tangent));

    offsetU = normalize(dir + tangent * samplingOffset);
    offsetV = normalize(dir + bitangent * samplingOffset);
}

float3 calculateGradient(float offset, float3 tangent, float3 bitangent, float noise, float noiseU, float noiseV)
{
    float gradU = (noiseU - noise);
    float gradV = (noiseV - noise);
    
    float3 gradient = gradU * tangent + gradV * bitangent;
    gradient = gradient / offset;
    return gradient;
}

float sampleGasGiantTurbulence(float3 position, float3 randomizationOffset, float turbulenceScale)
{
    float turbulence;
    fbmSimplexNoise3D_float(position, randomizationOffset, 3, .5, 2, turbulenceScale, turbulence);
    return turbulence;
}

void gasGiantTurbulence_float(
    float3 position,
    float3 randomizationOffset,
    float turbulenceScale,
    out float3 curlVector)
{
    float samplingOffset;
    float3 tangent;
    float3 bitangent;
    float3 offsetU;
    float3 offsetV;
    
    getSamplingOffsetsInTangentDirs(position, samplingOffset, tangent, bitangent, offsetU, offsetV);

    float noise = sampleGasGiantTurbulence(position, randomizationOffset, turbulenceScale);
    float noiseU = sampleGasGiantTurbulence(offsetU, randomizationOffset, turbulenceScale);
    float noiseV = sampleGasGiantTurbulence(offsetV, randomizationOffset, turbulenceScale);
    
    float3 gradient = calculateGradient(samplingOffset, tangent, bitangent, noise, noiseU, noiseV);
    
    curlVector = cross(position, gradient);
}

float sampleGasGiantBandedFlow(float3 position, float3 randomizationOffset, float bandsScale)
{
    float bands = sin(position.y * PI * bandsScale);
    return bands;
}

void gasGiantBandedFlow_float(
    float3 position,
    float3 randomizationOffset,
    float bandsScale,
    out float3 curlVector)
{
    float samplingOffset;
    float3 tangent;
    float3 bitangent;
    float3 offsetU;
    float3 offsetV;
    
    getSamplingOffsetsInTangentDirs(position, samplingOffset, tangent, bitangent, offsetU, offsetV);

    float noise = sampleGasGiantBandedFlow(position, randomizationOffset, bandsScale);
    float noiseU = sampleGasGiantBandedFlow(offsetU, randomizationOffset, bandsScale);
    float noiseV = sampleGasGiantBandedFlow(offsetV, randomizationOffset, bandsScale);
    
    float3 gradient = calculateGradient(samplingOffset, tangent, bitangent, noise, noiseU, noiseV);
    
    curlVector = cross(position, gradient);
}
