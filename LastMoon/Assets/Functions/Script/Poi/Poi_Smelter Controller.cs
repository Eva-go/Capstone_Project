using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Poi_SmelterController : MonoBehaviour
{
    public PhotonView pv;

    public Animator animator;
    public int nodeItme = 0;
    public int mixItme = 0;
    public int nodeCount = 0;
    public int mixoldItem = 0;
    public int nodeNumber = 5;
    public string nodeName = "Scrap";
    private string playerName = " ";
    private bool processing = false;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        nodeName = "Scrap";
        nodeNumber = 5;
    }

    [PunRPC]
    public void ReceiveData(int nodeItemCount, string nodenames, string playerNickName)
    {
        if (nodeName.Equals(nodenames))
        {
            if (playerName == " " && nodeItemCount >= 0)
            {
                nodeCount = nodeItemCount;
                nodeItme++;
                playerName = playerNickName;
            }
            else if (playerName == playerNickName)
            {
                nodeItme++;
            }

            if (!processing)
            {
                Debug.Log("아이템 갯수" + nodeCount);
                StartCoroutine(ProcessItems());
            }
        }
    }

    private IEnumerator ProcessItems()
    {
        processing = true;
        while (nodeItme > 0)
        {
            nodeItme--;
            animator.SetBool("isActvie", true);
            yield return new WaitForSeconds(5f);
            mixItme++;
            Debug.Log("아이템 제작");
        }
        nodeCount = 0;
        processing = false;
        animator.SetBool("isActvie", false);
    }

    [PunRPC]
    public void UpdatePlayerNodeItem(int index, int newCount)
    {
        PlayerController localPlayer = LocalPlayerManger.Instance.GetLocalPlayer();
        if (localPlayer != null)
        {
            localPlayer.UpdateNodeItem(index, newCount);
        }
    }

    [PunRPC]
    public int GetMixItem()
    {
        mixoldItem = mixItme;
        mixItme = 0;
        Debug.Log("아이템 전송");
        return mixoldItem;
    }
}
