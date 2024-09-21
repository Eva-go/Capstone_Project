using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
    public GameObject[] targetObjects; // Ȱ��ȭ/��Ȱ��ȭ�� ������Ʈ �迭

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject target in targetObjects)
        {
            string targetName = target.name + "(Clone)";
            if (other.name == targetName)
            {
                Debug.Log("Ʈ����" + other.name + " : " + targetName);
                other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                other.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �迭�� ������Ʈ �� �ϳ��� Ʈ���ſ��� ���� ���
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