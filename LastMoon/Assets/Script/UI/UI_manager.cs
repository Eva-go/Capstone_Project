using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class Craft
{
    public string Name;
    public GameObject Object_prefab; //��ġ�� ������
    public GameObject Object_prview; // �̸����� ������
}

public class UI_manager : MonoBehaviour
{
    public Sprite[] buttonImage;
    public Button[] buttons;

    [SerializeField]
    private Craft[] carfts; //��

    private static GameObject preview; //�̸����� �������� ���� ���� 
    

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
                Debug.Log("����");
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
