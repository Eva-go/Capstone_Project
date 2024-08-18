using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public int count = 0; // 카운트 변수
    private PhotonView photonView; // PhotonView 컴포넌트

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void SetAcitve(bool index)
    {
        gameObject.transform.GetChild(0).transform.Find("Drill_Body001").gameObject.SetActive(index);
    }

    [PunRPC]
    public void IncreaseCount()
    {
        count++;
        Debug.Log($"Count increased to: {count}"); // 카운트 증가 시 디버그 메시지 출력
    }

    [PunRPC]
    public void SetActiveState(int instanceID, bool state)
    {
        GameObject obj = PhotonView.Find(instanceID)?.gameObject;
        if (obj != null)
        {
            obj.SetActive(state);
        }
    }


    public void Interact()
    {
        if (photonView.IsMine)
        {
            // 카운트를 증가시키기 위해 RPC 호출
            photonView.RPC("IncreaseCount", RpcTarget.AllBuffered);
        }
    }
}
