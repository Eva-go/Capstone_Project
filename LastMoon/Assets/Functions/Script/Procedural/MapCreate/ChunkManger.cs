using System.Collections;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager instance;

    public Vector2 worldSize;
    public int resolution = 16;
    public float islandRadius = 900f;

    public Material terrainMat;

    public Vector3 worldCentre;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        worldCentre = new Vector3((worldSize.x / 2f) * 128f, 0f, (worldSize.y / 2f) * 128f);
        StartCoroutine(GenerateChunks());
    }

    IEnumerator GenerateChunks()
    {
        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.y; y++)
            {
                TerrainGenerator tg = new TerrainGenerator();

                GameObject current = new GameObject("Terrain " + (x * y), typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                current.transform.parent = transform;
                current.transform.localPosition = new Vector3(x * 128f, 0f, y * 128f);

                tg.Init(current);
                tg.Generate(terrainMat);

                yield return new WaitForSeconds(0.001f);
            }
        }
    }
}

class TerrainGenerator
{
    MeshFilter filter;
    MeshRenderer renderer;
    MeshCollider collider;
    Mesh mesh;

    Vector3[] verts;
    int[] triangles;
    Vector2[] uvs;

    public void Init(GameObject cur)
    {
        filter = cur.GetComponent<MeshFilter>();
        renderer = cur.GetComponent<MeshRenderer>();
        collider = cur.GetComponent<MeshCollider>();
        mesh = new Mesh();
    }

    public void Generate(Material mat)
    {
        ChunkManager chunkManager = new ChunkManager();
        Vector3 worldPos = filter.gameObject.transform.localPosition;
        int resolution = ChunkManager.instance.resolution;
        verts = new Vector3[(resolution + 1) * (resolution + 1)];
        uvs = new Vector2[verts.Length];

        Vector3 worldCentre = ChunkManager.instance.worldCentre;

        for (int i = 0, x = 0; x <= resolution; x++)
        {
            for (int z = 0; z <= resolution; z++)
            {
                Vector2 vertexWorldPos = new Vector2(worldPos.x + (x * (128f / resolution)), worldPos.z + (z * (128f / resolution)));
                float distance = Vector2.Distance(worldCentre, vertexWorldPos);
                float islandMultiplier = Mathf.Clamp01((chunkManager.islandRadius - distance) / chunkManager.islandRadius);
                islandMultiplier = Mathf.Pow(islandMultiplier, 3); // Smoothing the island edge

                // Adjusting Perlin noise
                float perlinValue = Mathf.PerlinNoise(vertexWorldPos.x * 0.01f, vertexWorldPos.y * 0.01f);
                float y = islandMultiplier * 150f * perlinValue;

                verts[i] = new Vector3(x * (128f / resolution), y, z * (128f / resolution));
                uvs[i] = new Vector2(x / (float)resolution, z / (float)resolution);
                i++;
            }
        }

        triangles = new int[resolution * resolution * 6];
        int tris = 0;
        int vert = 0;
        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + resolution + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + resolution + 1;
                triangles[tris + 5] = vert + resolution + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        collider.sharedMesh = mesh;
        filter.mesh = mesh;
        renderer.material = mat;

        Debug.Log("Mesh generated for chunk at position: " + filter.gameObject.transform.localPosition);
    }
}
