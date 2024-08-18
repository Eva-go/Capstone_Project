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
    public Text countText;
    private Coroutine countCoroutine; // ī��Ʈ ���� �ڷ�ƾ

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        countText = GameObject.FindGameObjectWithTag("CountText")?.GetComponent<Text>();
        if (countText == null)
        {
            Debug.LogWarning("CountText UI element not found.");
        }
        UpdateCountText();
        SetLinkObjectsActive(false); // �ʱ⿡�� ��� Link ������Ʈ�� ��Ȱ��ȭ
    }

    public void StartCounting(int playerId)
    {
        if (interactingPlayerId == -1)
        {
            interactingPlayerId = playerId;
            SetLinkObjectsActive(true); // �������� �ο��� �� Link ������Ʈ Ȱ��ȭ
            if (countCoroutine != null)
            {
                StopCoroutine(countCoroutine); // ���� �ڷ�ƾ ����
            }
            countCoroutine = StartCoroutine(CountUpCoroutine()); // �� �ڷ�ƾ ����
        }
    }

    private IEnumerator CountUpCoroutine()
    {
        while (interactingPlayerId != -1)
        {
            count++;
            UpdateCountText();
            yield return new WaitForSeconds(1f); // 1�ʸ��� ����
        }
    }

    public void StopCounting()
    {
        if (interactingPlayerId != -1)
        {
            interactingPlayerId = -1;
            SetLinkObjectsActive(false); // �������� ���µǸ� Link ������Ʈ ��Ȱ��ȭ
            if (countCoroutine != null)
            {
                StopCoroutine(countCoroutine); // �ڷ�ƾ ����
                countCoroutine = null;
            }
        }
    }

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
        Debug.Log("Resetting ownership and count.");
        StopCounting(); // ī��Ʈ ���� �� ������ �ʱ�ȭ
        ResetInteraction(); // ī��Ʈ �ʱ�ȭ
        SetLinkObjectsActive(state); // Link ������Ʈ ��Ȱ��ȭ
    }

    private void ResetInteraction()
    {
        count = 0; // ī��Ʈ �ʱ�ȭ
        UpdateCountText(); // UI ������Ʈ
    }

    public void SetLinkObjectsActive(bool state)
    {
        gameObject.transform.GetChild(0).transform.Find("Drill_Body001").gameObject.SetActive(state);
    }

    private void UpdateCountText()
    {
        if (countText != null)
        {
            countText.text = "Count: " + count.ToString();
        }
    }

    public int GetInteractingPlayerId()
    {
        return interactingPlayerId;
    }

    public void SetInteractingPlayerId(int playerId)
    {
        StartCounting(playerId);
    }
}
