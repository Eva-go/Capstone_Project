using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class TimerController : MonoBehaviourPunCallbacks
{
    public RectTransform timerImage;
    private float totalTime; // 10분을 초 단위로 표시
    public float decreaseTime = 180f; // f1 누를 때 감소시킬 시간 (3분)

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
        // 타이머 감소 및 이미지 위치 조정
        currentTime -= Time.deltaTime;
        float ratio = currentTime / totalTime;
        float posX = Mathf.Lerp(finalPosX, initialPosX, ratio);
        timerImage.localPosition = new Vector3(posX, timerImage.localPosition.y, timerImage.localPosition.z);

        // 타이머 텍스트 업데이트
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        // f1 키를 누르면 시간 감소
        if (Input.GetKeyDown(KeyCode.F1))
        {
            photonView.RPC("RPC_DecreaseTime", RpcTarget.AllBuffered); 
        }
        if(currentTime <=0)
        {
            totalTime = 500f;
        }
    }

    // 시간을 감소시키는 함수
    [PunRPC]
    void RPC_DecreaseTime()
    {
        currentTime -= decreaseTime;
        // 타이머가 음수가 되지 않도록 보정
        if (currentTime < 0)
        {
            currentTime = 0;
        }
    }
}