using UnityEngine;
using Photon.Pun;

public class Node : MonoBehaviourPunCallbacks, IPunObservable
{
    public int maxHealth = 100;
    [HideInInspector]
    public int currentHealth;

    public Animator animator;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0)
        {
            photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Hit");
            
        }
        else if (currentHealth <= 0)
        {
            currentHealth = 0;
            photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Destroy");
            // 애니메이션 끝날 때 오브젝트 삭제
        }

        // 포톤 네트워크를 통해 HP 동기화
        photonView.RPC("SyncHealth", RpcTarget.OthersBuffered, currentHealth);
        Debug.Log(currentHealth);
    }

    [PunRPC]
    void SyncHealth(int health)
    {
        currentHealth = health;
    }

    [PunRPC]
    void RPC_SetTrigger(string triggerName)
    {
        Debug.Log("RPC_SetTrigger: " + triggerName);
        animator.SetTrigger(triggerName);
    }

    // 애니메이션 이벤트로 호출될 메서드
    public void OnDestroyAnimationEnd()
    {
        // 마스터 클라이언트가 오브젝트를 삭제하도록 요청
        photonView.RPC("RPC_DestroyNode", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_DestroyNode()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonSerializeView called");

        if (stream.IsWriting)
        {
            // 포톤 네트워크를 통해 HP 값을 전송
            stream.SendNext(currentHealth);
        }
        else
        {
            // 다른 플레이어로부터 HP 값을 수신
            currentHealth = (int)stream.ReceiveNext();
        }
    }
}