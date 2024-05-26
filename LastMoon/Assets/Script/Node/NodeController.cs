using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NodeController : MonoBehaviourPun
{
    int NodeHp = 100;

    public PhotonView PV;

    public Animator animator;
    // 초기 설정
   public void RCPUpdate()
    {
        PV.RPC("NodeHP", RpcTarget.All,10);
    }

    [PunRPC]
    void NodeHP (int Damge)
    {
        NodeHp -= Damge;
        Debug.Log(NodeHp);

        if(NodeHp>0)
        {
            animator.SetTrigger("Hit");
        }
        else if(NodeHp<=0)
        {
            animator.SetTrigger("Destroy");
        }
    }
    public void OnDestroyAnimationEnd()
    {
        // 마스터 클라이언트가 오브젝트를 삭제하도록 요청
        PV.RPC("RPC_DestroyNode", RpcTarget.MasterClient);
    }
    [PunRPC]
    void RPC_DestroyNode()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}