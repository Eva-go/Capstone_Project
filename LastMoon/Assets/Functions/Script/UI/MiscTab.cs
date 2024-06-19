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
     
        // SetActive(false)�� ���¿����� ���������� ȣ��Ǵ� �Լ�
        if (!gameObject.activeSelf)
        {
            if (GameValue.lived)
            {
                Debug.Log("Ż��!");
                Bye();
            }

            if (PlayerController.Hp == 0)
            {
                Debug.Log("����!");
                Die();
            }
        }

        // ���� ���ϴ� ������Ʈ ���� �߰�
       nodeCountUpdate();
    }

    private void Initialize()
    {
        // �ʱ�ȭ�� �ʿ䰡 �ִ� ��� �̰����� �ʱ�ȭ ������ �߰��մϴ�.
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
