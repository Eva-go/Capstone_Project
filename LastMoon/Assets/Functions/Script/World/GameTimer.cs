using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviourPunCallbacks
{
    public RectTransform timerImage;
    public Image timerImageFill_LT;
    public Image timerlmageFill_HT;

    public GameObject Wave_Time_Active_LT;
    public GameObject Wave_Time_Active_HT;

    private bool switchingTime = false;

    private float totalTime;
    public float decreaseTime = 180f; // f1 ���� �� ���ҽ�ų �ð� (3��)

    public float currentTime;
    private float initialPosX = 234f;
    private float finalPosX = -234f;

    private Image tempImage; // Temporary image for swapping sprites

    private float TideChangeProgress = 0;


    void Start()
    {
        Wave_Time_Active_LT.SetActive(true);
        switchingTime = false;
        GameValue.RoundEnd = false;
        totalTime = 0.5f * (GameValue.Round - 1) * 60f / GameValue.MaxRound * 60f + (GameValue.setMaxtime * 60f);
        currentTime = totalTime;
        timerImage.localPosition = new Vector3(initialPosX, timerImage.localPosition.y, timerImage.localPosition.z);
        timerImageFill_LT.fillAmount = 1;

        PhotonNetwork.AutomaticallySyncScene = true;
        GameValue.WaveTimer = currentTime;
        GameValue.WaveTimerMax = totalTime;
        Debug.Log("��Ż Ÿ�̸�: " + totalTime + " ���� ����: " + GameValue.Round + " �ִ� ����: " + GameValue.MaxRound + " ���� �ð�: " + GameValue.setMaxtime);

        GameValue.LowTide = true;
        GameValue.TideCycle = 1;
        GameValue.TideChange = false;
        GameValue.TideChangeProgress = 0;
        
        GameValue.DNCycle = 0;

        TideChangeProgress = 0;
    }

    void Update()
    {
        // Ÿ�̸� ���� �� �̹��� ��ġ ����
        if (SceneManager.GetActiveScene().name == "Map")
        {

            GameValue.DNCycle += Time.deltaTime;

            if (GameValue.DNCycle > 720.0f)
            {
                GameValue.DNCycle = 0.0f;
            }

            if (!GameValue.TideChange)
            {
                currentTime -= Time.deltaTime;

                // Ÿ�̸Ӱ� 0 ���Ϸ� �������� �̹��� ���� ó��
                if (currentTime < 0)
                {
                    HandleTimeChange();
                }

                // f1 Ű�� ������ �ð� ����
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    photonView.RPC("RPC_DecreaseTime", RpcTarget.AllBuffered);
                }
            }
            else
            {
                TideChangeProgress -= Time.deltaTime;
                if (TideChangeProgress < 0)
                {
                    TideChangeProgress = 0;
                    GameValue.TideChange = false;
                }

                GameValue.TideChangeProgress = TideChangeProgress;
            }
            GameValue.WaveTimer = currentTime;
            GameValue.WaveTimerMax = totalTime;
            float ratio = currentTime / totalTime;
            float posX = Mathf.Lerp(finalPosX, initialPosX, ratio);
            timerImage.localPosition = new Vector3(posX, timerImage.localPosition.y, timerImage.localPosition.z);
            timerImageFill_LT.fillAmount = ratio;
        }
    }

    [PunRPC]
    void RPC_All_Inside_Time()
    {
        GameValue.insideUser++;
        if (GameValue.insideUser >= GameValue.MaxUser)
        {
            currentTime = 0;
            HandleTimeChange(); // Ensure time change handling if insideUser reaches max
        }
    }

    [PunRPC]
    void RPC_DecreaseTime()
    {
        currentTime -= decreaseTime;

        // Ÿ�̸Ӱ� 0 ���Ϸ� �������� �̹��� ���� ó��
        if (currentTime < 0)
        {
            HandleTimeChange();
        }
    }

    void HandleTimeChange()
    {
        GameValue.LowTide = !GameValue.LowTide;
        if (GameValue.LowTide) GameValue.TideCycle++;

        if (!GameValue.TideChange)
        {
            GameValue.TideChange = true;
            TideChangeProgress = 5.0f;
        }

        if (tempImage == null)
        {
            tempImage = new GameObject("TempImage").AddComponent<Image>();
        }
        switchingTime = !switchingTime;
        currentTime = totalTime;
        if (switchingTime)
        {
            // Swap sprites
            tempImage.sprite = timerImageFill_LT.sprite;
            timerImageFill_LT.sprite = timerlmageFill_HT.sprite;
            timerlmageFill_HT.sprite = tempImage.sprite;
            Wave_Time_Active_LT.SetActive(false);
            Wave_Time_Active_HT.SetActive(true);
        }
        else
        {
            // Swap sprites
            tempImage.sprite = timerlmageFill_HT.sprite;
            timerlmageFill_HT.sprite = timerImageFill_LT.sprite;
            timerImageFill_LT.sprite = tempImage.sprite;
            Wave_Time_Active_LT.SetActive(true);
            Wave_Time_Active_HT.SetActive(false);
        }
    }
}