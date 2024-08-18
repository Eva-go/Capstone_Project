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
    public Text countText;
    private Coroutine countCoroutine; // 카운트 증가 코루틴

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        countText = GameObject.FindGameObjectWithTag("CountText")?.GetComponent<Text>();
        if (countText == null)
        {
            Debug.LogWarning("CountText UI element not found.");
        }
        UpdateCountText();
        SetLinkObjectsActive(false); // 초기에는 모든 Link 오브젝트를 비활성화
    }

    public void StartCounting(int playerId)
    {
        if (interactingPlayerId == -1)
        {
            interactingPlayerId = playerId;
            SetLinkObjectsActive(true); // 소유권이 부여될 때 Link 오브젝트 활성화
            if (countCoroutine != null)
            {
                StopCoroutine(countCoroutine); // 기존 코루틴 중지
            }
            countCoroutine = StartCoroutine(CountUpCoroutine()); // 새 코루틴 시작
        }
    }

    private IEnumerator CountUpCoroutine()
    {
        while (interactingPlayerId != -1)
        {
            count++;
            UpdateCountText();
            yield return new WaitForSeconds(1f); // 1초마다 증가
        }
    }

    public void StopCounting()
    {
        if (interactingPlayerId != -1)
        {
            interactingPlayerId = -1;
            SetLinkObjectsActive(false); // 소유권이 리셋되면 Link 오브젝트 비활성화
            if (countCoroutine != null)
            {
                StopCoroutine(countCoroutine); // 코루틴 중지
                countCoroutine = null;
            }
        }
    }

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
        Debug.Log("Resetting ownership and count.");
        StopCounting(); // 카운트 중지 및 소유권 초기화
        ResetInteraction(); // 카운트 초기화
        SetLinkObjectsActive(state); // Link 오브젝트 비활성화
    }

    private void ResetInteraction()
    {
        count = 0; // 카운트 초기화
        UpdateCountText(); // UI 업데이트
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
