using UnityEngine;
using Photon.Pun;

public class NodeController : MonoBehaviourPunCallbacks, IPunObservable
{
    public float maxHealth = 30f;
    [HideInInspector]
    public float currentHealth;
    public Animator animator;
    public string nodeName;

    public AudioSource sfx_NodeHit;
    public AudioClip sfx_NodeHit1, sfx_NodeHit2, sfx_NodeHit3, sfx_NodeHit4;

    public int nodeCount;

    //private int giveMoney = 50;

    private void Start()
    {
        currentHealth = maxHealth;
        nodeCount = 0;
    }

    public void TakeDamage(float Damage)
    {

        float R_sfx = Random.value;

        if (R_sfx > 0.75) sfx_NodeHit.clip = sfx_NodeHit1;
        else if (R_sfx > 0.5) sfx_NodeHit.clip = sfx_NodeHit2;
        else if (R_sfx > 0.25) sfx_NodeHit.clip = sfx_NodeHit3;
        else sfx_NodeHit.clip = sfx_NodeHit4;
        sfx_NodeHit.Play();

        currentHealth -= Damage;
        if (currentHealth > 0)
        {
            photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Hit");

        }
        else if (currentHealth <= 0)
        {
            currentHealth = 0;
            gameObject.GetComponent<BoxCollider>().enabled = false;
            //GameValue.GetMomey(PlayerController.getMoney);
            photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Harvest");
            // �ִϸ��̼� ���� �� ������Ʈ ����
            nodeName = gameObject.name;
            nodeCount++;
        }

        // ���� ��Ʈ��ũ�� ���� HP ����ȭ
        photonView.RPC("SyncHealth", RpcTarget.OthersBuffered, currentHealth);
    }
    [PunRPC]
    void SyncHealth(float health)
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
        if (stream.IsWriting)
        {
            // ���� ��Ʈ��ũ�� ���� HP ���� ����
            stream.SendNext(currentHealth);
        }
        else
        {
            // �ٸ� �÷��̾�κ��� HP ���� ����
            currentHealth = (float)stream.ReceiveNext();
        }
    }
}