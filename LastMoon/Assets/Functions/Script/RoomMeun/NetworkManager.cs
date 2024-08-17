using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField m_inputField_Nickname; // 닉네임 입력받는 곳.
    public Dropdown m_dropdown_RoomMaxPlayers; // 최대 인원 몇 명까지 할지.
    public Dropdown m_dropdown_MaxTime; // 게임 시간은 몇 초로 정할 건지.
    public Button m_button_RadomMatching;
    public Dropdown dropdown_MaxRound;


    public GameObject m_panel_Loading; // 로딩 UI.
    public Text m_text_CurrentPlayerCount; // 로딩 UI 중에서 현재 인원 수를 나타냄.
    public Button mbutton_Start;


    Player[] sortedPlayers;
    public static int PlayerID = -1; //플레이어 번호

    // 게임 시간
    private const byte StartTimeEventCode = 1;


    //seed값
    private int seed1;
    private int seed2;
    private GameValue gameValue;
    private bool LocalClient = true;

    private int maxTime;


    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        // 마스터 클라이언트는 PhotonNetwork.LoadLevel()를 호출할 수 있고, 모든 연결된 플레이어는 자동적으로 동일한 레벨을 로드한다.
        PhotonNetwork.AutomaticallySyncScene = true;
        gameValue = FindObjectOfType<GameValue>();
        m_panel_Loading.SetActive(false);
        //mbutton_Start.GetComponent<Button>().interactable = false;
    }

    void Start()
    {
        Screen.SetResolution(Screen.width, Screen.width * 9 / 16, false);
        print("서버 연결 시도.");
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

        print($"{nick} 랜덤 매칭 시작.");
        PhotonNetwork.LocalPlayer.NickName = nick; // 현재 플레이어 닉네임 설정하기.
        gameValue.NickName(nick);
        // UI에서 값 얻어오기.
        byte maxPlayers = byte.Parse(m_dropdown_RoomMaxPlayers.options[m_dropdown_RoomMaxPlayers.value].text); // 드롭다운에서 값 얻어오기.
        byte maxRound = byte.Parse(dropdown_MaxRound.options[dropdown_MaxRound.value].text);
        maxTime = int.Parse(m_dropdown_MaxTime.options[m_dropdown_MaxTime.value].text);

        GameValue.MaxRound = maxRound;
        GameValue.MaxUser = maxPlayers;
        
        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = maxPlayers; // 인원 지정.
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } }; // 게임 시간 지정.
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "maxTime" }; // 여기에 키 값을 등록해야, 필터링이 가능하다.

        // 방 참가를 시도하고, 실패하면 생성해서 참가함.
        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } }, expectedMaxPlayers: maxPlayers, // 참가할 때의 기준.
            roomOptions: roomOptions // 생성할 때의 기준.
        );
    }

    public void CancelMatching()
    {
        print("매칭 취소.");
        m_panel_Loading.SetActive(false);

        print("방 떠남.");
        PhotonNetwork.LeaveRoom();
    }

    private void UpdatePlayerCounts()
    {
        if (m_text_CurrentPlayerCount != null)
        {
            m_text_CurrentPlayerCount.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
    }

    #region 포톤 콜백 함수

    public override void OnConnectedToMaster()
    {
        print("서버 접속 완료.");
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

        print("방 참가 완료.");

        Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}은 인원수 {PhotonNetwork.CurrentRoom.MaxPlayers} 매칭 기다리는 중. "+PlayerID+"플레이어 번호");
        UpdatePlayerCounts();

        m_panel_Loading.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트에서만 시드 값을 생성합니다.
            seed1 = UnityEngine.Random.Range(0, 1000);
            seed2 = UnityEngine.Random.Range(0, 1000);
            gameValue.seed(seed1, seed2);
            LocalClient = false;
        }
        photonView.RPC("SendSeedsToClients", RpcTarget.OthersBuffered, seed1, seed2);

    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"플레이어 {newPlayer.NickName} 방 참가.");
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

            // 목표 인원 수 채웠으면, 맵 이동을 한다. 권한은 마스터 클라이언트만.
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
                Debug.Log("게임시간 :" + maxTime);

                PhotonNetwork.LoadLevel("Map-spwanFix");
            }
        }
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"플레이어 {otherPlayer.NickName} 방 나감.");
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