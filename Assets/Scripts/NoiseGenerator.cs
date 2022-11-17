using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
    // generates a new noise map based on a number of parameters
    // returns a 2D float array
    public static float[,] GenerateNoiseMap (int noiseSampleSize, float scale, Wave[] waves, Vector2 offset, int resolution = 1)
    {
        float[,] noiseMap = new float[noiseSampleSize * resolution, noiseSampleSize * resolution];

        for (int x = 0; x < noiseSampleSize * resolution; x++)
        {
            for (int y = 0; y < noiseSampleSize * resolution; y++)
            {
                float samplePosX = ((float)x / scale / (float)resolution) + offset.y;
                float samplePosY = ((float)y / scale / (float)resolution) + offset.x;

                float noise = 0.0f;
                float norvalization = 0.0f;

                // appy the various different waves to add in varied terrain
                foreach (Wave wave in waves)
                {
                    noise += wave.amplitude * Mathf.PerlinNoise(samplePosX * wave.frequence + wave.seed, samplePosY * wave.frequence + wave.seed);
                    norvalization += wave.amplitude;
                }

                noise /= norvalization;
                noiseMap[x, y] = noise;
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateUniformNoiseMap(int size, float vertexOffset, float maxVertexDistance)
    {
        float[,] noiseMap = new float[size, size];

        for (int x = 0; x < size; x++)
        {
            float xSample = x + vertexOffset;
            float noise = Mathf.Abs(xSample) / maxVertexDistance;

            for (int z = 0; z < size; z++)
            {
                noiseMap[x, size - z - 1] = noise;
            }
        }
        return noiseMap;
    }
}

[System.Serializable]
public class Wave
{
    public float seed;
    public float frequence;
    public float amplitude;
}
