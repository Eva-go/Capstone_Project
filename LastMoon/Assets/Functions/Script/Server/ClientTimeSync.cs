using UnityEngine;

public class ClientTimeSync : MonoBehaviour
{
    public static ClientTimeSync Instance;
    private float serverTime;
    private float timeOffset;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateServerTime(float time)
    {
        serverTime = time;
        timeOffset = Time.time - serverTime; // 서버와 로컬 시간 간의 차이를 계산
    }

    public float GetServerTime()
    {
        return serverTime + (Time.time - timeOffset); // 현재 서버 시간을 계산
    }

    public void Update()
    {
        Debug.Log("현재서버 시간" + GetServerTime());

    }
}