#ifndef FRACTAL_NOISE_INCLUDED
#define FRACTAL_NOISE_INCLUDED

#include "SimplexNoise.hlsl"

void fbmSimplexNoise3D_float(
	float3 pos,
	float3 offset,
	int numLayers,
	float persistence,
	float lacunarity,
	float scale, 
	out float noise)
{
	float noiseSum = 0;
	float amplitude = 1;
	float amplitudeSum = 1e-6; // Avoid division by zero
	float frequency = scale;
	for (int i = 0; i < numLayers; i++) {
		noiseSum += snoise(pos * frequency + offset) * amplitude;
		amplitudeSum += amplitude;
		amplitude *= persistence;
		frequency *= lacunarity;
	}
	noise = noiseSum / amplitudeSum;  // Returns values in the range [-1, 1]
}

#endif