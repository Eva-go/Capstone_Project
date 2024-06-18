using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class GameTimer : MonoBehaviourPunCallbacks
{
    public RectTransform timerImage;
    private float totalTime;
    public float decreaseTime = 180f; // f1 ���� �� ���ҽ�ų �ð� (3��)

    private float currentTime;
    private float initialPosX = 234f;
    private float finalPosX = -234f;


    void Start()
    {
        GameValue.RoundEnd = false;
        totalTime = GameValue.setMaxtime * 60;
        currentTime = totalTime;
        timerImage.localPosition = new Vector3(initialPosX, timerImage.localPosition.y, timerImage.localPosition.z);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        // Ÿ�̸� ���� �� �̹��� ��ġ ����
        if (SceneManager.GetActiveScene().name == "Map")
        {
            currentTime -= Time.deltaTime;
            float ratio = currentTime / totalTime;
            float posX = Mathf.Lerp(finalPosX, initialPosX, ratio);
            timerImage.localPosition = new Vector3(posX, timerImage.localPosition.y, timerImage.localPosition.z);

            // Ÿ�̸� �ؽ�Ʈ ������Ʈ
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
        }
        // f1 Ű�� ������ �ð� ����
        if (Input.GetKeyDown(KeyCode.F1))
        {
            photonView.RPC("RPC_DecreaseTime", RpcTarget.AllBuffered);
        }
        if (currentTime <= 0 && SceneManager.GetActiveScene().name == "Map" && !GameValue.RoundEnd)
        {
            if (GameValue.Round < 3)
            {
                GameValue.Round += 1;
                Debug.Log("����" + GameValue.Round);
                GameValue.RoundEnd = true;
                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.LoadLevel("Shop");
                totalTime = GameValue.setMaxtime * 60;
            }
            else
            {
                //GameValue.RoundEnd = true;
                //if (PhotonNetwork.IsMasterClient)
                //    PhotonNetwork.LoadLevel("Ending");
            }
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