using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using Unity.Mathematics;
using Photon.Pun;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, Mesh, FalloffMap};
    public DrawMode drawMode;

    public TerrainData terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;

    [Range(0,6)]
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

    public GameObject[] buildingPrefabs; // �ǹ� ������ �迭
    public GameObject[] nodePrefabs;
    public MeshCollider placementArea;
    public int maxBuildings = 40;
    public LayerMask groundLayer; // ���� ���̾� ����ũ

    public float irregularity = 2f;

    public float BuildingYMin;
    public float BuildingYMax;
    public float NodedirtYMin;
    public float NodedirtYMax;
    public float NodesandYMin;
    public float NodesandYMax;

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

    public GameObject[] lowNoisePrefabs; // ���� ���� ������ �迭
    public GameObject[] mediumNoisePrefabs; // �߰� ���� ������ �迭
    public GameObject[] highNoisePrefabs; // ���� ���� ������ �迭

    public GameObject[] dirtlowNodePrefabs;
    public GameObject[] dirtmediumNodePrefabs;
    public GameObject[] dirthighNodePrefabs;

    public GameObject[] sandlowNodePrefabs;
    public GameObject[] sandmediumNodePrefabs;
    public GameObject[] sandhighNodePrefabs;


    public GameObject[] POIPrefabs;

    //public GameObject selectedPrefab;

    public float groundThreshold = 10.0f;

    void PlaceBuildings(float[,] noiseMap)
    {
        System.Random prng = new System.Random(seed1); // Initialize random number generator with the same seed

        for (int y = 12; y < mapChunkSize; y += 24)
        {
            for (int x = 12; x < mapChunkSize; x += 24)
            {
                Vector3 position = new Vector3(x-120, 50, y-120); // Adjust the y value if needed
                // heightThreshold�� ���� �ٸ� ������ �迭 ����
                GameObject prefabToPlace = null;

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

                if (prefabToPlace != null)
                {
                    // �ǹ� ����
                    GameObject newBuilding = Instantiate(prefabToPlace, position, Quaternion.identity);

                    // �ǹ��� ���鿡 ���̱�
                    if (PositionBuildingOnGround(newBuilding, BuildingYMin, BuildingYMax))
                    {
                        // ���鿡 ���������� ��ġ�� ��쿡�� �ǹ��� Ȱ��ȭ
                        newBuilding.SetActive(true);
                    }
                    else
                    {
                        // ������ ��� �ǹ� �ı�
                        Destroy(newBuilding);
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
                Vector3 position = new Vector3(x - 120, 50, y - 120); // Adjust the y value if needed

                GameObject prefabToPlace = null;

                // heightThreshold�� ���� �ٸ� ������ �迭 ����
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

                if (prefabToPlace != null)
                {
                    GameObject newNode = Instantiate(prefabToPlace, position, Quaternion.identity);

                    if (PositionBuildingOnGround(newNode, NodedirtYMin, NodedirtYMax))
                    {
                        newNode.SetActive(true);
                    }
                    else
                    {
                        Destroy(newNode);
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
                Vector3 position = new Vector3(x - 120, 50, y - 120); // Adjust the y value if needed

                GameObject prefabToPlace = null;

                // heightThreshold�� ���� �ٸ� ������ �迭 ����
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
                if (prefabToPlace != null)
                {
                    GameObject newNode = Instantiate(prefabToPlace, position, Quaternion.identity);

                    if (PositionBuildingOnGround(newNode, NodesandYMin, NodesandYMax)) 
                    {
                        newNode.SetActive(true);
                    }
                    else
                    {
                        Destroy(newNode);
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
                Vector3 position = new Vector3(x, 0, y); // Adjust the y value if needed

                // heightThreshold�� ���� �ٸ� ������ �迭 ����
                GameObject prefabToPlace = null;
                if (noiseMap[x, y] > POIThreshold1)
                {
                    prefabToPlace = POIPrefabs[prng.Next(POIPrefabs.Length)];
                }
                else if (noiseMap[x, y] > POIThreshold2)
                {
                    prefabToPlace = POIPrefabs[prng.Next(POIPrefabs.Length)];
                }
                else if (noiseMap[x, y] > POIThreshold3)
                {
                    prefabToPlace = POIPrefabs[prng.Next(POIPrefabs.Length)];
                }
                else if (noiseMap[x, y] > POIThreshold4)
                {
                    prefabToPlace = POIPrefabs[prng.Next(POIPrefabs.Length)];
                }
                else if (noiseMap[x, y] > POIThreshold5)
                {
                    prefabToPlace = POIPrefabs[prng.Next(POIPrefabs.Length)];
                }
                else if (noiseMap[x, y] > POIThreshold6)
                {
                    prefabToPlace = POIPrefabs[prng.Next(POIPrefabs.Length)];
                }

                Instantiate(prefabToPlace, position, Quaternion.identity);
            }
        }
    }

    // �־��� ���� ������ ������ ��ġ�� ��ȯ�ϴ� �޼���
    private Vector3 GetRandomPositionInBounds(Bounds bounds)
    {
        float randomX = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(randomX, 0, randomZ); // y ���� 0���� ����
    }


    public void PlaceBuildingsOnTop(float minHeight, Vector3[] fixedPositions)
    {
        Vector3[] topPositions = GetTopPositionsOnMesh(placementArea, minHeight);

        // �������� ������ ��ġ�� �ε����� ������ ����Ʈ
        List<int> selectedIndices = new List<int>();
        int positionIndex = 0;

        while (selectedIndices.Count < maxBuildings && positionIndex < fixedPositions.Length)
        {
            // ������ ��ġ���� ���� ����� topPositions�� �ε����� ã��
            float minDistance = float.MaxValue;
            int closestIndex = -1;
            for (int i = 0; i < topPositions.Length; i++)
            {
                float distance = Vector3.Distance(fixedPositions[positionIndex], topPositions[i]);
                if (distance < minDistance && !selectedIndices.Contains(i))
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            if (closestIndex != -1)
            {
                selectedIndices.Add(closestIndex);
            }

            positionIndex++;
        }

        foreach (int index in selectedIndices)
        {
            // ������ ������ ����
            GameObject selectedPrefab = buildingPrefabs[UnityEngine.Random.Range(0, buildingPrefabs.Length)];

            // �ǹ� ����
            GameObject newBuilding = Instantiate(selectedPrefab, topPositions[index], Quaternion.identity);

            // �ǹ��� ���鿡 ���̱�
            PositionBuildingOnGround(newBuilding, BuildingYMin, BuildingYMax);
        }
    }
    // �־��� �޽� �ݶ��̴� ǥ�鿡�� ���� ���� ��ġ���� ��ȯ�ϴ� �޼���
    private Vector3[] GetTopPositionsOnMesh(MeshCollider meshCollider, float minHeight)
    {
        Mesh mesh = meshCollider.sharedMesh;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        // ���� ���� ��ġ��(Ȥ�� �� ��ó ��ġ��)�� ������ ����Ʈ�� ����ϴ�.
        List<Vector3> topPositions = new List<Vector3>();

        // Ư�� y�� �̻��� ��� ������ ã�� ����Ʈ�� �߰��մϴ�.
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 vertex1 = meshCollider.transform.TransformPoint(vertices[triangles[i]]);
            Vector3 vertex2 = meshCollider.transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 vertex3 = meshCollider.transform.TransformPoint(vertices[triangles[i + 2]]);

            if (vertex1.y >= minHeight) topPositions.Add(vertex1);
            if (vertex2.y >= minHeight) topPositions.Add(vertex2);
            if (vertex3.y >= minHeight) topPositions.Add(vertex3);
        }

        // ���������� ����Ʈ�� �迭�� ��ȯ�Ͽ� ��ȯ�մϴ�.
        return topPositions.ToArray();
    }

    // �ǹ��� ���鿡 ���̴� �޼���
    private bool PositionBuildingOnGround(GameObject building, float minY, float maxY)
    {
        // �ǹ��� ��ġ���� �Ʒ��� Raycast�� ����Ͽ� ���� ã��
        RaycastHit hit;
        if (Physics.Raycast(building.transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            // ������ ã���� �ǹ��� ��ġ�� �ش� �������� �̵�
            if (hit.point.y >= minY && hit.point.y <= maxY)
            {
                building.transform.position = hit.point;
                return true;
            }
            else
            {
                Debug.LogWarning("������ ���� ���� ���� �ʽ��ϴ�. �ǹ��� �ʱ� ��ġ�� ���ܵӴϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("������ ã�� �� �����ϴ�. �ǹ��� �ʱ� ��ġ�� ���ܵӴϴ�.");
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

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
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
            MeshDataThread(mapData,lod, callback);
        };
        
        new Thread(threadStart).Start();
    }

    void MeshDataThread (MapData mapData, int lod, Action<MeshData> callback) 
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, terrainData.useFlatShading);
        lock(meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData> (callback, meshData));
        }
    }

    void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        
        if(meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0;i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData(Vector2 center)
    {
        // ������ �� ����
        float[,] noiseMap1 = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseData.noiseScale1,
            noiseData.octaves1, noiseData.persistance, noiseData.lacunarity, center + offset1, noiseData.normalizedMode);

        // �߰����� ������ �� ����
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
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);

        return new MapData(noiseMap);
    }

    void OnValidate()
    {
        if(terrainData != null)
        {
            terrainData.OnValueUpdated -= OnValuesUpdated;
            terrainData.OnValueUpdated += OnValuesUpdated;
        }
        if(noiseData != null)
        {
            noiseData.OnValueUpdated -= OnValuesUpdated;
            noiseData.OnValueUpdated += OnValuesUpdated;
        }
        if(textureData != null)
        {
            textureData.OnValueUpdated -= OnValuesUpdated;
            textureData.OnValueUpdated += OnValuesUpdated;
        }
    }
    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo (Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
    void Start() 
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

        if (placementArea != null && buildingPrefabs.Length > 0)
        {
            float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseData.noiseScale1 , irregularity, irregularityoffset);
            PlaceBuildings(irregularNoiseMap);
        }
        else
        {
            Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
        }


        DrawMapInEditor(); // �� ���� �޼��� ȣ��
    }

    void OnEnable()
    {
        DrawMapInEditor(); // ������ ��忡���� Ȱ��ȭ�Ǵ� ������ �ڵ����� GenerateMap �޼��带 ȣ���Ͽ� ������ �����մϴ�.
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
