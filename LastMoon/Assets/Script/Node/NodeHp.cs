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
            // �ִϸ��̼� ���� �� ������Ʈ ����
        }

        // ���� ��Ʈ��ũ�� ���� HP ����ȭ
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

    // �ִϸ��̼� �̺�Ʈ�� ȣ��� �޼���
    public void OnDestroyAnimationEnd()
    {
        // ������ Ŭ���̾�Ʈ�� ������Ʈ�� �����ϵ��� ��û
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
            // ���� ��Ʈ��ũ�� ���� HP ���� ����
            stream.SendNext(currentHealth);
        }
        else
        {
            // �ٸ� �÷��̾�κ��� HP ���� ����
            currentHealth = (int)stream.ReceiveNext();
        }
    }
}