using System.Collections;
using UnityEngine;
using Photon.Pun;


[System.Serializable]
public struct ItemData
{
    public string[] nodeName;
    public int[] nodeItemCount;
    public string[] mixName;
    public int[] mixItemCount;
}


public class PoiController : MonoBehaviour
{
    public PhotonView pv;
    public ItemData itemData;
    public string poiName;
    public int SetnodeCount;
    public Animator animator;
    private string playerName = " ";
    private bool processing = false;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        processing = false;
        SetnodeCount = 0;
        itemData.nodeItemCount = new int[6];
        itemData.mixItemCount = new int[6];
        itemData.nodeName = new string[] { "Dirt", "Concrete", "Driftwood", "Sand", "Planks", "Scrap" };
        itemData.mixName = new string[] { "clayware", "porcelain", "quartz_glass", "sodaLime_glass", "iron", "steel" };
        poiName = gameObject.name;
        for (int i = 0; i < itemData.nodeItemCount.Length; i++)
        {
            itemData.nodeItemCount[i] = 0;
            itemData.mixItemCount[i] = 0;
        }
    }

    [PunRPC]
    public void ReceiveData(int nodeItemCount, string nodeName, string playerNickName, int i)
    {
        if (itemData.nodeName[i].Equals(nodeName) && nodeItemCount >= 0)
        {
            if (playerName == " ")
            {
                SetnodeCount = nodeItemCount;
                itemData.nodeItemCount[i]++;
                playerName = playerNickName;
            }
            else if (playerName == playerNickName)
            {
                itemData.nodeItemCount[i]++;
            }

            if (!processing)
            {
                StartCoroutine(ProcessItems(i));
            }
        }
    }

    private IEnumerator ProcessItems(int i)
    {
        processing = true;
        animator.SetBool("isActvie", true);
        while (itemData.nodeItemCount[i] > 0)
        {
            itemData.nodeItemCount[i]--;
            yield return new WaitForSeconds(5f);
            itemData.mixItemCount[i]++;
        }
        itemData.nodeItemCount[i] = 0;
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
    public int GetMixItem(int i)
    {
        int mixoldItem = itemData.mixItemCount[i];
        itemData.mixItemCount[i] = 0;
        return mixoldItem;
    }
}
