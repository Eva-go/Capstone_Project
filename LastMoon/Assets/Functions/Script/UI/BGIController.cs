using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGIController : MonoBehaviour
{
    public Sprite Round1, Round2, Round3, Round4, Round5, Round6, Round7, Round8, Round9, Round10;

    void Start()
    {
        int CurruntRound;
        if (GameValue.Round >= GameValue.MaxRound)
        {
            CurruntRound = 10;
        }
        else
        {
            CurruntRound = (int)Mathf.Round((GameValue.Round - 1) * 10 / GameValue.MaxRound );
        }
        switch (CurruntRound)
        {
            case 1:
                gameObject.GetComponent<Image>().sprite = Round1;
                break;
            case 2:
                gameObject.GetComponent<Image>().sprite = Round2;
                break;
            case 3:
                gameObject.GetComponent<Image>().sprite = Round3;
                break;
            case 4:
                gameObject.GetComponent<Image>().sprite = Round4;
                break;
            case 5:
                gameObject.GetComponent<Image>().sprite = Round5;
                break;
            case 6:
                gameObject.GetComponent<Image>().sprite = Round6;
                break;
            case 7:
                gameObject.GetComponent<Image>().sprite = Round7;
                break;
            case 8:
                gameObject.GetComponent<Image>().sprite = Round8;
                break;
            case 9:
                gameObject.GetComponent<Image>().sprite = Round9;
                break;
            case 10:
                gameObject.GetComponent<Image>().sprite = Round10;
                break;
            default:
                gameObject.GetComponent<Image>().sprite = Round1;
                break;
        }
    }
}
