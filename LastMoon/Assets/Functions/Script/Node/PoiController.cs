using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PoiController : MonoBehaviourPunCallbacks
{
    private bool ani;
    public int poiItem;
    public string nodeItem;

    // �ִϸ�����
    public Animator animator;

    // ����
    public AudioSource sfx_Station_Start, sfx_Station_Loop;

    // ���� �ֱ� (�� ����)
    public float increaseInterval = 5.0f; // 5�ʸ��� ����

    // �ڷ�ƾ �ν��Ͻ� ����
    private Coroutine increaseCoroutine;

    // ��� ���� ����
    private static bool isBeingUsed = false;

    // ani�� ������ ID
    private int aniOwner = -1;

    void Start()
    {
        poiItem = 0;
        nodeItem = "";
    }

    // Update is called once per frame
    void Update()
    {
        // ���� �÷��̾ �������̰� ani�� ������ �� �ִ� ���
       
    }

    [PunRPC]
    void RPC_PoiSet(bool newAni, int ownerID)
    {
        ani = newAni;

        if (ani)
        {
            if (increaseCoroutine == null)
            {
                // �ڷ�ƾ�� ���� ���� �ƴϸ� ����
                increaseCoroutine = StartCoroutine(IncreasePoiItemRoutine());
            }

            aniOwner = ownerID; // ���� ani�� ������ ����
            isBeingUsed = true; // ��� �� ���� ����

            sfx_Station_Loop.Play();
            animator.SetBool("isActive", ani);
        }
        else
        {
            if (increaseCoroutine != null)
            {
                // �ڷ�ƾ�� ���� ���̸� ����
                StopCoroutine(increaseCoroutine);
                increaseCoroutine = null;
            }

            // �ڷ�ƾ ���� �� �ʱ�ȭ
            poiItem = 0;

            aniOwner = -1; // ������ �ʱ�ȭ
            isBeingUsed = false; // ��� �� ���� ����

            sfx_Station_Loop.Stop();
            animator.SetBool("isActive", ani);
        }
    }

    // PoiItem�� ������Ű�� �ڷ�ƾ
    IEnumerator IncreasePoiItemRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(increaseInterval); // increaseInterval �� ��ٸ�
            poiItem++; // poiItem ����
            Debug.Log($"poiItem increased to: {poiItem}"); // �ֿܼ� ������ �� ���

            // �ʿ信 ���� photonView�� ����Ͽ� ��� Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("RPC_UpdatePoiItem", RpcTarget.AllBuffered, poiItem);
        }
    }

    [PunRPC]
    void RPC_UpdatePoiItem(int newPoiItem)
    {
        poiItem = newPoiItem;
        // �߰������� ����Ʈ �������� ��ȭ�� ���� UI ������Ʈ�� �ٸ� ������ ������ �� ����
    }
}