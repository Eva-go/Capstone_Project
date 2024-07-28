using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using Unity.Mathematics;
using Photon.Pun;
using UnityEngine.UIElements;
using Unity.Burst.CompilerServices;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, Mesh, FalloffMap };
    public DrawMode drawMode;

    public TerrainData terrainData;
    public NoiseData noiseData;
    //public TextureData textureData;

    public Material terrainMaterial;

    [Range(0, 6)]
    public int editorPreviewLOD;

    public int seed1;
    public int seed2;

    public Vector2 offset1;
    public Vector2 offset2;
    public Vector2 irregularityoffset;

    public float minHeight;

    public bool autoUpdate;

    float[,] falloffMap;

    [Range(0, 1)]
    public float blendStrength;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public GameObject[] buildingPrefabs; // 건물 프리팹 배열
    public GameObject[] nodePrefabs;
    public GameObject[] POIPrefabs;
    public MeshCollider placementArea;
    public int maxBuildings = 40;
    public LayerMask groundLayer; // 지면 레이어 마스크

    // 빈 게임 오브젝트의 Transform
    public Transform parentTransform;

    public float irregularity = 2f;

    public float BuildingYMin;
    public float BuildingYMax;
    public float NodedirtYMin;
    public float NodedirtYMax;
    public float NodesandYMin;
    public float NodesandYMax;
    public float POIYMin;
    public float POIYMax;

    [Range(0, 1)]
    public float BuildingThreshold1 = 0.8f;
    [Range(0, 1)]
    public float BuildingThreshold2 = 0.6f;
    [Range(0, 1)]
    public float BuildingThreshold3 = 0.4f;
    [Range(0, 1)]
    public float NodedirtThreshold1 = 0.9f;
    [Range(0, 1)]
    public float NodedirtThreshold2 = 0.7f;
    [Range(0, 1)]
    public float NodedirtThreshold3 = 0.5f;
    [Range(0, 1)]
    public float POIThreshold1 = 0.9f;
    [Range(0, 1)]
    public float POIThreshold2 = 0.7f;
    [Range(0, 1)]
    public float POIThreshold3 = 0.5f;
    [Range(0, 1)]
    public float POIThreshold4 = 0.5f;
    [Range(0, 1)]
    public float POIThreshold5 = 0.3f;
    [Range(0, 1)]
    public float POIThreshold6 = 0.1f;

    public GameObject[] lowNoisePrefabs; // 낮은 높이 프리팹 배열
    public GameObject[] mediumNoisePrefabs; // 중간 높이 프리팹 배열
    public GameObject[] highNoisePrefabs; // 높은 높이 프리팹 배열

    public GameObject[] dirtlowNodePrefabs;
    public GameObject[] dirtmediumNodePrefabs;
    public GameObject[] dirthighNodePrefabs;

    public GameObject[] sandlowNodePrefabs;
    public GameObject[] sandmediumNodePrefabs;
    public GameObject[] sandhighNodePrefabs;

    public GameObject[] POIPrefabs1;
    public GameObject[] POIPrefabs2;
    public GameObject[] POIPrefabs3;
    public GameObject[] POIPrefabs4;
    public GameObject[] POIPrefabs5;
    public GameObject[] POIPrefabs6;

    //public GameObject selectedPrefab;

    public float groundThreshold = 10.0f;

    void PlaceBuildings(float[,] noiseMap)
    {
        System.Random prng = new System.Random(seed1); // Initialize random number generator with the same seed

        for (int y = 12; y < mapChunkSize; y += 24)
        {
            for (int x = 12; x < mapChunkSize; x += 24)
            {
                Vector3 position = new Vector3(x - 120, 0, y - 120); // Adjust the y value if needed\
                RaycastHit hit;
                if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
                {
                    GameObject prefabToPlace = null;
                    float height = hit.point.y;
                    if (height > BuildingYMin)
                    {
                        if (height <= BuildingYMax)
                        {
                            position = new Vector3(x - 120, height, y - 120);
                            if (noiseMap[x, y] > BuildingThreshold1)
                            {
                                prefabToPlace = highNoisePrefabs[prng.Next(highNoisePrefabs.Length)];
                            }
                            else if (noiseMap[x, y] > BuildingThreshold2)
                            {
                                prefabToPlace = mediumNoisePrefabs[prng.Next(mediumNoisePrefabs.Length)];
                            }
                            else if (noiseMap[x, y] > BuildingThreshold3)
                            {
                                prefabToPlace = lowNoisePrefabs[prng.Next(lowNoisePrefabs.Length)];
                            }
                        }
                    }

                    if (prefabToPlace != null)
                    {
                        GameObject newBliding = PhotonNetwork.Instantiate(prefabToPlace.name, position, Quaternion.identity);
                        newBliding.transform.SetParent(parentTransform);
                    }
                }
            }
        }
    }

    void PlaceDirtNodes(float[,] noiseMap)
    {
        System.Random prng = new System.Random(seed1); // Initialize random number generator with the same seed

        for (int y = 0; y < mapChunkSize; y += 1)
        {
            for (int x = 0; x < mapChunkSize; x += 1)
            {
                Vector3 position = new Vector3(x - 120, 0, y - 120); // Adjust the y value if needed

                RaycastHit hit;
                if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
                {
                    GameObject prefabToPlace = null;
                    float height = hit.point.y;
                    if (height > NodedirtYMin)
                    {
                        if (height <= NodedirtYMax)
                        {
                            position = new Vector3(x - 120, height, y - 120);
                            // heightThreshold에 따라 다른 프리팹 배열 선택
                            if (noiseMap[x, y] > NodedirtThreshold1)
                            {
                                prefabToPlace = dirtlowNodePrefabs[prng.Next(dirtlowNodePrefabs.Length)];
                            }
                            else if (noiseMap[x, y] > NodedirtThreshold2)
                            {
                                prefabToPlace = dirtmediumNodePrefabs[prng.Next(dirtmediumNodePrefabs.Length)];
                            }
                            else if (noiseMap[x, y] > NodedirtThreshold3)
                            {
                                prefabToPlace = dirthighNodePrefabs[prng.Next(dirthighNodePrefabs.Length)];
                            }
                        }
                    }
                            

                    if (prefabToPlace != null)
                    {
                        GameObject newNode = PhotonNetwork.Instantiate(prefabToPlace.name, position, Quaternion.identity);
                        newNode.transform.SetParent(parentTransform);
                    }
                }
            }
        }
    }

    void PlaceSandNodes(float[,] noiseMap)
    {
        System.Random prng = new System.Random(seed1); // Initialize random number generator with the same seed

        for (int y = 0; y < mapChunkSize; y += 1)
        {
            for (int x = 0; x < mapChunkSize; x += 1)
            {
                Vector3 position = new Vector3(x - 120, 0, y - 120); // Adjust the y value if needed

                RaycastHit hit;
                if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
                {
                    GameObject prefabToPlace = null;
                    float height = hit.point.y;
                    if (height > NodesandYMin)
                    {
                        if (height <= NodesandYMax)
                        {
                            position = new Vector3(x - 120, height, y - 120);
                            // heightThreshold에 따라 다른 프리팹 배열 선택
                            if (noiseMap[x, y] > NodedirtThreshold1)
                            {
                                prefabToPlace = sandlowNodePrefabs[prng.Next(sandlowNodePrefabs.Length)];
                            }
                            else if (noiseMap[x, y] > NodedirtThreshold2)
                            {
                                prefabToPlace = sandmediumNodePrefabs[prng.Next(sandmediumNodePrefabs.Length)];
                            }
                            else if (noiseMap[x, y] > NodedirtThreshold3)
                            {
                                prefabToPlace = sandhighNodePrefabs[prng.Next(sandhighNodePrefabs.Length)];
                            }
                        }
                    }

                    if (prefabToPlace != null)
                    {
                        GameObject newNode = PhotonNetwork.Instantiate(prefabToPlace.name, position, Quaternion.identity);
                        newNode.transform.SetParent(parentTransform);
                    }
                }
            }
        }
    }

    void PlacePOIs(float[,] noiseMap)
    {
        System.Random prng = new System.Random(seed1); // Initialize random number generator with the same seed

        for (int y = 0; y < mapChunkSize; y += 10)
        {
            for (int x = 0; x < mapChunkSize; x += 10)
            {
                Vector3 position = new Vector3(x - 120, 0, y - 120); // Adjust the y value if needed

                RaycastHit hit;
                if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
                {
                    GameObject prefabToPlace = null;
                    float height = hit.point.y;
                    position = new Vector3(x - 120, height, y - 120);
                    if (height > POIYMin)
                    {
                        if (height <= POIYMax)
                        {

                            if (noiseMap[x, y] > POIThreshold1)
                            {
                                prefabToPlace = POIPrefabs1[prng.Next(POIPrefabs1.Length)];
                            }
                            else if (noiseMap[x, y] > POIThreshold2)
                            {
                                prefabToPlace = POIPrefabs2[prng.Next(POIPrefabs2.Length)];
                            }
                            else if (noiseMap[x, y] > POIThreshold3)
                            {
                                prefabToPlace = POIPrefabs3[prng.Next(POIPrefabs3.Length)];
                            }
                            else if (noiseMap[x, y] > POIThreshold4)
                            {
                                prefabToPlace = POIPrefabs4[prng.Next(POIPrefabs4.Length)];
                            }
                            else if (noiseMap[x, y] > POIThreshold5)
                            {
                                prefabToPlace = POIPrefabs5[prng.Next(POIPrefabs5.Length)];
                            }
                            else if (noiseMap[x, y] > POIThreshold6)
                            {
                                prefabToPlace = POIPrefabs6[prng.Next(POIPrefabs6.Length)];
                            }
                        }
                    }

                    if (prefabToPlace != null)
                    {
                        GameObject newPOI = PhotonNetwork.Instantiate(prefabToPlace.name, position, Quaternion.identity);
                        newPOI.transform.SetParent(parentTransform);
                    }
                }
            }
        }
    }
    // 건물을 지면에 붙이는 메서드
    private bool PositionBuildingOnGround(GameObject building, float minY, float maxY)
    {
        // 건물의 위치에서 아래로 Raycast를 사용하여 지면 찾기
        RaycastHit hit;
        if (Physics.Raycast(building.transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            // 지면을 찾으면 건물의 위치를 해당 지점으로 이동
            if (hit.point.y > minY)
            {
                if (hit.point.y <= maxY)
                {
                    building.transform.position = hit.point;
                    return true;
                }
            }
            else
            {
                Debug.LogWarning("지면이 범위 내에 있지 않습니다. 건물을 초기 위치에 남겨둡니다.");
            }
        }
        else
        {
            Debug.LogWarning("지면을 찾을 수 없습니다. 건물을 초기 위치에 남겨둡니다.");
        }
        return false;
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLOD, terrainData.useFlatShading));
        }

    }

    void Awake()
    {
        seed1 = GameValue.seed1;
        seed2 = GameValue.seed2;
        Debug.Log("Map: " + seed1);
        Debug.Log("Map: " + seed2);
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    public int mapChunkSize
    {
        get
        {
            if (terrainData.useFlatShading)
            {
                return 95;
            }
            else
            {
                return 239;
            }
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(center);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, terrainData.useFlatShading);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData(Vector2 center)
    {
        // 노이즈 맵 생성
        float[,] noiseMap1 = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseData.noiseScale1,
            noiseData.octaves1, noiseData.persistance, noiseData.lacunarity, center + offset1, noiseData.normalizedMode);

        // 추가적인 노이즈 맵 생성
        float[,] noiseMap2 = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed2, noiseData.noiseScale2,
            noiseData.octaves2, noiseData.persistance, noiseData.lacunarity, center + offset2, noiseData.normalizedMode);

        float[,] IrregularNoiseMap = Noise.GenerateIrregularNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseData.noiseScale1,
            irregularity, center + irregularityoffset);

        float[,] noiseMap = Noise.AddNoise(noiseMap1, noiseMap2, blendStrength);

        if (terrainData.useFalloff)
        {
            if (falloffMap == null)
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize + 2);
            }
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    if (terrainData.useFalloff)
                    {
                        noiseMap[x, y] = Mathf.Lerp(noiseMap[x, y], falloffMap[x, y], 0.5f);
                    }
                }
            }
        }

        return new MapData(noiseMap);
    }

    void OnValidate()
    {
        if (terrainData != null)
        {
            terrainData.OnValueUpdated -= OnValuesUpdated;
            terrainData.OnValueUpdated += OnValuesUpdated;
        }
        if (noiseData != null)
        {
            noiseData.OnValueUpdated -= OnValuesUpdated;
            noiseData.OnValueUpdated += OnValuesUpdated;
        }
    }
    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (placementArea != null && nodePrefabs.Length > 0)
            {
                float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseData.noiseScale1, irregularity, irregularityoffset);
                PlaceDirtNodes(irregularNoiseMap);
                PlaceSandNodes(irregularNoiseMap);
            }
            else
            {
                Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
            }

            if (placementArea != null && POIPrefabs.Length > 0)
            {
                float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseData.noiseScale1, irregularity, irregularityoffset);
                PlacePOIs(irregularNoiseMap);
            }
            else
            {
                Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
            }

            if (placementArea != null && buildingPrefabs.Length > 0)
            {
                float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseData.noiseScale1, irregularity, irregularityoffset);
                PlaceBuildings(irregularNoiseMap);
            }
            else
            {
                Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
            }

            DrawMapInEditor(); // 맵 생성 메서드 호출
        }
    }

    void OnEnable()
    {
        DrawMapInEditor(); // 에디터 모드에서는 활성화되는 시점에 자동으로 GenerateMap 메서드를 호출하여 색상을 적용합니다.
    }


}

public struct MapData
{
    public readonly float[,] heightMap;

    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}
