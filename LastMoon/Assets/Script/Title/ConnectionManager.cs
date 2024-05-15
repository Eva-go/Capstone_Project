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

    //Unity Events
    private void Start()
    {
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.ConnectUsingSettings(); //ServerID
    }

    //Photon Events

    //���� ���� ������ �۵� �ݹ�
    public override void OnConnectedToMaster()
    {
        LoginMenu.SetActive(true);//(����->�κ�)
    }

    //���� ���н� �۵� �ݹ�
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.ConnectUsingSettings();//�ٽ��ѹ� ServerID�� ����
    }
    //�κ� ���� ������ �ݹ�
    public override void OnJoinedLobby()
    {
        myNickName.text = PhotonNetwork.NickName;
    }

    //Functions
    public void CreateNickName()
    {
        if (string.IsNullOrEmpty(nickNameInputField.text)) return;
        PhotonNetwork.NickName = nickNameInputField.text;
        LoginMenu.SetActive(false);
        TitleMenu.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

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
    public void CreateRoom()
    {
        TitleMenu.SetActive(false);
        CreateRoomMenu.SetActive(true);
    }

}

