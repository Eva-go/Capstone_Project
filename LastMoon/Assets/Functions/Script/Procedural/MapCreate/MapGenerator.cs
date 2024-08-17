using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using Unity.Mathematics;
using Photon.Pun;
using UnityEngine.UIElements;
using Unity.Burst.CompilerServices;
using System.Threading.Tasks;
using Unity.Collections;
using static UnityEngine.Mesh;

public class MapGenerator : MonoBehaviour
{
    [ReadOnly]
    public static bool useFlatShading = false;

    public enum DrawMode { NoiseMap, Mesh, FalloffMap };
    public DrawMode drawMode;

    public MapTerrainData terrainData;
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

    float nodeSpawnHeight = 0.5f;
    float buildingSpawnHeight = 0.7f;
    float POISpawnHeight = 0.9f;

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

    public int chunks = 9;
    public int chunkSize = 10;
    public GameObject chunkPrefab;
    public GameObject player;

    private Vector3 spawnPoint;
    private Queue<MeshData> meshDataQueue = new Queue<MeshData>();

    private Dictionary<Vector2Int, GameObject> chunksDictionary = new Dictionary<Vector2Int, GameObject>();

    void PlaceBuildings(float[,] noiseMap, MeshData meshData, GameObject[] highNoisePrefabs, GameObject[] mediumNoisePrefabs, GameObject[] lowNoisePrefabs)
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
                    GameObject[] prefabsToUse = null;
                    float height = hit.point.y;
                    if (height > BuildingYMin)
                    {
                        if (height <= BuildingYMax)
                        {
                            position = new Vector3(x - 120, height, y - 120);
                            if (noiseMap[x, y] > BuildingThreshold1)
                            {
                                prefabsToUse = highNoisePrefabs;
                            }
                            else if (noiseMap[x, y] > BuildingThreshold2)
                            {
                                prefabsToUse = mediumNoisePrefabs;
                            }
                            else if (noiseMap[x, y] > BuildingThreshold3)
                            {
                                prefabsToUse = lowNoisePrefabs;
                            }
                        }
                    }

                    if (prefabsToUse != null)
                    {
                        // Add the selected prefabs to the MeshData
                        meshData.AddObjectToSpawn(prefabsToUse, position);

                        // Instantiate and place each prefab
                        foreach (var prefab in prefabsToUse)
                        {
                            GameObject newBuilding = PhotonNetwork.Instantiate(prefab.name, position, Quaternion.identity);
                            newBuilding.transform.SetParent(parentTransform);
                        }
                    }
                }
            }
        }
    }

    void PlaceDirtNodes(float[,] noiseMap, MeshData meshData, GameObject[] dirtlowNodePrefabs, GameObject[] dirtmediumNodePrefabs, GameObject[] dirthighNodePrefabs)
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
                    GameObject[] prefabsToUse = null;
                    float height = hit.point.y;
                    if (height > NodedirtYMin)
                    {
                        if (height <= NodedirtYMax)
                        {
                            position = new Vector3(x - 120, height, y - 120);
                            // heightThreshold에 따라 다른 프리팹 배열 선택
                            if (noiseMap[x, y] > NodedirtThreshold1)
                            {
                                prefabsToUse = dirtlowNodePrefabs;
                            }
                            else if (noiseMap[x, y] > NodedirtThreshold2)
                            {
                                prefabsToUse = dirtmediumNodePrefabs;
                            }
                            else if (noiseMap[x, y] > NodedirtThreshold3)
                            {
                                prefabsToUse = dirthighNodePrefabs;
                            }
                        }
                    }


                    if (prefabsToUse != null)
                    {
                        // Add the selected prefabs to the MeshData
                        meshData.AddObjectToSpawn(prefabsToUse, position);

                        // Instantiate and place each prefab
                        foreach (var prefab in prefabsToUse)
                        {
                            GameObject newBuilding = PhotonNetwork.Instantiate(prefab.name, position, Quaternion.identity);
                            newBuilding.transform.SetParent(parentTransform);
                        }
                    }

                }
            }
        }
    }

    void PlaceSandNodes(float[,] noiseMap, MeshData meshData, GameObject[] sandlowNodePrefabs, GameObject[] sandmediumNodePrefabs, GameObject[] sandhighNodePrefabs)
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
                    GameObject[] prefabsToUse = null;
                    float height = hit.point.y;
                    if (height > NodesandYMin)
                    {
                        if (height <= NodesandYMax)
                        {
                            position = new Vector3(x - 120, height, y - 120);
                            // heightThreshold에 따라 다른 프리팹 배열 선택
                            if (noiseMap[x, y] > NodedirtThreshold1)
                            {
                                prefabsToUse = sandlowNodePrefabs;
                            }
                            else if (noiseMap[x, y] > NodedirtThreshold2)
                            {
                                prefabsToUse = sandmediumNodePrefabs;
                            }
                            else if (noiseMap[x, y] > NodedirtThreshold3)
                            {
                                prefabsToUse = sandhighNodePrefabs;
                            }
                        }
                    }

                    if (prefabsToUse != null)
                    {
                        // Add the selected prefabs to the MeshData
                        meshData.AddObjectToSpawn(prefabsToUse, position);

                        // Instantiate and place each prefab
                        foreach (var prefab in prefabsToUse)
                        {
                            GameObject newBuilding = PhotonNetwork.Instantiate(prefab.name, position, Quaternion.identity);
                            newBuilding.transform.SetParent(parentTransform);
                        }
                    }
                }
            }
        }
    }

    void PlacePOIs(float[,] noiseMap, MeshData meshData, GameObject[] POIPrefabs1, GameObject[] POIPrefabs2, GameObject[] POIPrefabs3, GameObject[] POIPrefabs4, GameObject[] POIPrefabs5, GameObject[] POIPrefabs6)
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
                    GameObject[] prefabsToUse = null;
                    float height = hit.point.y;
                    position = new Vector3(x - 120, height, y - 120);
                    if (height > POIYMin)
                    {
                        if (height <= POIYMax)
                        {

                            if (noiseMap[x, y] > POIThreshold1)
                            {
                                prefabsToUse = POIPrefabs1;
                            }
                            else if (noiseMap[x, y] > POIThreshold2)
                            {
                                prefabsToUse = POIPrefabs2;
                            }
                            else if (noiseMap[x, y] > POIThreshold3)
                            {
                                prefabsToUse = POIPrefabs3;
                            }
                            else if (noiseMap[x, y] > POIThreshold4)
                            {
                                prefabsToUse = POIPrefabs4;
                            }
                            else if (noiseMap[x, y] > POIThreshold5)
                            {
                                prefabsToUse = POIPrefabs5;
                            }
                            else if (noiseMap[x, y] > POIThreshold6)
                            {
                                prefabsToUse = POIPrefabs6;
                            }
                        }
                    }

                    if (prefabsToUse != null)
                    {
                        // Add the selected prefabs to the MeshData
                        meshData.AddObjectToSpawn(prefabsToUse, position);

                        // Instantiate and place each prefab
                        foreach (var prefab in prefabsToUse)
                        {
                            GameObject newBuilding = PhotonNetwork.Instantiate(prefab.name, position, Quaternion.identity);
                            newBuilding.transform.SetParent(parentTransform);
                        }
                    }
                }
            }
        }
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
                return 479;
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
        if (meshDataQueue.Count > 0)
        {
            MeshData meshData = meshDataQueue.Dequeue();
            CreateChunk(meshData.position, meshData);
        }
    }

    void GenerateChunks(Vector2Int centerChunk)
    {
        for (int z = -chunks / 2; z < chunks / 2; z++)
        {
            for (int x = -chunks / 2; x < chunks / 2; x++)
            {
                Vector2Int chunkCoord = new Vector2Int(x, z) + centerChunk;
                GenerateChunk(chunkCoord);
            }
        }
    }

    void GenerateChunk(Vector2Int chunkCoord)
    {
        if (!chunksDictionary.ContainsKey(chunkCoord))
        {
            Vector3 chunkPosition = new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize);
            Task.Run(() =>
            {
                MeshData meshData = TerrainGenerator.GenerateTerrainMesh(chunkPosition, chunkSize, noiseData.noiseScale1, seed1, mapChunkSize, nodeSpawnHeight, buildingSpawnHeight, POISpawnHeight, nodePrefabs, buildingPrefabs, POIPrefabs);
                meshDataQueue.Enqueue(meshData);
            });
        }
    }

    void CreateChunk(Vector2Int chunkCoord, MeshData meshData)
    {
        if (!chunksDictionary.ContainsKey(chunkCoord))
        {
            GameObject chunk = Instantiate(chunkPrefab, new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize), Quaternion.identity);
            MeshFilter meshFilter = chunk.GetComponent<MeshFilter>();
            meshFilter.mesh = meshData.CreateMesh();
            chunk.transform.SetParent(transform);
            chunksDictionary.Add(chunkCoord, chunk);
        }
    }

    public class TerrainGenerator
    {
        public static MeshData GenerateTerrainMesh(Vector3 position, int chunkSize, float noiseScale, int seed, int mapSize, float nodeHeight, float buildingHeight, float POIHeight, GameObject[] nodePrefabs, GameObject[] buildingPrefabs, GameObject[] POIPrefabs)
        {
            MeshData meshData = new MeshData(chunkSize, useFlatShading);
            System.Random random = new System.Random(seed);

            for (int y = 0; y <= chunkSize; y++)
            {
                for (int x = 0; x <= chunkSize; x++)
                {
                    float worldX = position.x + x;
                    float worldZ = position.z + y;
                    float noiseValue = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale) * 20;
                    float height = noiseValue;
                    int vertexIndex = y * (chunkSize + 1) + x;
                    meshData.SetVertex(vertexIndex, new Vector3(worldX, height, worldZ));

                    GameObject[] prefabsToUse = null;

                    if (height >= nodeHeight && height < buildingHeight)
                    {
                        prefabsToUse = nodePrefabs;
                    }
                    else if (height >= buildingHeight && height < POIHeight)
                    {
                        prefabsToUse = buildingPrefabs;
                    }
                    else if (height >= POIHeight)
                    {
                        prefabsToUse = POIPrefabs;
                    }

                    if (prefabsToUse != null)
                    {
                        meshData.AddObjectToSpawn(prefabsToUse, new Vector3(worldX, height, worldZ));
                    }
                }
            }

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    int vertexIndex = y * (chunkSize + 1) + x;
                    meshData.AddTriangle(vertexIndex, vertexIndex + chunkSize + 1, vertexIndex + 1);
                    meshData.AddTriangle(vertexIndex + chunkSize + 1, vertexIndex + chunkSize + 2, vertexIndex + 1);
                }
            }

            return meshData;
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
            Vector2Int centerChunk = new Vector2Int(Mathf.RoundToInt(spawnPoint.x / chunkSize), Mathf.RoundToInt(spawnPoint.z / chunkSize));
            GenerateChunks(centerChunk);
            MeshData meshData = new MeshData(chunkSize, useFlatShading);
            if (placementArea != null && nodePrefabs.Length > 0)
            {
                float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseData.noiseScale1, irregularity, irregularityoffset);
                PlaceDirtNodes(irregularNoiseMap, meshData, highNoisePrefabs, mediumNoisePrefabs, lowNoisePrefabs);
                PlaceSandNodes(irregularNoiseMap, meshData, highNoisePrefabs, mediumNoisePrefabs, lowNoisePrefabs);
            }
            else
            {
                Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
            }

            if (placementArea != null && POIPrefabs.Length > 0)
            {
                float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseData.noiseScale1, irregularity, irregularityoffset);
                PlacePOIs(irregularNoiseMap, meshData, POIPrefabs1, POIPrefabs2, POIPrefabs3, POIPrefabs4, POIPrefabs5, POIPrefabs6);
            }
            else
            {
                Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
            }

            if (placementArea != null && buildingPrefabs.Length > 0)
            {
                float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseData.noiseScale1, irregularity, irregularityoffset);

                PlaceBuildings(irregularNoiseMap, meshData, highNoisePrefabs, mediumNoisePrefabs, lowNoisePrefabs);
                PlayerSpawn.OnBuildingCreated();
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
