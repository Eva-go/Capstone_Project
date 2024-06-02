using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Seed : MonoBehaviour
{
    static public int seed1;
    static public int seed2;
    static public int setMaxtime;
    private NetworkManager networkManager;

    // Start is called before the first frame update
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        DontDestroyOnLoad(gameObject);
    }

    public void setTimer (int maxtime)
    {
        setMaxtime = maxtime;
        Debug.Log("½Ã°£: " + setMaxtime);
    }

    private void Update()
    {
    }
    public void seed(int a, int b)
    {
        seed1 = 1;
        seed2 = a;
        Debug.Log("seed: " + seed1);
        Debug.Log("seed: " + seed2);
    }
    
}
