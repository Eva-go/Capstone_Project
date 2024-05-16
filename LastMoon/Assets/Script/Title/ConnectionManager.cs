using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviourPunCallbacks
{ 

    //Fields
    public GameObject LoginMenu;
    public GameObject TitleMenu;
    [SerializeField]
    InputField nickNameInputField;
    [SerializeField]
    Text myNickName;

    public GameObject CreateRoomMenu;
    [SerializeField]
    InputField createRoomInputField;
    [SerializeField]
    Text txtPlayerCount;
    private const int minimumPlayer = 1;
    private const int maximumPlayer = 8;
    private int playerCount = 1;

    [Header("Room")]
    [SerializeField] Text roomName;
    [SerializeField] Text roomCurrentPlayer;
    [SerializeField] Text roomMaxPlayer;
    public GameObject RoomMenu;

    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;


    // 메뉴
    [SerializeField] GameObject FindRoomMenu;
    // 룸 리스트
    [Header("RoomList")]
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    private List<RoomListItem> lstRoom = new List<RoomListItem>();

    //Unity Events
    private void Start()
    {
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.ConnectUsingSettings(); //ServerID
    }

    //Photon Events

    //서버 접속 성공시 작동 콜백
    public override void OnConnectedToMaster()
    {
        LoginMenu.SetActive(true);//(서버->로비)
    }

    //접속 실패시 작동 콜백
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.ConnectUsingSettings();//다시한번 ServerID로 접근
    }
    //로비 접속 성공시 콜백
    public override void OnJoinedLobby()
    {
        myNickName.text = PhotonNetwork.NickName;
    }

    public override void OnJoinedRoom()
    {
        RoomMenu.SetActive(true);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        roomCurrentPlayer.text =
                        PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        roomMaxPlayer.text =
                        PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).
                           GetComponent<PlayerNameItem>().SetUp(players[i]);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            RoomListItem item = Instantiate(roomListItemPrefab,
                         roomListContent).GetComponent<RoomListItem>();
            if (item != null)
            {
                item.SetUp(roomInfo);
                lstRoom.Add(item);
            }
        }
    }

    // FindRoom 버튼을 누르면 TitleMenu --> FindRoomMenu
    public void Button_FindRoom()
    {
        TitleMenu.SetActive(false);
        FindRoomMenu.SetActive(true);
    }
    // Back 버튼을 누르면 FindRoomMenu --> TitleMenu
    public void Button_FindRoomBack()
    {
        FindRoomMenu.SetActive(false);
        TitleMenu.SetActive(true);
    }


    //Functions

    //Login & titeMeun
    public void CreateNickName()
    {
        if (string.IsNullOrEmpty(nickNameInputField.text)) return;
        PhotonNetwork.NickName = nickNameInputField.text;
        LoginMenu.SetActive(false);
        TitleMenu.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public void Button_CreateRoom()
    {
        TitleMenu.SetActive(false);
        CreateRoomMenu.SetActive(true);
    }

    //CreateRoomMenu
    public void IncreaseCount()
    {
        if (playerCount == maximumPlayer) return;
        playerCount++;
        txtPlayerCount.text = playerCount.ToString();
    }
    public void DecreaseCount()
    {
        if (playerCount == minimumPlayer) return;
        playerCount--;
        txtPlayerCount.text = playerCount.ToString();
    }

    //RoomMenu

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createRoomInputField.text)) return;
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = (byte)playerCount,
            IsVisible = true,
            IsOpen = true,
            CleanupCacheOnLeave = true
        };
        PhotonNetwork.CreateRoom(createRoomInputField.text, roomOptions);
        CreateRoomMenu.SetActive(false);
    }



}

