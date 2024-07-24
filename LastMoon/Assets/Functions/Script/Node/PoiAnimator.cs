using UnityEngine;
using Photon.Pun;
public class PoiAnimator : MonoBehaviourPunCallbacks
{
    public Animator animator;
    private bool ani;
    // Start is called before the first frame update

    public AudioSource sfx_Station_Start, sfx_Station_Loop;

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
        if (ani)
        {
            sfx_Station_Loop.Play();
            animator.SetBool("isActvie", ani);
        }
        else
            sfx_Station_Loop.Stop();
            animator.SetBool("isActvie", ani);
    }
}
