using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RankingUpdate : MonoBehaviourPunCallbacks
{
    public GameObject[] Ranking;
    public Text[] NickName;
    public Text[] TotalMoney;

    void Start()
    {
        for (int i = 0; i < Ranking.Length; i++)
        {
            Ranking[i].SetActive(false);
        }

        for (int i = 0; i < GameValue.MaxUser; i++)
        {
            Ranking[i].SetActive(true);
        }
    }

    void Update()
    {
        UpdateRanking();
    }

    void UpdateRanking()
    {
        List<PlayerScoreData> playerScores = new List<PlayerScoreData>();

        // ��� �÷��̾��� Ŀ���� �Ӽ��� ������
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("score"))
            {
                int playerScore = (int)player.CustomProperties["score"];
                playerScores.Add(new PlayerScoreData(player.NickName, playerScore));
            }
        }

        // ���� ������������ ����
        playerScores.Sort((a, b) => b.score.CompareTo(a.score));

        // UI ������Ʈ
        for (int i = 0; i < playerScores.Count; i++)
        {
            if (i < NickName.Length && i < TotalMoney.Length)
            {
                NickName[i].text = playerScores[i].playerName;
                TotalMoney[i].text = playerScores[i].score.ToString();
            }
        }
    }

    private class PlayerScoreData
    {
        public string playerName;
        public int score;

        public PlayerScoreData(string playerName, int score)
        {
            this.playerName = playerName;
            this.score = score;
        }
    }
}