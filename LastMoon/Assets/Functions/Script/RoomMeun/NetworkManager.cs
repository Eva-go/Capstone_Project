using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField m_inputField_Nickname; // �г��� �Է¹޴� ��.
    public Dropdown m_dropdown_RoomMaxPlayers; // �ִ� �ο� �� ����� ����.
    public Dropdown m_dropdown_MaxTime; // ���� �ð��� �� �ʷ� ���� ����.
    public Button m_button_RadomMatching;
    public Dropdown dropdown_MaxRound;


    public GameObject m_panel_Loading; // �ε� UI.
    public Text m_text_CurrentPlayerCount; // �ε� UI �߿��� ���� �ο� ���� ��Ÿ��.
    public Button mbutton_Start;


    Player[] sortedPlayers;
    public static int PlayerID = -1; //�÷��̾� ��ȣ

    // ���� �ð�
    private const byte StartTimeEventCode = 1;


    //seed��
    private int seed1;
    private int seed2;
    private GameValue gameValue;
    private bool LocalClient = true;

    private int maxTime;


    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        // ������ Ŭ���̾�Ʈ�� PhotonNetwork.LoadLevel()�� ȣ���� �� �ְ�, ��� ����� �÷��̾�� �ڵ������� ������ ������ �ε��Ѵ�.
        PhotonNetwork.AutomaticallySyncScene = true;
        gameValue = FindObjectOfType<GameValue>();
        m_panel_Loading.SetActive(false);
        //mbutton_Start.GetComponent<Button>().interactable = false;
    }

    void Start()
    {
        Screen.SetResolution(Screen.width, Screen.width * 9 / 16, false);
        print("���� ���� �õ�.");
        PhotonNetwork.ConnectUsingSettings();
    }
    public int GetSeed1()
    {
        return seed1;
    }

    public int GetSeed2()
    {
        return seed2;
    }
    public void JoinRandomOrCreateRoom()
    {
        string nick = m_inputField_Nickname.text;

        print($"{nick} ���� ��Ī ����.");
        PhotonNetwork.LocalPlayer.NickName = nick; // ���� �÷��̾� �г��� �����ϱ�.
        gameValue.NickName(nick);
        // UI���� �� ������.
        byte maxPlayers = byte.Parse(m_dropdown_RoomMaxPlayers.options[m_dropdown_RoomMaxPlayers.value].text); // ��Ӵٿ�� �� ������.
        byte maxRound = byte.Parse(dropdown_MaxRound.options[dropdown_MaxRound.value].text);
        maxTime = int.Parse(m_dropdown_MaxTime.options[m_dropdown_MaxTime.value].text);

        GameValue.MaxRound = maxRound;
        GameValue.MaxUser = maxPlayers;
        
        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = maxPlayers; // �ο� ����.
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } }; // ���� �ð� ����.
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "maxTime" }; // ���⿡ Ű ���� ����ؾ�, ���͸��� �����ϴ�.

        // �� ������ �õ��ϰ�, �����ϸ� �����ؼ� ������.
        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } }, expectedMaxPlayers: maxPlayers, // ������ ���� ����.
            roomOptions: roomOptions // ������ ���� ����.
        );
    }

    public void CancelMatching()
    {
        print("��Ī ���.");
        m_panel_Loading.SetActive(false);

        print("�� ����.");
        PhotonNetwork.LeaveRoom();
    }

    private void UpdatePlayerCounts()
    {
        if (m_text_CurrentPlayerCount != null)
        {
            m_text_CurrentPlayerCount.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
    }

    #region ���� �ݹ� �Լ�

    public override void OnConnectedToMaster()
    {
        print("���� ���� �Ϸ�.");
        m_button_RadomMatching.interactable = true;
    }

    public override void OnJoinedRoom()
    {

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        sortedPlayers = PhotonNetwork.PlayerList;

        for (int i = 0; i < sortedPlayers.Length; i += 1)
        {
            if (sortedPlayers[i].ActorNumber == actorNumber)
            {
                PlayerID = i;
                break;
            }
        }

        print("�� ���� �Ϸ�.");

        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}�� �ο��� {PhotonNetwork.CurrentRoom.MaxPlayers} ��Ī ��ٸ��� ��. "+PlayerID+"�÷��̾� ��ȣ");
        UpdatePlayerCounts();

        m_panel_Loading.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        {
            // ������ Ŭ���̾�Ʈ������ �õ� ���� �����մϴ�.
            seed1 = UnityEngine.Random.Range(0, 1000);
            seed2 = UnityEngine.Random.Range(0, 1000);
            gameValue.seed(seed1, seed2);
            LocalClient = false;
        }
        photonView.RPC("SendSeedsToClients", RpcTarget.OthersBuffered, seed1, seed2);

    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"�÷��̾� {newPlayer.NickName} �� ����.");
        UpdatePlayerCounts();
        if (PhotonNetwork.IsMasterClient)
        {
            mbutton_Start.GetComponent<Button>().interactable = true;
            gameValue.seed(seed1, seed2);
            LocalClient = false;
        }


    }
    [PunRPC]
    private void SendSeedsToClients(int seed1, int seed2)
    {
        this.seed1 = seed1;
        this.seed2 = seed2;
        if (LocalClient)
        {
            gameValue.seed(seed1, seed2);
            gameValue.setTimer(maxTime);
        }

    }
    public void Button_Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            // ��ǥ �ο� �� ä������, �� �̵��� �Ѵ�. ������ ������ Ŭ���̾�Ʈ��.
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                int maxTime = (int)PhotonNetwork.CurrentRoom.CustomProperties["maxTime"];
                double startTime = PhotonNetwork.Time;

                // Raise event to sync start time with all clients
                object[] content = new object[] { startTime };
                PhotonNetwork.RaiseEvent(StartTimeEventCode, content, RaiseEventOptions.Default, SendOptions.SendReliable);

                // Set the start time in room properties
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "startTime", startTime } });
                gameValue.setTimer(maxTime);
                Debug.Log("���ӽð� :" + maxTime);

                PhotonNetwork.LoadLevel("Map-spwanFix");
            }
        }
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"�÷��̾� {otherPlayer.NickName} �� ����.");
        UpdatePlayerCounts();
        PhotonNetwork.DestroyPlayerObjects(otherPlayer);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == StartTimeEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            double startTime = (double)data[0];
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "startTime", startTime } });
        }
    }

    #endregion
}