using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizedMode { Local , Grobal};
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed,
        float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizedMode normalizedMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y- halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)  
            {
                if (normalizedMode == NormalizedMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                } 
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, 1);  
                }
            }
        }
        return noiseMap;
    }

    public static float[,] GenerateIrregularNoiseMap(int mapWidth, int mapHeight, int seed,
    float scale, float irregularity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        offset = new Vector2(prng.Next(-100000, 100000) + offset.x, prng.Next(-100000, 100000) + offset.y);

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                float sampleX = (x - halfWidth + offset.x) / scale;
                float sampleY = (y - halfHeight + offset.y) / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                // Apply irregularity factor
                perlinValue *= irregularity;

                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }

    public static float[,] AddNoise(float[,] baseNoiseMap, float[,] additionalNoiseMap, float[,] irregularNoiseMap, float blendStrength, float irregularBlendStrength)
    {
        int width = baseNoiseMap.GetLength(0);
        int height = baseNoiseMap.GetLength(1);

        float[,] newNoiseMap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float blendedNoise = Mathf.Lerp(baseNoiseMap[x, y], additionalNoiseMap[x, y], blendStrength);
                newNoiseMap[x, y] = Mathf.Lerp(blendedNoise, irregularNoiseMap[x, y], irregularBlendStrength);
            }
        }

        return newNoiseMap;
    }
}
