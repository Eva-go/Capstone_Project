using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.SceneManagement;

public class APTInformation : MonoBehaviourPun
{
    private int PlayerID;
    public MeshRenderer Icon;
    public Material GreenMaterial;
    public Material RedMaterial;
    public bool APT_use;
    public enum Color { Yellow, Green, Red }
    public Color color;
    public PhotonView pv;
    public PlayerController UsePlayer;

    public int BuildingType; // 0 - 아파트, 1 - 마천루, 2 - 벙커
    public ScriptableObject_Item Key1;
    public ScriptableObject_Item Key2;
    public ScriptableObject_Item Key3;

    private bool isLoading = false;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            if (gameObject.name == "Apartment(Clone)")
            {
                Destroy(gameObject);
            }
            color = Color.Yellow;
            APT_use = false;
            PlayerID = NetworkManager.PlayerID;
        }
    }

    void Update()
    {
        if (GameValue.TideCycle > 11 && BuildingType == 0)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
            for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
            {
                Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                Icon.material = RedMaterial;
                color = Color.Red;
            }
        }
        else if (GameValue.TideCycle > 17 && BuildingType == 1)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
            for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
            {
                Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                Icon.material = RedMaterial;
                color = Color.Red;
            }
        }

    }
    public void Use_player(PlayerController player)
    {
        UsePlayer = player;
        bool HasKey = false;

        switch (BuildingType)
        {
            case 0:
                HasKey = (player.HouseKey == 2);
                break;
            case 1:
                HasKey = (player.HouseKey == 3);
                break;
            case 2:
                HasKey = (player.HouseKey == 4);
                break;
        }

        if (HasKey && APT_use&&BuildingType==0)
        {
            for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
            {
                player.HouseKey = BuildingType + 2;
                Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                Icon.material = GreenMaterial;
                color = Color.Green;
                photonView.RPC("APT_cloes", RpcTarget.OthersBuffered, Color.Red.ToString(), APT_use,BuildingType);
            }
        }
        else if (HasKey  && !APT_use&& BuildingType == 0)
        {
            for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
            {
                Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                Icon.material = RedMaterial;
                color = Color.Red;
            }
        }
        else if (HasKey  && APT_use&& BuildingType == 1)
        {
            for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
            {
                player.HouseKey = BuildingType + 2;
                Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                Icon.material = GreenMaterial;
                color = Color.Green;
                photonView.RPC("APT_cloes", RpcTarget.OthersBuffered, Color.Red.ToString(), APT_use, BuildingType);
            }
        }
        else if (HasKey  && !APT_use&&BuildingType == 1)
        {
            for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
            {
                Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                Icon.material = RedMaterial;
                color = Color.Red;
            }
        } 
    }

    public void Request_APT(PlayerController player)
    {
        photonView.RPC("APT_cloes", RpcTarget.OthersBuffered, Color.Red.ToString(), APT_use, BuildingType);
    }

    [PunRPC]
    public void APT_cloes(string colorString, bool Use ,int buildingType)
    {
        if (Use)
        {
            if(buildingType==0)
            {
                Color newColor;
                if (System.Enum.TryParse(colorString, out newColor))
                {
                    Material material = newColor == Color.Red ? RedMaterial : GreenMaterial;
                    Debug.Log("Applying material: " + material.name + " with color: " + newColor);
                    for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
                    {
                        Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                        Icon.material = material;
                        color = newColor;
                        Debug.Log("Updated material to: " + material.name + " with color: " + color);
                    }
                }
                else
                {
                    Debug.LogWarning($"Failed to parse colorString: {colorString}");
                }
            }
           else if(buildingType==1)
            {
                Color newColor;
                if (System.Enum.TryParse(colorString, out newColor))
                {
                    Material material = newColor == Color.Red ? RedMaterial : GreenMaterial;
                    Debug.Log("Applying material: " + material.name + " with color: " + newColor);
                    for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
                    {
                        Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                        Icon.material = material;
                        color = newColor;
                        Debug.Log("Updated material to: " + material.name + " with color: " + color);
                    }
                }
                else
                {
                    Debug.LogWarning($"Failed to parse colorString: {colorString}");
                }
            }
        }
    }
}