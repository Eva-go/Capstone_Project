using Photon.Pun;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private PhotonView photonView;
    public Item[] RPOI_PortalInventory = new Item[4]; // ���� �κ��丮 �迭
    public ScriptableObject_Item[] RPOI_Items; // �ټ��� �������� ��� ScriptableObject_Item �迭
    public int[] itemCounts; // �� ������ ���� ������ �����ϴ� �迭
    private int interactingPlayerId = -1;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        // ó���� Drill_Body001�� ��Ȱ��ȭ
        GameObject drillBody = gameObject.transform.GetChild(0).transform.Find("Drill_Body001").gameObject;
        if (drillBody != null)
        {
            drillBody.SetActive(false);  // Drill_Body001 ��Ȱ��ȭ
        }
        InitializeRPOIInventory();
    }

    // RPOI_PortalInventory �ʱ�ȭ
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
                RPOI_PortalInventory[i] = new Item { ItemType = null, Count = 0 }; // �� ������ ���� ó��
            }
        }
    }

    // RPOI �������� �÷��̾� �κ��丮�� �ű�� �޼���
    public void TransferItemsToPlayer(Inventory playerInventory)
    {
        // RPOI ���ο� �ִ� �������� �÷��̾� �κ��丮�� ����
        for (int i = 0; i < RPOI_Items.Length; i++)
        {
            if (RPOI_Items[i] != null && itemCounts[i] > 0)
            {
                // RPOI�� �����۰� ������ �÷��̾� �κ��丮�� �߰�
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

    // �÷��̾� �κ��丮 ��������
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

    // RPOI �κ��丮�� ���� �޼���
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

        // �������� �̸����� ã�Ƽ� �κ��丮�� �߰� ó��
        ScriptableObject_Item itemType = FindItemByName(itemName);
        if (itemType != null)
        {
            AddItemToPlayerInventory(itemType, addCount, playerId);
        }
    }

    // ������ �̸����� ScriptableObject_Item�� ã�� �޼��� (���� ����)
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
