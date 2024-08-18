using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    private PhotonView photonView;
    [SerializeField]
    private List<PhotonView> linkedObjects; // 사용되지 않는 변수 삭제
    public int count = 0;
    private int interactingPlayerId = -1; // 현재 상호작용 중인 플레이어의 ID

    public void IncreaseCount(int playerId)
    {
        // 소유권이 있는 플레이어만 카운트를 증가시킬 수 있음
        if (interactingPlayerId == playerId)
        {
            // 카운트는 코루틴에서 증가하므로 여기는 필요 없을 수 있습니다.
        }
    }


    [PunRPC]
    public void ResetOwnership(bool state)
    {
        ResetInteraction(); // 카운트 초기화
        SetLinkObjectsActive(state); // Link 오브젝트 비활성화
    }

    private void ResetInteraction()
    {
        count = 0; // 카운트 초기화
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