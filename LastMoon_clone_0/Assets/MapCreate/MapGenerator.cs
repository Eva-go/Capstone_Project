using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using Unity.Mathematics;
using Photon.Pun;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh, FalloffMap};
    public DrawMode drawMode;
    
    public Noise.NormalizedMode normalizedMode;
    public const int mapChunkSize = 239;
    [Range(0,6)]
    public int editorPreviewLOD;
    public float noiseScale1;
    public float noiseScale2;

    [Range (0,10)]
    public int octaves1;
    [Range(0, 10)]
    public int octaves2;


    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed1;
    public int seed2;
    public Vector2 offset1;
    public Vector2 offset2;

    public bool useFalloff;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    float[,] falloffMap;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    public GameObject[] buildingPrefabs; // �ǹ� ������ �迭
    public MeshCollider placementArea;
    public int maxBuildings = 40;
    public LayerMask groundLayer; // ���� ���̾� ����ũ

    public GameObject[] NodePrefabs; // �ǹ� ������ �迭

    //public GameObject selectedPrefab;

    public void PlaceNodes(int numberOfNodes)
    {
        int nodesPerGroup = 5; // �� �׷�� ������ ��� ��
        int groups = numberOfNodes / nodesPerGroup; // �׷� ��
        int remainder = numberOfNodes % nodesPerGroup; // �׷캰 ���� ��� ��

        for (int i = 0; i < groups; i++)
        {
            PlaceNodeGroup(nodesPerGroup); // ��� �׷� ����
        }

        // ���� ��带 ���� ����
        if (remainder > 0)
        {
            PlaceNodeGroup(remainder);
        }
    }

    private void PlaceNodeGroup(int numberOfNodes)
    {
        for (int i = 0; i < numberOfNodes; i++)
        {
            // ������ ������ ����
            GameObject selectedPrefab = NodePrefabs[UnityEngine.Random.Range(0, NodePrefabs.Length)];

            // ������ ��ġ ����
            Vector3 randomPosition = GetRandomPositionInBounds(placementArea.bounds);

            // ���� ǥ�� ���̸� ����Ͽ� ��带 ��ġ
            if (Physics.Raycast(new Vector3(randomPosition.x, 1000, randomPosition.z), Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                randomPosition.y = hit.point.y;

                // �ǹ� ����
                GameObject newBuilding = Instantiate(selectedPrefab, randomPosition, Quaternion.identity);
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


    public void PlaceBuildingsOnTop()
    {
        Vector3[] topPositions = GetTopPositionsOnMesh(placementArea);

        // �������� ������ ��ġ�� �ε����� ������ ����Ʈ
        List<int> selectedIndices = new List<int>();
        while (selectedIndices.Count < maxBuildings && selectedIndices.Count < topPositions.Length)
        {
            int randomIndex = UnityEngine.Random.Range(0, topPositions.Length);
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedIndices.Add(randomIndex);
            }
        }

        foreach (int index in selectedIndices)
        {
            // ������ ������ ����
            GameObject selectedPrefab = buildingPrefabs[UnityEngine.Random.Range(0, buildingPrefabs.Length)];

            // �ǹ� ����
            GameObject newBuilding = Instantiate(selectedPrefab, topPositions[index], Quaternion.identity);

            // �ǹ��� ���鿡 ���̱�
            PositionBuildingOnGround(newBuilding);
        }
    }
    // �־��� �޽� �ݶ��̴� ǥ�鿡�� ���� ���� ��ġ���� ��ȯ�ϴ� �޼���
    private Vector3[] GetTopPositionsOnMesh(MeshCollider meshCollider)
    {
        Mesh mesh = meshCollider.sharedMesh;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        float highestY = float.NegativeInfinity;

        // ���� ���� ���� y ���� ã���ϴ�.
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 vertex1 = meshCollider.transform.TransformPoint(vertices[triangles[i]]);
            Vector3 vertex2 = meshCollider.transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 vertex3 = meshCollider.transform.TransformPoint(vertices[triangles[i + 2]]);

            if (vertex1.y > highestY) highestY = vertex1.y;
            if (vertex2.y > highestY) highestY = vertex2.y;
            if (vertex3.y > highestY) highestY = vertex3.y;
        }

        // ���� ���� ��ġ��(Ȥ�� �� ��ó ��ġ��)�� ������ ����Ʈ�� ����ϴ�.
        List<Vector3> topPositions = new List<Vector3>();

        // ���� ���� y �� �̻��� ��� ������ ã�� ����Ʈ�� �߰��մϴ�.
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 vertex1 = meshCollider.transform.TransformPoint(vertices[triangles[i]]);
            Vector3 vertex2 = meshCollider.transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 vertex3 = meshCollider.transform.TransformPoint(vertices[triangles[i + 2]]);

            if (vertex1.y >= highestY) topPositions.Add(vertex1);
            if (vertex2.y >= highestY) topPositions.Add(vertex2);
            if (vertex3.y >= highestY) topPositions.Add(vertex3);
        }

        // ���������� ����Ʈ�� �迭�� ��ȯ�Ͽ� ��ȯ�մϴ�.
        return topPositions.ToArray();
    }

    // �ǹ��� ���鿡 ���̴� �޼���
    private void PositionBuildingOnGround(GameObject building)
    {
        Collider buildingCollider = building.GetComponent<Collider>();
        if (buildingCollider != null)
        {
            Vector3 colliderBottomCenter = buildingCollider.bounds.center - new Vector3(0, buildingCollider.bounds.extents.y, 0);
            Ray ray = new Ray(colliderBottomCenter + Vector3.up * 10, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 20, groundLayer))
            {
                building.transform.position = hit.point;
            }
            else
            {
                Debug.LogWarning("No ground detected below the building.");
            }
        }
        else
        {
            Debug.LogWarning("Building does not have a collider.");
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
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD), TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }

    }

    void Awake()
    {
        seed1 = Seed.seed1;
        seed2 = Seed.seed2;
        Debug.Log("Map: " + seed1);
        Debug.Log("Map: " + seed2);
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
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
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
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
        float[,] noiseMap1 = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed1, noiseScale1,
            octaves1, persistance, lacunarity, center + offset1, normalizedMode);

        // �߰����� ������ �� ����
        float[,] noiseMap2 = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed2, noiseScale2,
            octaves2, persistance, lacunarity, center + offset2, normalizedMode);

        float[,] noiseMap = Noise.AddNoise(noiseMap1, noiseMap2, 0.5f);

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
                    if (currentHeight >= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colourMap);
    }

    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves1 < 0)
        {
            octaves1 = 0;
        }
        if (octaves2 < 0)
        {
            octaves2 = 0;
        }
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
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



        if (placementArea != null && NodePrefabs.Length > 0)
        {
            //PlaceNodes(1000);
        }
        else
        {
            Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
        }

        if (placementArea != null && buildingPrefabs.Length > 0)
        {
            //PlaceBuildingsOnTop(); // �ǹ��� ����� ���� ��ġ
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
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
public struct MapData
{
    public readonly float[,] heightMap; 
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}
