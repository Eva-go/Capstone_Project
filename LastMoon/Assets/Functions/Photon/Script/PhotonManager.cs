using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    //���� �Է�
    private readonly string version = "1.0f";
    //User ID
    private string userId = "�����";
    [SerializeField]
    private GameObject wave;

    private bool master = false;



    void Awake()
    {
        // ���� ���� �����鿡�� �ڵ����� ���� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;
        //���� ������ �������� ���� ���
        PhotonNetwork.GameVersion = version;
        //���� ���̵� �Ҵ�
        PhotonNetwork.NickName = userId;
        //���� ������ ��� Ƚ�� ���� .�ʴ� 30ȸ
        Debug.Log(PhotonNetwork.SendRate);
        //���� ����
        PhotonNetwork.ConnectUsingSettings();
    }
    
    void Start()
    {

    }

    //���� ������ ������ ȣ�� �Ǵ� �ݹ� �Լ�
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby(); //�κ�����
    }

    //�κ� ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRoom("Last Moon");
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRoom Filed {returnCode}:{message}");

        ////���� �Ӽ� ����
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20; //�ִ� ������ �� : 20 ��
        ro.IsOpen = true; //���� ���� ����
        ro.IsVisible = true; // �κ񿡼� �� ��Ͽ� ���⿩��
        //
        ////�� ����
        PhotonNetwork.CreateRoom("Last Moon", ro);
        master = true;
    }

    //������ �� ������ �������� ��� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Filed {returnCode}:{message}");

        ////���� �Ӽ� ����
        //RoomOptions ro = new RoomOptions();
        //ro.MaxPlayers = 20; //�ִ� ������ �� : 20 ��
        //ro.IsOpen = true; //���� ���� ����
        //ro.IsVisible = true; // �κ񿡼� �� ��Ͽ� ���⿩��
        //
        ////�� ����
        //PhotonNetwork.CreateRoom("My Room", ro);
    }

    //�� ������ �Ϸ���� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom}");
    }

    //�뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        //�뿡 ������ ����� ���� Ȯ��
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName},{player.Value.ActorNumber}"); //����ڸ�,�÷��̾� ��ȣ
        }
        //ĳ���� ���� ������ �迭�� ����
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        //ĳ���͸� ����
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