using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class ShopTimerController : MonoBehaviourPunCallbacks
{
    public RectTransform timerImage;
    private float totalTime = 180f; // 3분을 초 단위로 표시
    public float decreaseTime = 60f;
    private float currentTime;
    private float initialPosX = 234f;
    private float finalPosX = -234f;

    private int seed1;
    private int seed2;
    private bool LocalClient = true;
    private GameValue gameValue;
    void Start()
    {
        currentTime = totalTime;
        timerImage.localPosition = new Vector3(initialPosX, timerImage.localPosition.y, timerImage.localPosition.z);
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