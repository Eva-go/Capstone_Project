using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, colourMap, Mesh};
    public DrawMode drawMode;

    const int mapChunkSize = 1000;
    [Range(0,6)]
    public int levelOfDetail;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    public Color innerColor; // 내부 색상
    public Color outerColor; // 외부 색상


    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale,
            octaves, persistance, lacunarity, offset);

        Texture2D texture = TextureGenerator.TextureFromHeightMap(noiseMap);

        // 원형 그라디언트 텍스처 생성
        Texture2D gradientTexture = TextureGenerator.CreateIslandGradientTexture(mapChunkSize, mapChunkSize, innerColor, outerColor);

        Color[] colourMap = new Color[mapChunkSize *  mapChunkSize];
        for(int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height) 
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType <MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
            display.DrawTexture(gradientTexture);
        }
        else if (drawMode == DrawMode.colourMap) 
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
            display.DrawTexture(gradientTexture);
        }
        else if(drawMode == DrawMode.Mesh)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
            display.DrawTexture(gradientTexture);
        }
    }


    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}