using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceNode : MonoBehaviourPunCallbacks
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
    public GameObject[] nodes;
    public int currentNodeID = 0;
    public float[,] irregularNoiseMap;
    System.Random prng;
    private bool isSpawn;

    public void PlaceDirtNodes(float[,] noiseMap , System.Random prng)
    {

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
                    else if (heightY > NodesandYMin && heightY <= NodesandYMax)
                    {
                        IsSand = true;
                        position = new Vector3(x - (int)(width / 2f), heightY, y - (int)(height / 2f));
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
                        // 새로운 노드 생성
                        GameObject newNode = Instantiate(prefabToPlace, this.transform);
                        newNode.transform.position = position;

                        // 고유한 nodeID 할당
                        newNode.GetComponent<NodeController>().nodeID = currentNodeID;
                        nodes[currentNodeID] = newNode;
                        currentNodeID++; // nodeID 카운터 증가

                        newNode.GetComponent<Animator>().enabled = false;
                        //newNode.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        //newNode.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
        }
    }


    public void nodespawns()
    {
        
        if (!isSpawn)
        {
            Debug.LogError("노드 생성");
            PlaceDirtNodes(irregularNoiseMap, prng);
            isSpawn = true;
        }
       
    }

    void Start()
    {
        nodes = new GameObject[10000];
        if (nodePrefabs.Length > 0)
        {
            prng = new System.Random(seed); // Initialize random number generator with the same seed
            irregularNoiseMap = Noise.GenerateIrregularNoiseMap(width, height, seed, noiseData.noiseScale1, irregularity, irregularityoffset);
            PlaceDirtNodes(irregularNoiseMap, prng);
        }
        else
        {
            Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
        }
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    if (nodePrefabs.Length > 0)
        //    {
        //        float[,] irregularNoiseMap = Noise.GenerateIrregularNoiseMap(width, height, seed, noiseData.noiseScale1, irregularity, irregularityoffset);
        //        PlaceDirtNodes(irregularNoiseMap);
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Placement area collider or building prefabs not properly assigned!");
        //    }
        //}
    }

    public void Ani_Hit(int id)
    {
        photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Hit",id);
    }

    public void Ani_Harvest(int id)
    {
        photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Harvest",id);
    }

    public void Ani_Destory(int id)
    {
        photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Destroy", id);
    }

    public void node_Hp(int id)
    {
        photonView.RPC("SyncHealth", RpcTarget.OthersBuffered, nodes[id].GetComponent<NodeController>().currentHealth,id);
    }

    public void node_Destory(int id)
    {
        photonView.RPC("RPC_DestroyNode", RpcTarget.AllBuffered, id);
    }

    public void All_node_Destory()
    {
        if(isSpawn)
        {
            Debug.LogError("노드 삭제중");
            photonView.RPC("RPC_All_DestroyNode", RpcTarget.AllBuffered);
            isSpawn = false;
        }
       
    }


    [PunRPC]
    void RPC_All_DestroyNode()
    {
        for(int i=0; i<nodes.Length;i++)
        {
            Destroy(nodes[i]);
        }
    }

    [PunRPC]
    void SyncHealth(float health,int id)
    {
        if (nodes[id] != null)
        {
            nodes[id].GetComponent<NodeController>().currentHealth = health;
        }
    }

    [PunRPC]
    void RPC_SetTrigger(string triggerName,int id)
    {
        if (nodes[id] != null)
        {
            nodes[id].GetComponent<NodeController>().animator.SetTrigger(triggerName);
        }
    }

    [PunRPC]
    void RPC_DestroyNode(int id)
    {
        if (id >= 0 && id < nodes.Length && nodes[id] != null)
        {
            Destroy(nodes[id]);
            nodes[id] = null; // 배열에서 참조를 제거
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info , int id)
    {
        if (stream.IsWriting)
        {
            // 포톤 네트워크를 통해 HP 값을 전송
            stream.SendNext(nodes[id].GetComponent<NodeController>().currentHealth);
        }
        else
        {
            // 다른 플레이어로부터 HP 값을 수신
            nodes[id].GetComponent<NodeController>().currentHealth = (float)stream.ReceiveNext();
        }
    }
}
