using Simplex;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunkGenerator : MonoBehaviour
{
    public int mapWidth = 256;
    public int mapHeight = 256;
    public int chunkSize = 64;

    public float scale1 = 20f;
    public float scale2 = 20f;

    [Range(0, 10)]
    public int octaves1;
    [Range(0, 10)]
    public int octaves2;

    [Range(0, 1)]
    public float persistance;

    public float lacunarity = 2f;

    public int seed1;
    public int seed2;

    public Vector2 globalOffset;

    [Range(0, 1)]
    public float blendStrength;

    private Terrain[,] terrains;

    void Start()
    {
        GenerateTerrainChunks();
    }

    void GenerateTerrainChunks()
    {
        int chunkCountX = mapWidth / chunkSize;
        int chunkCountY = mapHeight / chunkSize;

        // 전체 맵에 대해 노이즈 맵을 생성하고 각 청크에 배정
        float[,] noiseMap1 = Noise.GenerateNoiseMap(mapWidth + 1, mapHeight + 1, seed1, scale1, octaves1, persistance, lacunarity, globalOffset, Noise.NormalizedMode.Local);
        float[,] noiseMap2 = Noise.GenerateNoiseMap(mapWidth + 1, mapHeight + 1, seed2, scale2, octaves2, persistance, lacunarity, globalOffset, Noise.NormalizedMode.Local);
        float[,] blendedNoiseMap = Noise.AddNoise(noiseMap1, noiseMap2, blendStrength);


        for (int y = 0; y < chunkCountY; y++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                CreateTerrainChunk(x, y, blendedNoiseMap);
            }
        }

        SetTerrainNeighbors();
    }

    Terrain CreateTerrainChunk(int chunkX, int chunkY, float[,] heightMap)
    {
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = chunkSize + 1;
        terrainData.size = new Vector3(chunkSize, 50, chunkSize);

        // 청크의 높이 데이터를 heightMap에서 가져오기
        float[,] chunkHeightMap = new float[chunkSize + 1, chunkSize + 1];
        for (int y = 0; y < chunkSize + 1; y++)
        {
            for (int x = 0; x < chunkSize + 1; x++)
            {
                // 맵 경계를 넘어가지 않도록 클램핑
                int heightMapX = Mathf.Clamp(chunkX * chunkSize + x, 0, mapWidth);
                int heightMapY = Mathf.Clamp(chunkY * chunkSize + y, 0, mapHeight);
                chunkHeightMap[x, y] = heightMap[heightMapX, heightMapY];
            }
        }

        terrainData.SetHeights(0, 0, chunkHeightMap);

        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.transform.position = new Vector3(chunkX * chunkSize, 0, chunkY * chunkSize);

        return terrainObject.GetComponent<Terrain>();
    }
    void SetTerrainNeighbors()
    {
        int chunkCountX = terrains.GetLength(0);
        int chunkCountY = terrains.GetLength(1);

        for (int y = 0; y < chunkCountY; y++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                Terrain currentTerrain = terrains[x, y];
                if (currentTerrain == null) continue;

                Terrain leftNeighbor = (x > 0) ? terrains[x - 1, y] : null;
                Terrain rightNeighbor = (x < chunkCountX - 1) ? terrains[x + 1, y] : null;
                Terrain topNeighbor = (y < chunkCountY - 1) ? terrains[x, y + 1] : null;
                Terrain bottomNeighbor = (y > 0) ? terrains[x, y - 1] : null;

                currentTerrain.SetNeighbors(leftNeighbor, topNeighbor, rightNeighbor, bottomNeighbor);
            }
        }
    }
}