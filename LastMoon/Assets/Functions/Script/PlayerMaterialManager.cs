using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerMaterialManager : MonoBehaviourPun
{
    public Material[] availableMaterials; // ��� ������ Material �迭
    public GameObject otherPlayerObject;  // ���� OtherPlayer ������Ʈ

    void Start()
    {
        if (photonView.IsMine)
        {
            // �� �÷��̾��� ���� ID (��: ActorNumber)�� ������� Material �ε����� ����
            int materialIndex = GetMaterialIndexForPlayer(PhotonNetwork.LocalPlayer);
            // RPC ȣ��� ��� Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("RPC_SetOtherPlayerMaterial", RpcTarget.AllBuffered, materialIndex);
        }
    }

    // �÷��̾��� ���� ID�� ���� Material �ε����� ��ȯ�ϴ� �޼���
    int GetMaterialIndexForPlayer(Player player)
    {
        // ActorNumber �Ǵ� UserId�� ���� ���� Material �ε����� ����
        return player.ActorNumber % availableMaterials.Length;
    }

    // RPC�� ���� ��� Ŭ���̾�Ʈ���� Material�� ����ȭ
    [PunRPC]
    void RPC_SetOtherPlayerMaterial(int index)
    {
        Renderer renderer = otherPlayerObject.GetComponent<Renderer>();
        if (renderer != null && index < availableMaterials.Length)
        {
            renderer.material = availableMaterials[index];
        }
    }
}
