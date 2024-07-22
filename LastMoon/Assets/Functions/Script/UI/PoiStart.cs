using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PoiStart : MonoBehaviour
{
   
    public Image image_fill; // fill type을 통해 연출할 이미지
    public Image alphaNode;
    public Sprite resultNode;
    public float time_coolTime = 2; // 쿨타임 public으로 인스펙터에서 조절할 수 있게 했다.
    
    private float time_current; // 진행된 시간을 저장할 필드 변수
    private bool isEnded = true; // 종료 여부를 저장할 필드 변수
    private int loopStart;
    
    public Text node1_Text;
    public Text node2_Text;
    public Text node_Text;

    private void Start()
    {
        loopStart = 0;
    }

    private void Update() // 매 프레임 쿨타임을 체크한다.
    {
        if (isEnded)
        {
              return;
        }
            
        Check_CoolTime();
    }

    void Check_CoolTime()
    {
        time_current += Time.deltaTime; //증가한 시간을 더한다.
        if (time_current < time_coolTime) //아직 쿨타임이 안됐으면
        {
            Set_FillAmount(time_current); //이미지를 갱신한다.
        }
        else if (!isEnded)//쿨타임이 다됐는데 안끝났으면
        {
            End_CoolTime(); //쿨타임을 끝낸다.
        }
    }

    void End_CoolTime()
    {
        Set_FillAmount(time_coolTime); //이미지를 갱신한다.
        isEnded = true; //끝낸다.
        alphaNode.sprite = resultNode;
        Debug.Log("루프 끝남");
    }

    void Trigger_Skill()
    {
        if (!isEnded) return; //아직 쿨타임이면 안한다.

        Reset_CoolTime(); // 쿨타임을 돌린다.
    }

    void Reset_CoolTime()
    {
      
        time_current = 0;
        Set_FillAmount(0);
        isEnded = false;
    }

    void Set_FillAmount(float value)
    {
        image_fill.fillAmount = value / time_coolTime;
        
    }

    public void on_Btn() //버튼 입력을 받아서 스킬을 시전한 걸로 친다.
    {
        int node1Value = int.Parse(node1_Text.text);
        int node2Value = int.Parse(node2_Text.text);

        if (node1Value < node2Value)
        {
            loopStart = node1Value;
        }
        else
        {
            loopStart = node2Value;
        }

        Trigger_Skill();

    }
}
