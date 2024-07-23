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
    private bool isRunning = false; // 쿨타임 체크가 실행 중인지 여부
    private int loopStart;
    private int currentLoopIndex = 0; // 현재 루프 인덱스

    public Text node1_Text;
    public Text node2_Text;
    public Text node_Text;

    private int node1Value;
    private int node2Value;

    private Coroutine repeatCoroutine; // 코루틴 참조를 저장할 변수

    private void Start()
    {
        loopStart = 0;
        node1Value = 0;
        node2Value = 0;
        node_Text.text = " ";
    }

    private void Update()
    {
        // Update 메서드에서 코루틴의 진행을 제어하지 않음
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
        Debug.Log("루프 끝남");
    }

    IEnumerator ExecuteRepeatedly()
    {
        for (currentLoopIndex = 0; currentLoopIndex < loopStart; currentLoopIndex++)
        {
            isRunning = true; // 쿨타임 시작

            Reset_CoolTime();

            while (isRunning) // 쿨타임이 진행 중일 때 대기
            {
                Check_CoolTime(); // 쿨타임 체크
                yield return null;
            }

            // node_Text의 값 증가
            IncreaseNodeTextValue();

            // 재료 소모
            int node1Value = int.Parse(node1_Text.text) - 1;
            int node2Value = int.Parse(node2_Text.text) - 1;
            node1_Text.text = node1Value.ToString();
            node2_Text.text = node2Value.ToString();
        }

        Debug.Log("모든 루프 종료");
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

        loopStart = Mathf.Min(node1Value, node2Value); // loopStart의 최솟값 설정

        if (repeatCoroutine != null)
        {
            StopCoroutine(repeatCoroutine); // 이전 코루틴 중지
        }

        repeatCoroutine = StartCoroutine(ExecuteRepeatedly()); // 코루틴 시작
    }

    public void ResumeExecution()
    {
        if (repeatCoroutine == null && loopStart > 0)
        {
            repeatCoroutine = StartCoroutine(ExecuteRepeatedly());
        }
    }
}