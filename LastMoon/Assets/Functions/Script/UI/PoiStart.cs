using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PoiStart : MonoBehaviour
{
    public Image image_fill;
    public Image alphaNode;
    public Sprite resultNode;
    public float time_coolTime = 2;

    private float time_current;
    private bool isRunning = false; // ��Ÿ�� üũ�� ���� ������ ����
    private int loopStart;
    private int currentLoopIndex = 0; // ���� ���� �ε���

    public Text node1_Text;
    public Text node2_Text;
    public Text node_Text;

    private int node1Value;
    private int node2Value;

    private Coroutine repeatCoroutine; // �ڷ�ƾ ������ ������ ����

    private void Start()
    {
        loopStart = 0;
        node1Value = 0;
        node2Value = 0;
        node_Text.text = " ";
    }

    private void Update()
    {
        // Update �޼��忡�� �ڷ�ƾ�� ������ �������� ����
    }

    void Check_CoolTime()
    {
        time_current += Time.deltaTime;
        if (time_current < time_coolTime)
        {
            Set_FillAmount(time_current);
        }
        else
        {
            End_CoolTime();
        }
    }

    void End_CoolTime()
    {
        Set_FillAmount(time_coolTime);
        isRunning = false;
        alphaNode.sprite = resultNode;
        Debug.Log("���� ����");
    }

    IEnumerator ExecuteRepeatedly()
    {
        for (currentLoopIndex = 0; currentLoopIndex < loopStart; currentLoopIndex++)
        {
            isRunning = true; // ��Ÿ�� ����

            Reset_CoolTime();

            while (isRunning) // ��Ÿ���� ���� ���� �� ���
            {
                Check_CoolTime(); // ��Ÿ�� üũ
                yield return null;
            }

            // node_Text�� �� ����
            IncreaseNodeTextValue();

            // ��� �Ҹ�
            int node1Value = int.Parse(node1_Text.text) - 1;
            int node2Value = int.Parse(node2_Text.text) - 1;
            node1_Text.text = node1Value.ToString();
            node2_Text.text = node2Value.ToString();
        }

        Debug.Log("��� ���� ����");
    }

    void IncreaseNodeTextValue()
    {
        if (int.TryParse(node_Text.text, out int currentValue))
        {
            currentValue++;
            node_Text.text = currentValue.ToString();
        }
        else
        {
            node_Text.text = "1";
        }
    }

    void Reset_CoolTime()
    {
        time_current = 0;
        Set_FillAmount(0);
        isRunning = true;
    }

    void Set_FillAmount(float value)
    {
        image_fill.fillAmount = value / time_coolTime;
    }

    public void on_Btn()
    {
        int node1Value = int.Parse(node1_Text.text);
        int node2Value = int.Parse(node2_Text.text);

        loopStart = Mathf.Min(node1Value, node2Value); // loopStart�� �ּڰ� ����

        if (repeatCoroutine != null)
        {
            StopCoroutine(repeatCoroutine); // ���� �ڷ�ƾ ����
        }

        repeatCoroutine = StartCoroutine(ExecuteRepeatedly()); // �ڷ�ƾ ����
    }

    public void ResumeExecution()
    {
        if (repeatCoroutine == null && loopStart > 0)
        {
            repeatCoroutine = StartCoroutine(ExecuteRepeatedly());
        }
    }
}