using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerMaterialManager : MonoBehaviourPun
{
    public Material[] availableMaterials; // 사용 가능한 Material 배열
    public GameObject otherPlayerObject;  // 하위 OtherPlayer 오브젝트

    void Start()
    {
        if (photonView.IsMine)
        {
            // 각 플레이어의 고유 ID (예: ActorNumber)를 기반으로 Material 인덱스를 결정
            int materialIndex = GetMaterialIndexForPlayer(PhotonNetwork.LocalPlayer);
            // RPC 호출로 모든 클라이언트에 동기화
            photonView.RPC("RPC_SetOtherPlayerMaterial", RpcTarget.AllBuffered, materialIndex);
        }
    }

    // 플레이어의 고유 ID에 따라 Material 인덱스를 반환하는 메서드
    int GetMaterialIndexForPlayer(Player player)
    {
        // ActorNumber 또는 UserId에 따라 고유 Material 인덱스를 생성
        return player.ActorNumber % availableMaterials.Length;
    }

    // RPC를 통해 모든 클라이언트에서 Material을 동기화
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
