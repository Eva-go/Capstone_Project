using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
public class PlayerNameItem : MonoBehaviourPunCallbacks
{
    [SerializeField] Text text;
    Player player; // ���� ����Ÿ��
    public void SetUp(Player _player)
    {
        player = _player;
        text.text = _player.NickName;
    }
}