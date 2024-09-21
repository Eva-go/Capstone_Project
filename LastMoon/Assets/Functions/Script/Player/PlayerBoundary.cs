using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    public GameObject[] targetObjects; // 활성화/비활성화할 오브젝트 배열

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject target in targetObjects)
        {
            string targetName = target.name + "(Clone)";
            if (other.name == targetName)
            {
                Debug.Log("트리거" + other.name + " : " + targetName);
                other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                other.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 배열의 오브젝트 중 하나가 트리거에서 나간 경우
        foreach (GameObject target in targetObjects)
        {
            string targetName = target.name + "(Clone)";
            if (other.name == targetName)
            {
                other.gameObject.GetComponent<NodeController>().enabled = false;
                other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                other.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                break;
            }
        }
    }
}