using System.Collections;
using UnityEngine;
using Photon.Pun;


public class ScriptableObjectSpawn : MonoBehaviour
{
    
    //tick  관련 변수
    public int tick;
    public int tickMax;
    public bool stop;

    //Spawn
    public GameObject item;
    public Transform OutputTransform;


    // Start is called before the first frame update
    void Start()
    {
        stop = false;
    }

    // Update is called once per frame
    void Update()
    {
        tick_ck(500);
    }
    public void tick_ck(int ticksToConstruct)
    {
        tickMax = ticksToConstruct;
        TickTimer.OnTick += TimeTickSystem_OnTick;
        if(stop)
        {
            Debug.Log("시간" + PhotonNetwork.Time);
            GameObject nodeItem = Instantiate(item, OutputTransform);
            NodeDestroy nodeDestroy = nodeItem.GetComponent<NodeDestroy>();
            stop = false;
        }
    }
    private void TimeTickSystem_OnTick(object sender, TickTimer.OnTickEventArgs e)
    {
        tick = e.tick % tickMax;
        if (tick >= tickMax - 1)
        {
            stop = true;
        }
    }
}
