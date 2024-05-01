using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class Craft
{
    public string Name;
    public GameObject Object_prefab; //설치될 프리팹
    public GameObject Object_prview; // 미리보기 프리뷰
}

public class UI_manager : MonoBehaviour
{
    public Sprite[] buttonImage;
    public Button[] buttons;

    [SerializeField]
    private Craft[] carfts; //탭

    private static GameObject preview; //미리보기 프리팹을 담을 변수 
    

    private PlayerController tf_Player;

    public static bool isPreviewActivated = false;

    private void Start()
    {
        tf_Player = GameObject.FindObjectOfType<PlayerController>();
        for (int i=0;i<buttons.Length;i++)
        {
            if (i < buttonImage.Length && buttonImage[i] != null)
            {
                buttons[i].image.sprite = buttonImage[i];
            }
            else
                Debug.Log("에러");
        }
    }

    public void SlotClick(int _slotNumber)
    {
        preview = Instantiate(carfts[_slotNumber].Object_prview, tf_Player.transform.position + tf_Player.transform.forward, Quaternion.identity);
        isPreviewActivated = true;
        GameObject.Find("Resources").transform.GetChild(0).gameObject.SetActive(false);
    }

    public static void Cancel()
    {
        if(isPreviewActivated)
            Destroy(preview);
        isPreviewActivated = false;
        preview = null;

    }

}
