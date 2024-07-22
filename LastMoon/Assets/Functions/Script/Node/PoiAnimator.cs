using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PoiAnimator : MonoBehaviourPunCallbacks
{
    public Animator animator;
    private bool ani;
    public GameObject PoiInventory;
    public GameObject PoiPopup;
    public GameObject PoiTab;
    // Start is called before the first frame update
    void Start()
    {
        PoiInventory.SetActive(false);
        PoiPopup.SetActive(false);
        PoiTab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ani = PlayerController.Poi;
        photonView.RPC("RPC_PoiSet", RpcTarget.AllBuffered, ani);
        UpdatePoiActive();
        //else
        //{
        //    PoiInventory.SetActive(false);
        //    PoiPopup.SetActive(false);
        //    Cursor.visible = false;
        //    Cursor.lockState = CursorLockMode.None;
        //}

    }
    void UpdatePoiActive()
    {
        bool isPoiActive = PlayerController.Poi;
        PoiPopup.SetActive(isPoiActive);
        if (isPoiActive)
        {
           
            PoiInventory.SetActive(true);
            PoiTab.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void Bt_poiExt()
    {
        PoiPopup.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        PlayerController.Poi = false;
        PoiInventory.SetActive(false);
        PoiTab.SetActive(false);
    }

    [PunRPC]
    void RPC_PoiSet(bool ani)
    {
        if (ani)
        {
            animator.SetBool("isActvie", ani);
        }
        else
            animator.SetBool("isActvie", ani);
    }
}
