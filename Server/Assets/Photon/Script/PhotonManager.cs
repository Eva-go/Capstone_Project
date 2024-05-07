using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    
    //���� �Է�
    private readonly string version = "1.0f";
    //User ID
    private string userId = "����������";

    private int userNumber = 0;

    void Awake()
    {
        // ���� ���� �����鿡�� �ڵ����� ���� �̵�
        PhotonNetwork.AutomaticallySyncScene = false;
        //���� ������ �������� ���� ���
        PhotonNetwork.GameVersion = version;
        //���� ���̵� �Ҵ�
        PhotonNetwork.NickName = userId;
        //���� ������ ��� Ƚ�� ���� .�ʴ� 30ȸ
        Debug.Log(PhotonNetwork.SendRate);
        //���� ����
        PhotonNetwork.ConnectUsingSettings();
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
        PhotonNetwork.JoinRandomRoom(); //���� ��ġ����ŷ ��� ����
    }

    //������ �� ������ �������� ��� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Filed {returnCode}:{message}");

        //���� �Ӽ� ����
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20; //�ִ� ������ �� : 20 ��
        ro.IsOpen = true; //���� ���� ����
        ro.IsVisible = true; // �κ񿡼� �� ��Ͽ� ���⿩��

        //�� ����
<<<<<<< HEAD
        PhotonNetwork.CreateRoom("Last Moon", ro);
=======
        PhotonNetwork.CreateRoom("My Room", ro);
>>>>>>> young
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
        //Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        //int idx = Random.Range(1, points.Length);
        //ĳ���͸� ����
        //PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);
    
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    
    }
}
