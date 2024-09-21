using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceNode : MonoBehaviour
{

    public Transform parent;

    public GameObject[] nodePrefabs;

    public MeshCollider placementArea;

    public TerrainCollider placementTerrain;

    public bool UseMesh;

    public Vector2 irregularityoffset;
    public float irregularity = 2f;

    public float NodedirtYMin;
    public float NodedirtYMax;
    public float NodesandYMin;
    public float NodesandYMax;


    [Range(0, 1)]
    public float NodedirtThreshold1 = 0.9975f;
    [Range(0, 1)]
    public float NodedirtThreshold2 = 0.995f;
    [Range(0, 1)]
    public float NodedirtThreshold3 = 0.5f;

    public GameObject[] dirtlowNodePrefabs;
    public GameObject[] dirtmediumNodePrefabs;
    public GameObject[] dirthighNodePrefabs;

    public GameObject[] sandlowNodePrefabs;
    public GameObject[] sandmediumNodePrefabs;
    public GameObject[] sandhighNodePrefabs;


    public int width;
    public int height;
    public int seed;

    //public GameObject selectedPrefab;

    public Transform parentTransform;
    public float groundThreshold = 10.0f;

    public NoiseData noiseData;


    public float activationRadius = 50f; // ��� Ȱ��ȭ �ݰ�
    private Transform playerTransform;
    private List<GameObject> nearbyNodes = new List<GameObject>();
    void PlaceDirtNodes(float[,] noiseMap)
    {
        System.Random prng = new System.Random(seed); // Initialize random number generator with the same seed

        bool IsSand = false;

        for (int y = 0; y < height; y += 5)
        {
            for (int x = 0; x < width; x += 5)
            {
                Vector3 position = new Vector3(x - (int)(width / 2f), 0, y - (int)(height / 2f)); // Adjust the y value if needed

                RaycastHit hit;
                if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
                {
                    GameObject prefabToPlace = null;
                    float heightY = hit.point.y;
                    if (heightY > NodedirtYMin && heightY <= NodedirtYMax)
                    {
                        IsSand = false;
                        position = new Vector3(x - (int)(width / 2f), heightY, y - (int)(height / 2f));
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
                    }
                    else if (heightY > NodesandYMin && heightY <= NodesandYMax)
                    {
                        IsSand = true;
                        position = new Vector3(x - (int)(width / 2f), heightY, y - (int)(height / 2f));
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
                    }
                    
                    if (prefabToPlace != null)
                    {
                        if (IsSand)
                            PlaceNodeClump(noiseMap, (int)position.x, (int)position.z, 5,
                                x, y,
                                prefabToPlace, NodesandYMin, NodesandYMax, 0.9f);
                        else
                            PlaceNodeClump(noiseMap, (int)position.x, (int)position.z, 5,
                                x, y,
                                prefabToPlace, NodedirtYMin, NodedirtYMax, 0.9f);

                      
                    }
                }
            }
        }
    }


    void PlaceNodeClump(float[,] noiseMap, int Clumpx, int Clumpy, int Distance,
        int noisex, int noisey,
        GameObject prefabToPlace, float HeightMin, float HeightMax, float NoiseLimit)
    {
        for (int x = 0; x < Distance; x++)
        {
            for (int y = 0; y < Distance; y++)
            {
                Vector3 position = new Vector3(Clumpx + x, 0, Clumpy + y);
                RaycastHit hit;
                if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
                {
                    float heightY = hit.point.y;
                    if (heightY > HeightMin && heightY <= HeightMax
                        && noiseMap[noisex + x, noisey + y] > NoiseLimit
                        )
                    {
                        position = new Vector3(Clumpx + x, heightY, Clumpy + y);

                        GameObject newNode = Instantiate(prefabToPlace,this.transform);
                        newNode.transform.position = position;
                    }
                }
            }
        }
    }



    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (nodePrefabs.Length > 0)
            {
                float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(width, height, seed, noiseData.noiseScale1, irregularity, irregularityoffset);
                PlaceDirtNodes(irregularNoiseMap);
            }
            else
            {
                Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
            }
        }
        if (PhotonNetwork.LocalPlayer.TagObject != null)
        {
            playerTransform = PhotonNetwork.LocalPlayer.TagObject as Transform;
        }
    }


    public void Update()
    {
        if (playerTransform == null)
        {
            // Player�� �����Ǿ����� Ȯ��
            if (PhotonNetwork.LocalPlayer.TagObject != null)
            {
                playerTransform = PhotonNetwork.LocalPlayer.TagObject as Transform;
            }
        }
        else
        {
            CheckNearbyNodes();
        }
    }
    void CheckNearbyNodes()
    {
        // Ȱ��ȭ �ݰ� ���� ��� �ݶ��̴��� ����
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, activationRadius);
        nearbyNodes.Clear();

        foreach (Collider hitCollider in hitColliders)
        {
            GameObject node = hitCollider.gameObject;

            // ������� Ȯ���ϰ� �̹� Ȱ��ȭ���� �ʾҴ��� Ȯ��
            if (node.CompareTag("Node"))
            {
                if (!nearbyNodes.Contains(node)) // ����Ʈ�� ���ԵǾ� ���� ������ �߰�
                {
                    nearbyNodes.Add(node);
                    node.SetActive(true); // ��带 Ȱ��ȭ
                }
            }
        }

        // ��� ��� �� ��Ȱ��ȭ�� ��带 üũ�Ͽ� ��Ȱ��ȭ
        foreach (Transform node in transform)
        {
            if (!nearbyNodes.Contains(node.gameObject))
            {
                node.gameObject.SetActive(false); // ��带 ��Ȱ��ȭ
            }
        }
    }

}
