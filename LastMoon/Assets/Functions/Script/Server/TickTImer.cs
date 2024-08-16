using UnityEngine;
using Photon.Pun;

public class TickTimer : MonoBehaviourPun
{
    public float tickInterval = 10f; // 틱 간격 (초 단위)
    private double nextTickTime;

    void Start()
    {
        // PhotonNetwork.Time은 double 형식이므로 변수도 double로 설정합니다.
        nextTickTime = PhotonNetwork.Time + tickInterval;
    }

    void Update()
    {
        // 현재 네트워크 시간을 double 형식으로 가져옵니다.
        if (PhotonNetwork.Time >= nextTickTime)
        {
            ExecuteTick();
            nextTickTime += tickInterval; // 다음 틱 시간을 double 형식으로 설정합니다.
        }
    }

    void ExecuteTick()
    {
        // 이곳에 틱마다 실행할 코드를 넣습니다.
        Debug.Log("Tick executed at: " + PhotonNetwork.Time);
    }
}