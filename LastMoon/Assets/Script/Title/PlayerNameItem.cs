using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
public class PlayerNameItem : MonoBehaviourPunCallbacks
{
    [SerializeField] Text text;
    Player player; // 포톤 리얼타일
    public void SetUp(Player _player)
    {
        player = _player;
        text.text = _player.NickName;
    }
}