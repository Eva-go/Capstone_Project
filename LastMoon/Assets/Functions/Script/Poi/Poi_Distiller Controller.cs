using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Poi_DistillerController : MonoBehaviour
{
    public PhotonView pv;

    public Animator animator;
    public int nodeItme = 0;
    public int mixItme = 0;
    public int nodeCount = 0;
    public int mixoldItem = 0;
    public int nodeNumber = 0;
    public string nodeName = "Dirt";
    private string playerName= " ";
    private bool processing = false;

    public GameObject Flame;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        nodeName = "Dirt";
        nodeNumber = 0;
        Flame.SetActive(false);
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
                Debug.Log("������ ����" + nodeCount);
                StartCoroutine(ProcessItems());
            }
        }
    }

    private IEnumerator ProcessItems()
    {
        processing = true;
        Flame.SetActive(true);
        animator.SetBool("isActvie", true);
        while (nodeItme > 0)
        {
            nodeItme--;
           
            
            yield return new WaitForSeconds(5f);
            mixItme++;
            Debug.Log("������ ����");
        }
        nodeCount = 0;
        processing = false;
        animator.SetBool("isActvie", false);
        Flame.SetActive(false);
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
        Debug.Log("������ ����");
        return mixoldItem;
    }
}
