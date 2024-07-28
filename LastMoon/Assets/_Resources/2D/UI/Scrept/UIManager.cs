using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BringPanelToFront(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            panel.transform.SetAsLastSibling(); // �г��� ���� ������ ����
        }
    }
}