using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceRPoi : MonoBehaviourPunCallbacks
{
    public Transform parent;
    public MeshCollider placementArea;

    public TerrainCollider placementTerrain;

    public GameObject[] RPOIPrefabs;

    public Vector2 irregularityoffset;
    public float irregularity = 2f;

    public float RPOIYMin;
    public float RPOIYMax;

    [Range(0, 1)]
    public float POIThreshold1 = 0.99f;
    [Range(0, 1)]
    public float POIThreshold2 = 0.98f;
    [Range(0, 1)]
    public float POIThreshold3 = 0.97f;
    [Range(0, 1)]
    public float POIThreshold4 = 0.96f;

    public GameObject[] POIPrefabs1;
    public GameObject[] POIPrefabs2;
    public GameObject[] POIPrefabs3;
    public GameObject[] POIPrefabs4;

    public int width;
    public int height;
    public int seed;

    public Transform parentTransform;
    public float groundThreshold = 10.0f;

    public NoiseData noiseData;
    public GameObject[] rpois;  // RPOI 오브젝트를 저장할 배열


    public int currentRpoiID = 0;

    void PlaceRPOIs(float[,] noiseMap)
    {
        System.Random prng = new System.Random(seed);  // Initialize random number generator with the same seed

        for (int y = 0; y < height; y += 10)
        {
            for (int x = 0; x < width; x += 10)
            {
                Vector3 position = new Vector3(x - (int)(width / 2f), 50f, y - (int)(height / 2f));  // Adjust the y value if needed

                RaycastHit hit;
                // Cast a ray downward from a high Y position (like 100f)
                if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
                {
                    GameObject prefabToPlace = null;
                    float heightY = hit.point.y;
                    position = new Vector3(x - (int)(width / 2f), heightY, y - (int)(height / 2f));

                    // Check height limits and noise thresholds
                    if (heightY > RPOIYMin && heightY <= RPOIYMax)
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
                    }

                    if (prefabToPlace != null)
                    {
                        // Placing clump of RPOIs
                        PlaceRPOIClump(noiseMap, (int)position.x, (int)position.z, 1,
                            x, y,
                            prefabToPlace, RPOIYMin, RPOIYMax, 0.9f);
                    }
                }
            }
        }
    }

    void PlaceRPOIClump(float[,] noiseMap, int Clumpx, int Clumpy, int Distance,
        int noisex, int noisey,
        GameObject prefabToPlace, float HeightMin, float HeightMax, float NoiseLimit)
    {
        for (int x = 0; x < Distance; x++)
        {
            for (int y = 0; y < Distance; y++)
            {
                Vector3 position = new Vector3(Clumpx + x, 100f, Clumpy + y);  // Starting Y position at 100 for raycast
                RaycastHit hit;
                if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
                {
                    float heightY = hit.point.y;
                    if (heightY > HeightMin && heightY <= HeightMax && noiseMap[noisex + x, noisey + y] > NoiseLimit)
                    {
                        position = new Vector3(Clumpx + x, heightY, Clumpy + y);
                        // Instantiate the RPOI object
                        GameObject newRpoi = Instantiate(prefabToPlace, position, Quaternion.identity, this.transform);

                        // Assign a unique node ID and add to the array
                        newRpoi.GetComponent<InteractableObject>().rpoiID = currentRpoiID;
                        rpois[currentRpoiID] = newRpoi;
                        currentRpoiID++;  // Increment the ID for the next RPOI
                    }
                }
            }
        }
    }
    void Start()
    {
        rpois = new GameObject[2000];
        if (RPOIPrefabs.Length > 0)
        {
            float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(width, height, seed, noiseData.noiseScale1, irregularity, irregularityoffset);
            PlaceRPOIs(irregularNoiseMap);
        }
        else
        {
            Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
        }

    }
    public void Rpoi_active(int id)
    {
        photonView.RPC("RPC_Rpoi_active", RpcTarget.AllBuffered, id);
    }

    [PunRPC]
    void RPC_Rpoi_active(int id)
    {
        if (rpois[id] != null)
        {
            rpois[id].gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
