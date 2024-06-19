using UnityEngine;
using UnityEngine.UI;

public class MiscTab : MonoBehaviour
{
    public GameObject[] nodes;
    public Text[] nodesCount;
    public int[] count;
    public int[] byeCount;
    public int[] salePrice;

    private void Start()
    {
        for (int i = 0; i < nodesCount.Length; i++)
        {
            nodesCount[i].text = "0";
            count[i] = 0;
        }
    }

    private void Update()
    {
     
        // SetActive(false)인 상태에서도 지속적으로 호출되는 함수
        if (!gameObject.activeSelf)
        {
            if (GameValue.lived)
            {
                Debug.Log("탈출!");
                Bye();
            }

            if (PlayerController.Hp == 0)
            {
                Debug.Log("죽음!");
                Die();
            }
        }

        // 이후 원하는 업데이트 로직 추가
       nodeCountUpdate();
    }

    private void Initialize()
    {
        // 초기화할 필요가 있는 경우 이곳에서 초기화 로직을 추가합니다.
    }

    public void nodeCountUpdate()
    {

        for (int i = 0; i < nodesCount.Length; i++)
        {
            if (nodes[i].name + "(Clone)" == GameValue.NodeName)
            {
                count[i]++;
                byeCount[i] = count[i];
                nodesCount[i].text = count[i].ToString();
                GameValue.GetNode("");
            }
        }
    }

    private void Bye()
    {
        for (int i = 0; i < nodesCount.Length; i++)
        {
            
        }
        GameValue.lived = false;
    }

    private void Die()
    {
        for (int i = 0; i < nodesCount.Length; i++)
        {
            nodesCount[i].text = "0";
        }
    }
}
