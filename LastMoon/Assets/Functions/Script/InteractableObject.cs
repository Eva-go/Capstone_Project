using Photon.Pun;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private PhotonView photonView;
    public Item[] RPOI_PortalInventory = new Item[4]; // 내부 인벤토리 배열
    public ScriptableObject_Item[] RPOI_Items; // 다수의 아이템을 담는 ScriptableObject_Item 배열
    public int[] itemCounts; // 각 아이템 별로 개수를 설정하는 배열
    private int interactingPlayerId = -1;

    public int rpoiID = -1;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        // 처음에 Drill_Body001을 비활성화
        GameObject drillBody = gameObject.transform.GetChild(0).transform.Find("Drill_Body001").gameObject;
        if (drillBody != null)
        {
            drillBody.SetActive(false);  // Drill_Body001 비활성화
        }
        InitializeRPOIInventory();
        // 초기 아이템 설정
        SetInitialItems();
    }

    // RPOI_PortalInventory 초기화
    private void InitializeRPOIInventory()
    {
        for (int i = 0; i < RPOI_PortalInventory.Length; i++)
        {
            if (i < RPOI_Items.Length && RPOI_Items[i] != null)
            {
                RPOI_PortalInventory[i] = new Item { ItemType = RPOI_Items[i], Count = itemCounts[i] };
            }
            else
            {
                RPOI_PortalInventory[i] = new Item { ItemType = null, Count = 0 }; // 빈 아이템 슬롯 처리
            }
        }
    }

    private void SetInitialItems()
    {
        if (RPOI_PortalInventory.Length > 0)
        {
            RPOI_PortalInventory[0] = new Item { ItemType = RPOI_Items[0], Count = 1 }; // 첫 번째 아이템
            RPOI_PortalInventory[1] = new Item { ItemType = RPOI_Items[1], Count = 2 }; // 두 번째 아이템
            RPOI_PortalInventory[2] = new Item { ItemType = RPOI_Items[2], Count = 3 }; // 세 번째 아이템
            //RPOI_PortalInventory[3] = new Item { ItemType = RPOI_Items[3], Count = 4 }; // 네 번째 아이템
        }
    }

    // RPOI_PortalInventory에 아이템을 추가하는 메서드
    public void AddItemToPortalInventory(Item item, int index)
    {
        if (index >= 0 && index < RPOI_PortalInventory.Length)
        {
            RPOI_PortalInventory[index] = item; // 특정 인덱스에 아이템 추가
            Debug.Log($"{item.ItemType.ItemName} added to portal inventory at index {index}. Current count: {item.Count}");
        }
        else
        {
            Debug.LogError("Index out of bounds for RPOI_PortalInventory.");
        }
    }

    // RPOI 아이템을 플레이어 인벤토리로 옮기는 메서드
    public void TransferItemsToPlayer(Inventory playerInventory)
    {
        // RPOI 내부에 있는 아이템을 플레이어 인벤토리로 전송
        for (int i = 0; i < RPOI_Items.Length; i++)
        {
            if (RPOI_Items[i] != null && itemCounts[i] > 0)
            {
                // RPOI의 아이템과 개수를 플레이어 인벤토리로 추가
                playerInventory.AddItem(new Item { ItemType = RPOI_Items[i], Count = itemCounts[i] });

                Debug.Log(RPOI_Items[i].ItemName + " x" + itemCounts[i] + " added to player inventory.");
            }
        }
    }

    public void AddItemToPlayerInventory(ScriptableObject_Item itemType, int addCount, int playerId)
    {
        RPOIInventory playerInventory = GetPlayerInventory(playerId);
        if (playerInventory != null)
        {
            playerInventory.AddItem(itemType, addCount);
        }
    }

    // 플레이어 인벤토리 가져오기
    private RPOIInventory GetPlayerInventory(int playerId)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == playerId)
            {
                GameObject playerObject = PhotonView.Find(player.ActorNumber).gameObject;
                return playerObject.GetComponent<RPOIInventory>();
            }
        }
        Debug.LogError("Player inventory not found for playerId: " + playerId);
        return null;
    }

    // RPOI 인벤토리를 비우는 메서드
    public void ClearRPOIInventory()
    {
        for (int i = 0; i < RPOI_PortalInventory.Length; i++)
        {
            RPOI_PortalInventory[i].Count = 0;
        }
    }

    [PunRPC]
    private void UpdateCountAndInventory(string itemName, int addCount, int playerId)
    {
        interactingPlayerId = playerId;

        // 아이템을 이름으로 찾아서 인벤토리에 추가 처리
        ScriptableObject_Item itemType = FindItemByName(itemName);
        if (itemType != null)
        {
            AddItemToPlayerInventory(itemType, addCount, playerId);
        }
    }

    // 아이템 이름으로 ScriptableObject_Item을 찾는 메서드 (예시 구현)
    private ScriptableObject_Item FindItemByName(string itemName)
    {
        foreach (var item in RPOI_Items)
        {
            if (item != null && item.ItemName == itemName)
            {
                return item;
            }
        }

        Debug.LogError("Item not found: " + itemName);
        return null;
    }

    [PunRPC]
    public void ResetOwnership(bool state)
    {
        ResetInteraction();
        SetLinkObjectsActive(state);
    }

    private void ResetInteraction()
    {
        interactingPlayerId = -1;
    }

    public void SetLinkObjectsActive(bool state)
    {
        GameObject drillBody = gameObject.transform.GetChild(0).transform.Find("Drill_Body001").gameObject;
        drillBody.SetActive(state);

        // Drill_Body001이 활성화될 때 아이템을 추가하는 로직
        if (state && drillBody.activeSelf)
        {
            AddItemToPortalInventory(new Item { ItemType = RPOI_Items[0], Count = 1 }, 0);
            AddItemToPortalInventory(new Item { ItemType = RPOI_Items[1], Count = 2 }, 1);
            Debug.Log("Items added to RPOI_PortalInventory when Drill_Body001 is active.");
        }
    }

    public int GetInteractingPlayerId()
    {
        return interactingPlayerId;
    }

    public void SetInteractingPlayerId(int playerId)
    {
        interactingPlayerId = playerId;
    }
}
