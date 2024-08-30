using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceNode : MonoBehaviour
{

    public GameObject[] nodePrefabs;

    public MeshCollider placementArea;

    public Vector2 irregularityoffset;
    public float irregularity = 2f;

    public float NodedirtYMin;
    public float NodedirtYMax;
    public float NodesandYMin;
    public float NodesandYMax;


    [Range(0, 1)]
    public float NodedirtThreshold1 = 0.9f;
    [Range(0, 1)]
    public float NodedirtThreshold2 = 0.7f;
    [Range(0, 1)]
    public float NodedirtThreshold3 = 0.5f;
    [Range(0, 1)]
    public float NodesandThreshold1 = 0.9f;
    [Range(0, 1)]
    public float NodesandThreshold2 = 0.7f;
    [Range(0, 1)]
    public float NodesandThreshold3 = 0.5f;

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


    void PlaceDirtNodes(float[,] noiseMap)
    {
        System.Random prng = new System.Random(seed); // Initialize random number generator with the same seed

        for (int y = 0; y < height; y += 1)
        {
            for (int x = 0; x < width; x += 1)
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
        System.Random prng = new System.Random(seed); // Initialize random number generator with the same seed

        for (int y = 0; y < height; y += 1)
        {
            for (int x = 0; x < width; x += 1)
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
                            if (noiseMap[x, y] > NodesandThreshold1)
                            {
                                prefabToPlace = sandlowNodePrefabs[prng.Next(sandlowNodePrefabs.Length)];
                            }
                            else if (noiseMap[x, y] > NodesandThreshold2)
                            {
                                prefabToPlace = sandmediumNodePrefabs[prng.Next(sandmediumNodePrefabs.Length)];
                            }
                            else if (noiseMap[x, y] > NodesandThreshold3)
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


    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (placementArea != null && nodePrefabs.Length > 0)
            {
                float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(width, height, seed, noiseData.noiseScale1, irregularity, irregularityoffset);
                PlaceDirtNodes(irregularNoiseMap);
                PlaceSandNodes(irregularNoiseMap);
            }
            else
            {
                Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
            }
        }
    }
}
