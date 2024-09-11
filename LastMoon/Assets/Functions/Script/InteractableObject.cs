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

        // ������ ���� �迭�� ������ �迭�� ũ�⿡ �°� �ʱ�ȭ
        if (itemCounts == null || itemCounts.Length != RPOI_Items.Length)
        {
            itemCounts = new int[RPOI_Items.Length];
        }

        // RPOI_Items �迭�� ����Ͽ� RPOI_PortalInventory �ʱ�ȭ
        InitializeRPOIInventory();
    }

    // RPOI_PortalInventory �ʱ�ȭ
    private void InitializeRPOIInventory()
    {
        for (int i = 0; i < RPOI_PortalInventory.Length; i++)
        {
            if (i < RPOI_Items.Length && RPOI_Items[i] != null)
            {
                RPOI_PortalInventory[i] = new Item { ItemType = RPOI_Items[i], Count = itemCounts[i] }; // ���� �ݿ�
            }
            else
            {
                RPOI_PortalInventory[i] = new Item { ItemType = null, Count = 0 }; // �� ������ ���� ó��
            }
        }
    }

    // �� �������� �÷��̾��� �κ��丮�� �߰�
    public void IncreaseAllItems(int playerId)
    {
        if (interactingPlayerId == -1 || interactingPlayerId == playerId)
        {
            interactingPlayerId = playerId;

            // �� �������� �÷��̾� �κ��丮�� �߰�
            for (int i = 0; i < RPOI_Items.Length; i++)
            {
                if (RPOI_Items[i] != null)
                {
                    // �÷��̾��� �κ��丮�� ������ �߰�
                    AddItemToPlayerInventory(RPOI_Items[i], itemCounts[i]);

                    // ��Ʈ��ũ �󿡼� ����ȭ�� �÷��̾�鿡�� ī��Ʈ�� ������Ű�� �κ��丮 ���¸� ������Ʈ
                    photonView.RPC("UpdateCountAndInventory", RpcTarget.AllBuffered, RPOI_Items[i].name, itemCounts[i], playerId);
                }
            }
        }
    }

    // �÷��̾��� �κ��丮�� �������� �߰��ϴ� �޼���
    public void AddItemToPlayerInventory(ScriptableObject_Item itemType, int addCount)
    {
        PlayerRPOIInventory playerInventory = GetPlayerInventory(interactingPlayerId);
        if (playerInventory != null)
        {
            playerInventory.AddItem(itemType, addCount);
        }
    }

    // �÷��̾� �κ��丮 �������� (�÷��̾� ID�� ���� ������)
    private PlayerRPOIInventory GetPlayerInventory(int playerId)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == playerId)
            {
                // �ش� playerId�� ����� �÷��̾� ������Ʈ�� ã�� PlayerRPOIInventory ������Ʈ�� ��ȯ
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

        // �������� �̸����� ã�Ƽ� �κ��丮�� �߰� ó��
        ScriptableObject_Item itemType = FindItemByName(itemName);
        if (itemType != null)
        {
            AddItemToPlayerInventory(itemType, addCount);
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
