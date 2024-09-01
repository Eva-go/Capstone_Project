using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePoiBuild : MonoBehaviour
{
    public MeshCollider placementArea;

    public GameObject[] buildingPrefabs;
    public GameObject[] POIPrefabs;

    public Vector2 irregularityoffset;
    public float irregularity = 2f;

    public float BuildingYMin;
    public float BuildingYMax;
    public float POIYMin;
    public float POIYMax;
    [Range(0, 1)]
    public float BuildingThreshold1 = 0.8f;
    [Range(0, 1)]
    public float BuildingThreshold2 = 0.6f;
    [Range(0, 1)]
    public float BuildingThreshold3 = 0.4f;
    [Range(0, 1)]
    public float POIThreshold1 = 0.9f;
    [Range(0, 1)]
    public float POIThreshold2 = 0.7f;
    [Range(0, 1)]
    public float POIThreshold3 = 0.5f;
    [Range(0, 1)]
    public float POIThreshold4 = 0.5f;

    public GameObject[] lowNoisePrefabs; // 낮은 높이 프리팹 배열
    public GameObject[] mediumNoisePrefabs; // 중간 높이 프리팹 배열
    public GameObject[] highNoisePrefabs; // 높은 높이 프리팹 배열

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

    void PlaceBuildings(float[,] noiseMap)
    {
        System.Random prng = new System.Random(seed); // Initialize random number generator with the same seed

        for (int y = 12; y < height; y += 24)
        {
            for (int x = 12; x < width; x += 24)
            {
                Vector3 position = new Vector3(x - (int)(width / 2f), 0, y - (int)(height / 2f)); // Adjust the y value if needed\
                RaycastHit hit;
                if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
                {
                    GameObject prefabToPlace = null;
                    float heightY = hit.point.y;
                    if (heightY > BuildingYMin)
                    {
                        if (heightY <= BuildingYMax)
                        {
                            position = new Vector3(x - (int)(width / 2f), heightY, y - (int)(height / 2f));
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

    void PlacePOIs(float[,] noiseMap)
    {
        System.Random prng = new System.Random(seed); // Initialize random number generator with the same seed

        for (int y = 0; y < height; y += 10)
        {
            for (int x = 0; x < width; x += 10)
            {
                Vector3 position = new Vector3(x - (int)(width / 2f), 0, y - (int)(height / 2f)); // Adjust the y value if needed

                RaycastHit hit;
                if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
                {
                    GameObject prefabToPlace = null;
                    float heightY = hit.point.y;
                    position = new Vector3(x - (int)(width / 2f), heightY, y - (int)(height / 2f));
                    if (heightY > POIYMin)
                    {
                        if (heightY <= POIYMax)
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

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (placementArea != null && POIPrefabs.Length > 0)
            {
                float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(width, height, seed, noiseData.noiseScale1, irregularity, irregularityoffset);
                PlacePOIs(irregularNoiseMap);
            }
            else
            {
                Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
            }

            if (placementArea != null && buildingPrefabs.Length > 0)
            {
                float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(width, height, seed, noiseData.noiseScale1, irregularity, irregularityoffset);
                PlaceBuildings(irregularNoiseMap);
            }
            else
            {
                Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
            }
        }
    }

}
