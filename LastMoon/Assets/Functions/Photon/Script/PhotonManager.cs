using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    //버전 입력
    private readonly string version = "1.0f";
    //User ID
    private string userId = "사용자";
    [SerializeField]
    private GameObject wave;

    private bool master = false;



    void Awake()
    {
        // 같은 룸의 유저들에게 자동으로 씬을 로딩
        PhotonNetwork.AutomaticallySyncScene = true;
        //같은 버전의 유저끼리 접속 허용
        PhotonNetwork.GameVersion = version;
        //유저 아이디 할당
        PhotonNetwork.NickName = userId;
        //포톤 서버와 통신 횟수 설정 .초당 30회
        Debug.Log(PhotonNetwork.SendRate);
        //서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }
    
    void Start()
    {

    }

    //포톤 서버에 접속후 호출 되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby(); //로비입장
    }

    //로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRoom("Last Moon");
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRoom Filed {returnCode}:{message}");

        ////룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20; //최대 접속자 수 : 20 명
        ro.IsOpen = true; //룸의 오픈 여부
        ro.IsVisible = true; // 로비에서 룸 목록에 노출여부
        //
        ////룸 생성
        PhotonNetwork.CreateRoom("Last Moon", ro);
        master = true;
    }

    //랜덤한 룸 입장이 실패했을 경우 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Filed {returnCode}:{message}");

        ////룸의 속성 정의
        //RoomOptions ro = new RoomOptions();
        //ro.MaxPlayers = 20; //최대 접속자 수 : 20 명
        //ro.IsOpen = true; //룸의 오픈 여부
        //ro.IsVisible = true; // 로비에서 룸 목록에 노출여부
        //
        ////룸 생성
        //PhotonNetwork.CreateRoom("My Room", ro);
    }

    //룸 생성이 완료된후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom}");
    }

    //룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        //룸에 접속한 사용자 정보 확인
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName},{player.Value.ActorNumber}"); //사용자명,플레이어 번호
        }
        //캐릭터 출현 정보를 배열에 저장
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        //캐릭터를 생성
        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);
        //GameObject.Find("Loding").SetActive(false);
        //GameObject.Find("LodingCam").SetActive(false);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Out Room");
    }
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {

    }
}