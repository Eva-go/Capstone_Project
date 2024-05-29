using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Seed : MonoBehaviour
{
    static public int seed1;
    static public int seed2;
    private NetworkManager networkManager;
    // Start is called before the first frame update
    private bool seedkey;
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
    }
    public void seed(int a, int b)
    {
        seed1 = a;
        seed2 = b;
        Debug.Log("seed: " + seed1);
        Debug.Log("seed: " + seed2);
    }
    
}
