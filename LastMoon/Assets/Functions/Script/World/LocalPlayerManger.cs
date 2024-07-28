using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerManger : MonoBehaviour
{
    public static LocalPlayerManger Instance;
    private PlayerController localPlayer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterLocalPlayer(PlayerController player)
    {
        localPlayer = player;
    }

    public PlayerController GetLocalPlayer()
    {
        return localPlayer;
    }
}
