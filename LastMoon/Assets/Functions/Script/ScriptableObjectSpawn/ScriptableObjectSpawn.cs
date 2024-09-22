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
    public Transform OutputTransform;
    public ScriptableObject_Item ItemType;
    public GameObject item;

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

    public void GiveItem()
    {
        GameObject nodeItem = Instantiate(item, OutputTransform.position, Quaternion.identity);
        NodeDestroy nodeDestroy = nodeItem.GetComponent<NodeDestroy>();
        nodeDestroy.Inv_Input = new Item { ItemType = ItemType, Count = 1 };
    }

    public void tick_ck(int ticksToConstruct)
    {
        tickMax = ticksToConstruct;
        TickTimer.OnTick += TimeTickSystem_OnTick;
        if(stop)
        {
            GiveItem();
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
