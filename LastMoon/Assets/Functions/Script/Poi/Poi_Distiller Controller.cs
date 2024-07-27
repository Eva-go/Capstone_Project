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
    private int mixoldItem = 0;
    private int DirtNumber = 0;
    public string nodeName = "Dirt";
    private string playerName= " ";
    private bool processing = false;
    void Start()
    {
        pv = GetComponent<PhotonView>();
        nodeName = "Dirt";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [PunRPC]
    public void ReceiveData(int nodeItemCount, string nodenames, string playerNickName)
    {
        if (nodeName.Equals(nodenames))
        {
            if (playerName == " " && nodeItemCount > 0)
            {
                nodeCount = nodeItemCount;
                // ������ ���� �����մϴ�
                nodeItme++;
                playerName = playerNickName;
            }
            else if (playerName == playerNickName)
            {
                nodeItme++;
            }

            // Start processing only if not already processing
            if (!processing)
            {
                Debug.Log("������ ����" + nodeCount);
                StartCoroutine(ProcessItems());
            }
        }
    }
    private IEnumerator ProcessItems()
    {
        processing = true; // Start processing
        while (nodeItme > 0)
        {
           
            nodeItme--; // Decrease node item count
            if (nodeCount>=0)
            {
                nodeCount--;
                // PlayerController�� nodeItiems[i] ���� ������Ʈ
                if (pv.IsMine)
                {
                    pv.RPC("UpdatePlayerNodeItem", RpcTarget.AllBuffered, DirtNumber, nodeCount);
                }
                animator.SetBool("isActvie", true);
                yield return new WaitForSeconds(5f); // Wait for 5 seconds

                mixItme++; // Increase mix item count
               

                Debug.Log("������ ����");
            }
        }
        nodeCount = 0;
        processing = false; // Stop processing
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
        Debug.Log("������ ����");
        return mixoldItem;
    }
}
