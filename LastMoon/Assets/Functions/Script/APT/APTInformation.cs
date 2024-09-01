using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class APTInformation : MonoBehaviourPun
{
    private int PlayerID;
    public MeshRenderer Icon;
    public Material GreenMaterial;
    public Material RedMaterial;
    public bool APT_use;
    public enum Color { Green, Red, Yellow }
    public Color color;
    public PhotonView pv;
    public PlayerController UsePlayer;

    public int BuildingType; // 0 - 아파트, 1 - 마천루, 2 - 벙커
    public ScriptableObject_Item Key1;
    public ScriptableObject_Item Key2;
    public ScriptableObject_Item Key3;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        if(pv.IsMine)
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
       if(GameValue.Round>5)
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

    public void Use_Item(PlayerController player)
    {
        switch (BuildingType)
        {
            case 0:
                player.PlayerInventory.RemoveItem(new Item { ItemType = Key1, Count = 50 });
                break;
            case 1:
                player.PlayerInventory.RemoveItem(new Item { ItemType = Key1, Count = 100 });
                player.PlayerInventory.RemoveItem(new Item { ItemType = Key2, Count = 50 });
                break;
            case 2:
                player.PlayerInventory.RemoveItem(new Item { ItemType = Key1, Count = 150 });
                player.PlayerInventory.RemoveItem(new Item { ItemType = Key2, Count = 100 });
                player.PlayerInventory.RemoveItem(new Item { ItemType = Key2, Count = 50 });
                break;
        }
    }


    public void Use_player(PlayerController player)
    {
        UsePlayer = player;
        bool HasKey = false;

        switch (BuildingType)
        {
            case 0:
                HasKey = player.PlayerInventory.CheckItem(new Item { ItemType = Key1, Count = 50 });
                break;
            case 1:
                HasKey = (
                    player.PlayerInventory.CheckItem(new Item { ItemType = Key1, Count = 100 })
                    && player.PlayerInventory.CheckItem(new Item { ItemType = Key2, Count = 50 })
                );
                break;
            case 2:
                HasKey = (
                    player.PlayerInventory.CheckItem(new Item { ItemType = Key1, Count = 150 })
                    && player.PlayerInventory.CheckItem(new Item { ItemType = Key2, Count = 100 })
                    && player.PlayerInventory.CheckItem(new Item { ItemType = Key3, Count = 50 })
                );
                break;
        }

        if (HasKey && player.APT_in && APT_use)
        {
            for (int j = 0; j < gameObject.transform.GetChild(0).childCount; j++)
            {
                player.HouseKey = BuildingType + 2;
                Icon = gameObject.transform.GetChild(0).GetChild(j).GetComponent<MeshRenderer>();
                Icon.material = GreenMaterial;
                color = Color.Green;
                photonView.RPC("APT_cloes", RpcTarget.OthersBuffered, Color.Red.ToString(), APT_use);
            }
        }
        else if (HasKey && player.APT_in && !APT_use)
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
        photonView.RPC("APT_cloes", RpcTarget.OthersBuffered, Color.Red.ToString(), APT_use);
    }

    [PunRPC]
    public void APT_cloes(string colorString, bool Use)
    {
        if (Use)
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