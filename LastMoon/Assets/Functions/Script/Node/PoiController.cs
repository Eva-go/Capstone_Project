using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PoiController : MonoBehaviourPunCallbacks
{
    private bool ani;
    public int poiItem;
    public string nodeItem;

    // 애니메이터
    public Animator animator;

    // 사운드
    public AudioSource sfx_Station_Start, sfx_Station_Loop;

    // 증가 주기 (초 단위)
    public float increaseInterval = 5.0f; // 5초마다 증가

    // 코루틴 인스턴스 추적
    private Coroutine increaseCoroutine;

    // 사용 여부 상태
    private static bool isBeingUsed = false;

    // ani의 소유자 ID
    private int aniOwner = -1;

    void Start()
    {
        poiItem = 0;
        nodeItem = "";
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 플레이어가 소유자이고 ani를 조작할 수 있는 경우
       
    }

    [PunRPC]
    void RPC_PoiSet(bool newAni, int ownerID)
    {
        ani = newAni;

        if (ani)
        {
            if (increaseCoroutine == null)
            {
                // 코루틴이 실행 중이 아니면 시작
                increaseCoroutine = StartCoroutine(IncreasePoiItemRoutine());
            }

            aniOwner = ownerID; // 현재 ani의 소유자 설정
            isBeingUsed = true; // 사용 중 상태 설정

            sfx_Station_Loop.Play();
            animator.SetBool("isActive", ani);
        }
        else
        {
            if (increaseCoroutine != null)
            {
                // 코루틴이 실행 중이면 중지
                StopCoroutine(increaseCoroutine);
                increaseCoroutine = null;
            }

            // 코루틴 중지 시 초기화
            poiItem = 0;

            aniOwner = -1; // 소유자 초기화
            isBeingUsed = false; // 사용 중 상태 해제

            sfx_Station_Loop.Stop();
            animator.SetBool("isActive", ani);
        }
    }

    // PoiItem을 증가시키는 코루틴
    IEnumerator IncreasePoiItemRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(increaseInterval); // increaseInterval 초 기다림
            poiItem++; // poiItem 증가
            Debug.Log($"poiItem increased to: {poiItem}"); // 콘솔에 증가된 값 출력

            // 필요에 따라 photonView를 사용하여 모든 클라이언트에 동기화
            photonView.RPC("RPC_UpdatePoiItem", RpcTarget.AllBuffered, poiItem);
        }
    }

    [PunRPC]
    void RPC_UpdatePoiItem(int newPoiItem)
    {
        poiItem = newPoiItem;
        // 추가적으로 포인트 아이템의 변화에 따라 UI 업데이트나 다른 로직을 수행할 수 있음
    }
}