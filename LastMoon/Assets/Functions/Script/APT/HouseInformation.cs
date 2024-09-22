using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class HouseInformation : MonoBehaviourPun
{
    private int PlayerID;
    private MeshRenderer Icon;
    private bool HouseKey;
    public Material GreenMaterial;
    public Material RedMaterial;
    public int index = -1;

    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.name == "House(Clone)")
        {
            Destroy(gameObject);
        }
        PlayerID = NetworkManager.PlayerID;
        HouseKey = false;
        Transform pointTransform = gameObject.transform.parent;
        Transform spawnPointTransform = pointTransform.parent;
        
        for (int i = 0; i < spawnPointTransform.childCount; i++)
        {
            if (spawnPointTransform.GetChild(i) == pointTransform)
            {
                index = i;
                if (index == PlayerID)
                {
                    for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
                    {
                        Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                        Icon.material = GreenMaterial;
                    }
                }
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameValue.TideCycle > 5)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
        }
        if (index==PlayerID&&!HouseKey)
        {
            foreach (PhotonView photonView in FindObjectsOfType<PhotonView>())
            {
                if (photonView.IsMine)
                {
                    PlayerController playerController = photonView.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.HouseKey = 1;
                        HouseKey = true;
                    }
                }
            }
        }
        else if(HouseKey)
        {
            foreach (PhotonView photonView in FindObjectsOfType<PhotonView>())
            {
                if (photonView.IsMine)
                {
                    PlayerController playerController = photonView.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        if(playerController.HouseKey!=1)
                        {
                            for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
                            {
                                Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                                Icon.material = RedMaterial;
                            }
                        }
                    }
                }
            }
        }
    }
}
