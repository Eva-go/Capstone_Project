using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    private PhotonView photonView;
    [SerializeField]
    private List<PhotonView> linkedObjects; // ������ �ʴ� ���� ����
    public int count = 0;
    private int interactingPlayerId = -1; // ���� ��ȣ�ۿ� ���� �÷��̾��� ID

    public void IncreaseCount(int playerId)
    {
        // �������� �ִ� �÷��̾ ī��Ʈ�� ������ų �� ����
        if (interactingPlayerId == playerId)
        {
            // ī��Ʈ�� �ڷ�ƾ���� �����ϹǷ� ����� �ʿ� ���� �� �ֽ��ϴ�.
        }
    }


    [PunRPC]
    public void ResetOwnership(bool state)
    {
        ResetInteraction(); // ī��Ʈ �ʱ�ȭ
        SetLinkObjectsActive(state); // Link ������Ʈ ��Ȱ��ȭ
    }

    private void ResetInteraction()
    {
        count = 0; // ī��Ʈ �ʱ�ȭ
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