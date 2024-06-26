using UnityEngine;
using Photon.Pun;
public class PoiAnimator : MonoBehaviourPunCallbacks
{
    public Animator animator;
    private bool ani;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ani = PlayerController.Poi;
        photonView.RPC("RPC_PoiSet", RpcTarget.AllBuffered, ani);

    }
    [PunRPC]
    void RPC_PoiSet(bool ani)
    {
        Debug.Log("Ani: " + ani);
        if (ani)
        {
            animator.SetBool("isActvie", ani);
        }
        else
            animator.SetBool("isActvie", ani);
    }
}
