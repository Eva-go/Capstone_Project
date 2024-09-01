using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Photon.Pun;

public class EscapeMenu : MonoBehaviour
{
    private bool inEscapeMenu;
    private float Volume_Master;
    private float Volume_Music;
    private float Volume_GamePlay;

    public GameObject Menu;
    public bool Ingame;

    [SerializeField] public Slider VolumeSlider_Master;
    [SerializeField] public Slider VolumeSlider_GamePlay;
    [SerializeField] public Slider VolumeSlider_Music;

    [SerializeField] public AudioMixer AudioMixerController;

    void Start()
    {
        inEscapeMenu = false;
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inEscapeMenu)
            {
                EscapeMenuClose();
            }
            else
            {
                EscapeMenuOpen();
            }
        }
    }

    public void VolumeChange()
    {
        Volume_Master = Mathf.Log10(VolumeSlider_Master.value) * 20;
        AudioMixerController.SetFloat("Master", Volume_Master);
        Volume_GamePlay = Mathf.Log10(VolumeSlider_GamePlay.value) * 20;
        AudioMixerController.SetFloat("GamePlay", Volume_GamePlay);
        Volume_Music = Mathf.Log10(VolumeSlider_Music.value) * 20;
        AudioMixerController.SetFloat("Music", Volume_Music);
    }

    public void EscapeMenuOpen()
    {
        Cursor.lockState = CursorLockMode.None;
        inEscapeMenu = true;
        Menu.SetActive(true);
    }

    public void EscapeMenuClose()
    {
        if (Ingame) Cursor.lockState = CursorLockMode.Locked;
        inEscapeMenu = false;
        Menu.SetActive(false);
    }


    public void EscapeMenuQuit()
    {
        // Application.Quit();
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); // 서버와의 연결을 끊고 씬 전환
        }
        GameValue.exit = true;
        PhotonNetwork.LoadLevel("RoomMeun");
    }
}
