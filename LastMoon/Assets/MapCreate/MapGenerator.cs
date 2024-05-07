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

    public Texture2D gradientTexture; // �׶���Ʈ �ؽ�ó
    public Gradient gradient; // �׶���Ʈ

    //public GameObject cubePrefab; // ť�� ������
    //public int maxCubeCount = 1000; // ������ �ִ� ť�� ����
    //private int cubeCount = 0; // ������ ť�� ����

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale,
            octaves, persistance, lacunarity, offset);


        // �׶���Ʈ �ؽ�ó�� �̿��Ͽ� ���� �� ����
        Texture2D colourTexture = TextureGenerator.TextureFromHeightMap(noiseMap);

        // ���� ���� ���
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
                        //if (regions[i].height >= 0.8f && regions[i].height <= 1.0f)
                        //{
                        //    if (cubeCount < maxCubeCount)
                        //    {
                        //        // ������ ��ġ�� ť�� ������Ʈ ����
                        //        Vector3 cubePosition = new Vector3(x, currentHeight, y);
                        //        Instantiate(cubePrefab, cubePosition, Quaternion.identity);
                        //        cubeCount++; // ������ ť�� ���� ����
                        //    }
                        //}
                        break;
                    }
                }
            }
        }


        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.colourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            // �޽� ������ ������ �ʰ� �׶���Ʈ ������ ��� ����
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
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