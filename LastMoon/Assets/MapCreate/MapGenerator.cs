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

    //public Texture2D gradientTexture; // 그라디언트 텍스처
    //public Gradient gradient; // 그라디언트

    //public int maxCubeCount = 1000; // 최대 큐브 개수
    //private int cubeCount = 0; // 현재 생성된 큐브 개수
    //public GameObject cubePrefab; // 큐브 프리팹

    void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
    public void GenerateMap()
    {
        // 노이즈 맵 생성
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize+2, mapChunkSize+2, seed, noiseScale,
            octaves, persistance, lacunarity, offset);

        // 컬러 맵 생성
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

        // 지도 표시
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
        //                        // 큐브 개수가 최대 개수를 초과하지 않는지 확인합니다.
        //                        if (cubeCount < maxCubeCount)
        //                        {
        //                            // 큐브를 설치할 위치 계산
        //                            Vector3 cubePosition = new Vector3(x + Random.Range(-0.5f, 0.5f), currentHeight, y + Random.Range(-0.5f, 0.5f));
        //                            // 큐브 생성
        //                            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);

        //                            // 큐브 크기 설정 (옵션)
        //                            cube.transform.localScale = new Vector3(18, 64, 18);

        //                            // 생성된 큐브 개수 증가
        //                            cubeCount++;
        //                        }
        //                        else
        //                        {
        //                            Debug.LogWarning("최대 큐브 개수에 도달했습니다.");
        //                            return; // 최대 개수에 도달하면 더 이상 큐브를 생성하지 않음
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
        //        return regions[regions.Length - 1]; // 기본값으로 마지막 지형 유형 반환
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
        // seed가 0이면 랜덤한 시드값을 사용하고, 그렇지 않으면 지정된 시드값을 사용합니다.
        if (seed == 0)
        {
            seed = UnityEngine.Random.Range(0, 100000); // 랜덤한 시드값 생성
        }

        GenerateMap(); // 맵 생성 메서드 호출
    }

    void OnEnable()
    {
        GenerateMap(); // 에디터 모드에서는 활성화되는 시점에 자동으로 GenerateMap 메서드를 호출하여 색상을 적용합니다.
    }


}
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}