using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PoiStart : MonoBehaviour
{
   
    public Image image_fill; // fill type�� ���� ������ �̹���
    public Image alphaNode;
    public Sprite resultNode;
    public float time_coolTime = 2; // ��Ÿ�� public���� �ν����Ϳ��� ������ �� �ְ� �ߴ�.
    private float time_current; // ����� �ð��� ������ �ʵ� ����
    private bool isEnded = true; // ���� ���θ� ������ �ʵ� ����


    private void Update() // �� ������ ��Ÿ���� üũ�Ѵ�.
    {
        if (isEnded)
        {
              return;
        }
            
        Check_CoolTime();
    }

    void Check_CoolTime()
    {
        time_current += Time.deltaTime; //������ �ð��� ���Ѵ�.
        if (time_current < time_coolTime) //���� ��Ÿ���� �ȵ�����
        {
            Set_FillAmount(time_current); //�̹����� �����Ѵ�.
        }
        else if (!isEnded)//��Ÿ���� �ٵƴµ� �ȳ�������
        {
            End_CoolTime(); //��Ÿ���� ������.
        }
    }

    void End_CoolTime()
    {
        Set_FillAmount(time_coolTime); //�̹����� �����Ѵ�.
        isEnded = true; //������.
        alphaNode.sprite = resultNode;

    }

    void Trigger_Skill()
    {
        if (!isEnded) return; //���� ��Ÿ���̸� ���Ѵ�.

        Reset_CoolTime(); // ��Ÿ���� ������.
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

    public void on_Btn() //��ư �Է��� �޾Ƽ� ��ų�� ������ �ɷ� ģ��.
    {
        Trigger_Skill();
    }
}
