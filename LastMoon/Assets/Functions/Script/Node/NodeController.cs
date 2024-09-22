using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class NodeController : MonoBehaviour
{
    public float maxHealth = 10f;
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

    public  int nodeID = -1;

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
            gameObject.transform.parent.GetComponent<PlaceNode>().Ani_Hit(nodeID);

        }
        else if (currentHealth <= 0)
        {
            if (Effective)
            {
                currentHealth = 0;
                gameObject.GetComponent<BoxCollider>().enabled = false;
                //GameValue.GetMomey(PlayerController.getMoney);
                //photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Harvest");
                gameObject.transform.parent.GetComponent<PlaceNode>().Ani_Harvest(nodeID);
                // 애니메이션 끝날 때 오브젝트 삭제
                nodeName = gameObject.name;
                nodeCount++;
            }
            else
            {
                //photonView.RPC("RPC_SetTrigger", RpcTarget.AllBuffered, "Destroy");
                gameObject.transform.parent.GetComponent<PlaceNode>().Ani_Destory(nodeID);
            }

        }
        // 포톤 네트워크를 통해 HP 동기화
        //photonView.RPC("SyncHealth", RpcTarget.OthersBuffered, currentHealth);
        gameObject.transform.parent.GetComponent<PlaceNode>().node_Hp(nodeID);
    }

    public void nodeDestroy()
    {
        gameObject.transform.parent.GetComponent<PlaceNode>().node_Destory(nodeID);
    }
}