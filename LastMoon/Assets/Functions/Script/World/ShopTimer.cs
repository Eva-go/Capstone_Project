using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ShopTimerController : MonoBehaviourPunCallbacks
{
    public RectTransform timerImage;
    public Image timerImageFill;

    private float totalTime = 180f; // 3분을 초 단위로 표시
    public float decreaseTime = 60f;
    private float currentTime;
    private float initialPosX = 234f;
    private float finalPosX = -234f;

    private bool isReady;

    public Text RoundText;
    public GameObject ReadyButton;

    private bool RBPressing;
    private float ReadyProgress;
    private float RBTime = 0.0f;

    public GameObject ReadyClockBar, CompleteClockBar;
    public Image ReadyClockBarImg;
    public RectTransform ReadyClockBar_End, ReadyClockBar_End_Mask;

    private bool[] roundEnd = new bool[GameValue.MaxPlayer + 1];

    void Start()
    {
        currentTime = totalTime;
        RoundText.text = GameValue.Round.ToString();
        isReady = false;
        ReadyButton.GetComponent<Button>().interactable = true;
        CompleteClockBar.SetActive(false);

        timerImage.localPosition = new Vector3(initialPosX, timerImage.localPosition.y, timerImage.localPosition.z);
        timerImageFill.fillAmount = 1;

        Cursor.lockState = CursorLockMode.Confined;
        PhotonNetwork.AutomaticallySyncScene = true;
        for (int i = 0; i < GameValue.MaxPlayer + 1; i++)
        {
            roundEnd[i] = false;
        }
    }

    void Update()
    {
        // 타이머 감소 및 이미지 위치 조정
        currentTime -= Time.deltaTime;
        float ratio = currentTime / totalTime;
        float posX = Mathf.Lerp(finalPosX, initialPosX, ratio);
        timerImage.localPosition = new Vector3(posX, timerImage.localPosition.y, timerImage.localPosition.z);
        timerImageFill.fillAmount = ratio;

        // 타이머 텍스트 업데이트
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        if (currentTime <= 0 && SceneManager.GetActiveScene().name == "Shop" && GameValue.RoundEnd)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("Map");
            }
            GameValue.RoundEnd = false;
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            photonView.RPC("RPC_DecreaseTime", RpcTarget.AllBuffered);
        }

        if (!isReady)
        {
            RBPressing = ReadyButton.GetComponent<ButtonPressed>().isButtonPressed;
            if (RBPressing)
            {
                if (ReadyProgress == 0) ReadyClockBar.SetActive(true);
                RBTime += Time.deltaTime;
                if (ReadyProgress < 100)
                {
                    ReadyProgress += RBTime / 100;
                }
                else
                {
                    ReadyProgress = 100;
                    isReady = true;
                    ReadyButton.GetComponent<Button>().interactable = false;
                    ReadyClockBar.SetActive(false);
                    CompleteClockBar.SetActive(true);
                    RoundEnd();
                }
            }
            else if (ReadyProgress < 100)
            {
                RBTime += Time.deltaTime;
                if (ReadyProgress > 0)
                {
                    ReadyProgress -= RBTime / 100;
                }
                else
                {
                    ReadyProgress = 0;
                    ReadyClockBar.SetActive(false);
                }
            }
            UIBarClockValue(ReadyProgress, ReadyClockBarImg, ReadyClockBar_End, ReadyClockBar_End_Mask);
        }
    }

    public void UIBarClockValue(float value, Image ClockBar, RectTransform ClockBar_End, RectTransform ClockBar_End_Mask)
    {
        float ClockProgress = (value / 100.0f);
        if (ClockProgress > 1) ClockProgress = 1;
        float angle = ClockProgress * 360;
        ClockBar.fillAmount = ClockProgress;
        ClockBar_End.localEulerAngles = new Vector3(0, 0, angle);
        ClockBar_End_Mask.localEulerAngles = new Vector3(0, 0, -angle);
    }

    [PunRPC]
    void RPC_DecreaseTime()
    {
        currentTime -= decreaseTime;
        if (currentTime < 0)
        {
            currentTime = 0;
        }
    }

    [PunRPC]
    void RPC_RoundEnd()
    {
        currentTime = 0;
        if (currentTime < 0)
        {
            currentTime = 0;
        }
    }

    [PunRPC]
    void RPC_Ready(int playerId)
    {
        roundEnd[playerId] = true;
        CheckAllPlayersReady();  // 모든 플레이어가 준비되었는지 확인
    }

    public void RoundEnd()
    {
        roundEnd[GameValue.PlayerID] = true;
        photonView.RPC("RPC_Ready", RpcTarget.OthersBuffered, GameValue.PlayerID);
        CheckAllPlayersReady();  // 자신의 상태도 확인
    }

    void CheckAllPlayersReady()
    {
        bool allPlayersReady = true;
        for (int i = 0; i < GameValue.MaxPlayer; i++)
        {
            if (!roundEnd[i])
            {
                allPlayersReady = false;
                break;
            }
        }

        if (allPlayersReady)
        {
            // 이 부분에서 마지막 플레이어가 확인한 후에만 RPC_RoundEnd를 호출합니다.
            if (PhotonNetwork.LocalPlayer.ActorNumber == GetLastReadyPlayer())
            {
                Debug.Log("모든 플레이어가 준비되었습니다. 작업을 수행합니다.");
                photonView.RPC("RPC_RoundEnd", RpcTarget.AllBuffered);
            }
        }
    }

    int GetLastReadyPlayer()
    {
        // 모든 플레이어 중에서 마지막으로 준비된 플레이어의 ID를 반환하는 메서드입니다.
        // 이 방법은 플레이어 ID를 사용하여 마지막 플레이어를 식별하는 데 사용됩니다.
        for (int i = GameValue.MaxPlayer; i >= 0; i--)
        {
            if (roundEnd[i])
            {
                return i;
            }
        }
        return -1; // 예외 처리: 준비된 플레이어가 없는 경우
    }
}