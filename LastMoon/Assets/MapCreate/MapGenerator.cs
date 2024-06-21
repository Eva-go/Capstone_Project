using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using Unity.Mathematics;
using Photon.Pun;

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

    public bool autoUpdate;

    float[,] falloffMap;

    [Range(0, 1)]
    public float blendStrength;

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

public struct MapData
{
    public readonly float[,] heightMap; 

    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}
