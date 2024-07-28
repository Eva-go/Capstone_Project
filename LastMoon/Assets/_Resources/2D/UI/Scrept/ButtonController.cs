using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject panelToBringToFront;

    public void OnButtonClick()
    {
        if (UIManager.Instance != null && panelToBringToFront != null)
        {
            UIManager.Instance.BringPanelToFront(panelToBringToFront);
        }
    }

}