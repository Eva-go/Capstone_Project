using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class GameTimer : MonoBehaviourPunCallbacks
{
    public RectTransform timerImage;
    private float totalTime;
    public float decreaseTime = 180f; // f1 누를 때 감소시킬 시간 (3분)

    public float currentTime;
    private float initialPosX = 234f;
    private float finalPosX = -234f;


    void Start()
    {
        GameValue.RoundEnd = false;
        totalTime = 0.5f * (GameValue.Round - 1)*60f / GameValue.MaxRound*60f + (GameValue.setMaxtime * 60f);
        currentTime = totalTime;
        timerImage.localPosition = new Vector3(initialPosX, timerImage.localPosition.y, timerImage.localPosition.z);
        PhotonNetwork.AutomaticallySyncScene = true;
        GameValue.WaveTimer = currentTime;
        GameValue.WaveTimerMax = totalTime;

        Debug.Log("토탈 타이머: " + totalTime+"현재 라운드: "+GameValue.Round+"최대 라운드: "+GameValue.MaxRound+"세팅 시간: "+GameValue.setMaxtime);
    }

    void Update()
    {
        // 타이머 감소 및 이미지 위치 조정
        if (SceneManager.GetActiveScene().name == "Map")
        {
            currentTime -= Time.deltaTime;
            GameValue.WaveTimer = currentTime;
            GameValue.WaveTimerMax = totalTime;
            float ratio = currentTime / totalTime;
            float posX = Mathf.Lerp(finalPosX, initialPosX, ratio);
            timerImage.localPosition = new Vector3(posX, timerImage.localPosition.y, timerImage.localPosition.z);

            // 타이머 텍스트 업데이트
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
        }
        if(GameValue.inside)
        {
            photonView.RPC("RPC_All_Inside_Time", RpcTarget.AllBuffered);
            GameValue.inside = false;
        }
        // f1 키를 누르면 시간 감소
        if (Input.GetKeyDown(KeyCode.F1))
        {
            photonView.RPC("RPC_DecreaseTime", RpcTarget.AllBuffered);
        }
        RoundEnd();
    }
    [PunRPC]
    void RPC_All_Inside_Time()
    {
        GameValue.insideUser++;
        if(GameValue.insideUser >= GameValue.MaxUser)
        {
            currentTime = 0;
        }
    }
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

    private void RoundEnd()
    {
        if (currentTime <= 0 && SceneManager.GetActiveScene().name == "Map" && !GameValue.RoundEnd)
        {
            if (GameValue.Round < GameValue.MaxRound)
            {
                GameValue.Round += 1;
                GameValue.RoundEnd = true;
                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.LoadLevel("Shop");
                totalTime = GameValue.setMaxtime * 60;
                GameValue.insideUser = 0;
            }
            else
            {
                GameValue.RoundEnd = true;

                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.LoadLevel("GameEnding");
            }
        }
    }
}