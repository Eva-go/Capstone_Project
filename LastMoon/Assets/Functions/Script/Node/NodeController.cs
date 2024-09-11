using UnityEngine;
using Photon.Pun;

public class NodeController : MonoBehaviourPunCallbacks, IPunObservable
{
    public float maxHealth = 30f;
    [HideInInspector]
    public float currentHealth;
    public int Node_Type;
    public Animator animator;
    public string nodeName;

    public AudioSource sfx_NodeHit;
    public AudioClip sfx_NodeHit1, sfx_NodeHit2, sfx_NodeHit3, sfx_NodeHit4;

    public ParticleSystem HitParticle;

    public int nodeCount;

    public ScriptableObject_Item NodeItemType;

    //private int giveMoney = 50;

    private void Start()
    {
        currentHealth = maxHealth;
        nodeCount = 0;
    }

    public void TakeDamage(float Damage, bool Effective)
    {

        float R_sfx = Random.value;

        if (R_sfx > 0.75) sfx_NodeHit.clip = sfx_NodeHit1;
        else if (R_sfx > 0.5) sfx_NodeHit.clip = sfx_NodeHit2;
        else if (R_sfx > 0.25) sfx_NodeHit.clip = sfx_NodeHit3;
        else sfx_NodeHit.clip = sfx_NodeHit4;

        if (Effective)
        {
            sfx_NodeHit.pitch = 1;
            sfx_NodeHit.volume = 1;
        }
        else
        {
            sfx_NodeHit.pitch = 0.5f;
            sfx_NodeHit.volume = 0.5f;
        }

        sfx_NodeHit.Play();
        HitParticle.Play();

        currentHealth -= Damage;
        if (currentHealth > 0)
        {
            //photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Hit");
            animator.SetTrigger("Hit");

        }
        else if (currentHealth <= 0)
        {
            if (Effective)
            {
                currentHealth = 0;
                gameObject.GetComponent<BoxCollider>().enabled = false;
                //GameValue.GetMomey(PlayerController.getMoney);
                photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Harvest");
                // 애니메이션 끝날 때 오브젝트 삭제
                nodeName = gameObject.name;
                nodeCount++;
            }
            else
            {
                photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Destroy");
               
            }

        }

        // 포톤 네트워크를 통해 HP 동기화
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
        if (stream.IsWriting)
        {
            // 포톤 네트워크를 통해 HP 값을 전송
            stream.SendNext(currentHealth);
        }
        else
        {
            // 다른 플레이어로부터 HP 값을 수신
            currentHealth = (float)stream.ReceiveNext();
        }
    }
}