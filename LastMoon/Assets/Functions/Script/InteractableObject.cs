using Photon.Pun;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private PhotonView photonView;
    public Item[] RPOI_PortalInventory = new Item[4]; // 내부 인벤토리 배열
    public ScriptableObject_Item[] RPOI_Items; // 다수의 아이템을 담는 ScriptableObject_Item 배열
    private int interactingPlayerId = -1;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        // RPOI_Items 배열을 사용하여 RPOI_PortalInventory 초기화
        InitializeRPOIInventory();
    }

    // RPOI_PortalInventory 초기화
    private void InitializeRPOIInventory()
    {
        for (int i = 0; i < RPOI_PortalInventory.Length; i++)
        {
            if (i < RPOI_Items.Length && RPOI_Items[i] != null)
            {
                RPOI_PortalInventory[i] = new Item { ItemType = RPOI_Items[i], Count = RPOI_Items[i].MaxCount };
            }
            else
            {
                RPOI_PortalInventory[i] = new Item { ItemType = null, Count = 0 }; // 빈 아이템 슬롯 처리
            }
        }
    }

    // 상호작용 시 플레이어의 인벤토리에 아이템을 추가하는 메서드
    public void IncreaseItem(ScriptableObject_Item itemType, int addCount, int playerId)
    {
        if (interactingPlayerId == -1 || interactingPlayerId == playerId)
        {
            interactingPlayerId = playerId;

            // 플레이어의 인벤토리에 아이템 추가
            AddItemToPlayerInventory(itemType, addCount);

            // 네트워크 상에서 동기화된 플레이어들에게 카운트를 증가시키고 인벤토리 상태를 업데이트
            photonView.RPC("UpdateCountAndInventory", RpcTarget.AllBuffered, itemType.name, addCount, playerId);
        }
    }

    // 플레이어의 인벤토리에 아이템을 추가하는 메서드
    public void AddItemToPlayerInventory(ScriptableObject_Item itemType, int addCount)
    {
        PlayerRPOIInventory playerInventory = GetPlayerInventory(interactingPlayerId);
        if (playerInventory != null)
        {
            playerInventory.AddItem(itemType, addCount);
        }
    }

    // 플레이어 인벤토리 가져오기 (플레이어 ID에 따라 가져옴)
    private PlayerRPOIInventory GetPlayerInventory(int playerId)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == playerId)
            {
                // 해당 playerId와 연결된 플레이어 오브젝트를 찾아 PlayerRPOIInventory 컴포넌트를 반환
                GameObject playerObject = PhotonView.Find(player.ActorNumber).gameObject;
                return playerObject.GetComponent<PlayerRPOIInventory>();
            }
        }

        Debug.LogError("Player inventory not found for playerId: " + playerId);
        return null;
    }

    [PunRPC]
    private void UpdateCountAndInventory(string itemName, int addCount, int playerId)
    {
        interactingPlayerId = playerId;

        // 아이템을 이름으로 찾아서 인벤토리에 추가 처리
        ScriptableObject_Item itemType = FindItemByName(itemName);
        if (itemType != null)
        {
            AddItemToPlayerInventory(itemType, addCount);
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
        gameObject.transform.GetChild(0).transform.Find("Drill_Body001").gameObject.SetActive(state);
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
