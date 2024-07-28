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

    private int seed1;
    private int seed2;
    private bool LocalClient = true;
    private GameValue gameValue;

    public bool isReady;

    public Text RoundText;
    public GameObject ReadyButton;

    private bool RBPressing;
    private float ReadyProgress;
    private float RBTime = 0.0f;

    public GameObject ReadyClockBar, CompleteClockBar;
    public Image ReadyClockBarImg;
    public RectTransform ReadyClockBar_End, ReadyClockBar_End_Mask;

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
        // 타이머가 음수가 되지 않도록 보정
        if (currentTime < 0)
        {
            currentTime = 0;
        }
    }
}