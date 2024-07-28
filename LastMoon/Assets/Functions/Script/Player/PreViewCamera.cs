using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PreViewCamera : MonoBehaviour
{
    public GameObject PreViewCamer;
    public PhotonView pv;
    private int currentPlayerIndex = 0;
    private GameObject[] playersToView;

    private void Start()
    {
        PreViewCamer.SetActive(false);
        pv = GetComponent<PhotonView>();

        // Assuming you have a list of players to observe
        playersToView = GameObject.FindGameObjectsWithTag("Player");
    }

    private void Update()
    {
        if (!pv.IsMine && PlayerController.PreViewCam)
        {
            PreViewCamer.SetActive(true);

            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                Debug.Log("Player: " + player.NickName + ", ID: " + player.ActorNumber);
            }
            // Detect mouse scroll wheel movement
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (playersToView.Length == 0)
                return; // No players to view, exit Update()

            if (scroll > 0f) // Scroll up
            {
                // Switch to the next player
                currentPlayerIndex = (currentPlayerIndex + 1) % playersToView.Length;
            }
            else if (scroll < 0f) // Scroll down
            {
                // Switch to the previous player
                currentPlayerIndex = (currentPlayerIndex - 1 + playersToView.Length) % playersToView.Length;
            }

            // Activate the camera for the current player
            if (currentPlayerIndex >= 0 && currentPlayerIndex < playersToView.Length)
            {
                //ActivatePlayerCamera(playersToView[currentPlayerIndex]);
            }
        }
        else
        {
            // If it's the player's own camera, deactivate the preview camera
            PreViewCamer.SetActive(false);
        }
    }

    // Activate camera for the specified player
    private void ActivatePlayerCamera(GameObject player)
    {
        // Deactivate all player cameras
        foreach (var p in playersToView)
        {
            if (p != player)
                p.GetComponent<Camera>().enabled = false;
        }

        // Activate camera for the specified player
        player.GetComponent<Camera>().enabled = true;
    }
}