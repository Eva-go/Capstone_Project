using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public int count = 0; // ī��Ʈ ����
    private PhotonView photonView; // PhotonView ������Ʈ

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
        Debug.Log($"Count increased to: {count}"); // ī��Ʈ ���� �� ����� �޽��� ���
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
            // ī��Ʈ�� ������Ű�� ���� RPC ȣ��
            photonView.RPC("IncreaseCount", RpcTarget.AllBuffered);
        }
    }
}
