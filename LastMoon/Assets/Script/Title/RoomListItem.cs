using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
public class RoomListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] Text text_Room;
    [SerializeField] Text text_PlayerMaxCount;
    [SerializeField] Text text_PlayerCount;
    RoomInfo roominfo;
    private int playerMaxCount;
    private int playerCount;

    public void SetUp(RoomInfo _roominfo)
    {
        roominfo = _roominfo;
        text_Room.text = _roominfo.Name;
        playerMaxCount = _roominfo.PlayerCount;
        playerCount = _roominfo.MaxPlayers;
    }
}