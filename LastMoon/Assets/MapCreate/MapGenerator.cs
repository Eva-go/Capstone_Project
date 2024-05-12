using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, colourMap, Mesh, FalloffMap};
    public DrawMode drawMode;

    public const int mapChunkSize = 239;
    [Range(0,6)]
    public int levelOfDetail;
    public float noiseScale;

    [Range (0,10)]
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool useFalloff;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    float[,] falloffMap;

    //public Texture2D gradientTexture; // �׶���Ʈ �ؽ�ó
    //public Gradient gradient; // �׶���Ʈ

    //public int maxCubeCount = 1000; // �ִ� ť�� ����
    //private int cubeCount = 0; // ���� ������ ť�� ����
    //public GameObject cubePrefab; // ť�� ������

    void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
    public void GenerateMap()
    {
        // ������ �� ����
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize+2, mapChunkSize+2, seed, noiseScale,
            octaves, persistance, lacunarity, offset);

        // �÷� �� ����
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
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

        //PlaceCubes();

        // ���� ǥ��
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
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
        }

        //void PlaceCubes()
        //{
        //    foreach (TerrainType region in regions)
        //    {
        //        if (region.name == "concrete1")
        //        {
        //            for (int y = 0; y < mapChunkSize; y++)
        //            {
        //                for (int x = 0; x < mapChunkSize; x++)
        //                {
        //                    float currentHeight = noiseMap[x, y];
        //                    if (ChooseTerrainType(currentHeight).Equals(region))
        //                    {
        //                        // ť�� ������ �ִ� ������ �ʰ����� �ʴ��� Ȯ���մϴ�.
        //                        if (cubeCount < maxCubeCount)
        //                        {
        //                            // ť�긦 ��ġ�� ��ġ ���
        //                            Vector3 cubePosition = new Vector3(x + Random.Range(-0.5f, 0.5f), currentHeight, y + Random.Range(-0.5f, 0.5f));
        //                            // ť�� ����
        //                            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

        //                            // ť�� ũ�� ���� (�ɼ�)
        //                            cube.transform.localScale = new Vector3(18, 64, 18);

        //                            // ������ ť�� ���� ����
        //                            cubeCount++;
        //                        }
        //                        else
        //                        {
        //                            Debug.LogWarning("�ִ� ť�� ������ �����߽��ϴ�.");
        //                            return; // �ִ� ������ �����ϸ� �� �̻� ť�긦 �������� ����
        //                        }
        //                    }
        //                }
        //            }
        //            break;
        //        }
        //    }
        //    TerrainType ChooseTerrainType(float height)
        //    {
        //        foreach (TerrainType region in regions)
        //        {
        //            if (height <= region.height)
        //            {
        //                return region;
        //            }
        //        }
        //        return regions[regions.Length - 1]; // �⺻������ ������ ���� ���� ��ȯ
        //    }
        //}

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
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
    void Start()
    {
        // seed�� 0�̸� ������ �õ尪�� ����ϰ�, �׷��� ������ ������ �õ尪�� ����մϴ�.
        if (seed == 0)
        {
            seed = UnityEngine.Random.Range(0, 100000); // ������ �õ尪 ����
        }

        GenerateMap(); // �� ���� �޼��� ȣ��
    }

    void OnEnable()
    {
        GenerateMap(); // ������ ��忡���� Ȱ��ȭ�Ǵ� ������ �ڵ����� GenerateMap �޼��带 ȣ���Ͽ� ������ �����մϴ�.
    }


}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}