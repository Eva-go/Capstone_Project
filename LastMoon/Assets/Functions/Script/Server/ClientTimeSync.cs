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
        timeOffset = Time.time - serverTime; // ������ ���� �ð� ���� ���̸� ���
    }

    public float GetServerTime()
    {
        return serverTime + (Time.time - timeOffset); // ���� ���� �ð��� ���
    }

    public void Update()
    {
        Debug.Log("���缭�� �ð�" + GetServerTime());

    }
}