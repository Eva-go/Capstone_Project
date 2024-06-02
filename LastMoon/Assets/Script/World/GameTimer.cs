using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class TimerController : MonoBehaviourPunCallbacks
{
    public RectTransform timerImage;
    private float totalTime; // 10���� �� ������ ǥ��
    public float decreaseTime = 180f; // f1 ���� �� ���ҽ�ų �ð� (3��)

    private float currentTime;
    private float initialPosX = 234f;
    private float finalPosX = -234f;
    private bool field = true;
    void Start()
    {
        if(field)
        {
            totalTime = Seed.setMaxtime * 60;
        }
       
        currentTime = totalTime;
        timerImage.localPosition = new Vector3(initialPosX, timerImage.localPosition.y, timerImage.localPosition.z);
    }

    void Update()
    {
        // Ÿ�̸� ���� �� �̹��� ��ġ ����
        currentTime -= Time.deltaTime;
        float ratio = currentTime / totalTime;
        float posX = Mathf.Lerp(finalPosX, initialPosX, ratio);
        timerImage.localPosition = new Vector3(posX, timerImage.localPosition.y, timerImage.localPosition.z);

        // Ÿ�̸� �ؽ�Ʈ ������Ʈ
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        // f1 Ű�� ������ �ð� ����
        if (Input.GetKeyDown(KeyCode.F1))
        {
            photonView.RPC("RPC_DecreaseTime", RpcTarget.AllBuffered); 
        }
        if(currentTime <=0)
        {
            totalTime = 500f;
        }
    }

    // �ð��� ���ҽ�Ű�� �Լ�
    [PunRPC]
    void RPC_DecreaseTime()
    {
        currentTime -= decreaseTime;
        // Ÿ�̸Ӱ� ������ ���� �ʵ��� ����
        if (currentTime < 0)
        {
            currentTime = 0;
        }
    }
}