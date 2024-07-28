using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class ShopTimerController : MonoBehaviourPunCallbacks
{
    public RectTransform timerImage;
    private float totalTime = 180f; // 3���� �� ������ ǥ��
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
        // Ÿ�̸� ���� �� �̹��� ��ġ ����
        currentTime -= Time.deltaTime;
        float ratio = currentTime / totalTime;
        float posX = Mathf.Lerp(finalPosX, initialPosX, ratio);
        timerImage.localPosition = new Vector3(posX, timerImage.localPosition.y, timerImage.localPosition.z);

        // Ÿ�̸� �ؽ�Ʈ ������Ʈ
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
        // Ÿ�̸Ӱ� ������ ���� �ʵ��� ����
        if (currentTime < 0)
        {
            currentTime = 0;
        }
    }
}