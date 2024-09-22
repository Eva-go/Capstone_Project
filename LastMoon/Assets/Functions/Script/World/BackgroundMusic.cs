using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource sfx_BGMusic, sfx_EventMusic;
    public AudioClip sfx_Music_Loobbi, sfx_Music_Main, sfx_Music_Shop, sfx_Music_Ending
        , sfx_Music_Round_Start, sfx_Music_Round_End, sfx_Music_EndGame, sfx_Music_Victory;

    public bool StopMusic = false;

    private bool PlayEM;
    private int CurruntSceneID = 0;
    private float BGMFadeout = 1;


    private void Awake()
    {
        var obj = FindObjectsOfType<BackgroundMusic>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += SceneIDUpdate;
    }

    void Update()
    {
        if (BGMFadeout != 1)
        {
            if (BGMFadeout > 0)
            {
                BGMFadeout -= 0.01f;
                sfx_BGMusic.volume = BGMFadeout;
            }
            else
            {
                sfx_BGMusic.Stop();
                BGMClipUpdater();
                BGMFadeout = 1;
                sfx_BGMusic.volume = 1f;
                sfx_BGMusic.Play();
                sfx_BGMusic.mute = true;
            }
        }


        if (PlayEM)
        {
            if (!sfx_EventMusic.isPlaying)
            {
                sfx_EventMusic.Stop();
            }
            EMClipUpdater();
            BGMFadeout = 0.75f;
            sfx_EventMusic.Play();
            PlayEM = false;
        }
        else if (!sfx_EventMusic.isPlaying)
        {
            if (sfx_BGMusic.mute)
            {
                sfx_BGMusic.mute = false;
            }
            else if (BGMFadeout != 1)
            {
                sfx_BGMusic.Stop();
                BGMClipUpdater();
                BGMFadeout = 1;
                sfx_BGMusic.volume = 1f;
                sfx_BGMusic.Play();
            }
        }
        if (CurruntSceneID == -1)
        {
            if (sfx_BGMusic != null && sfx_BGMusic.isPlaying)
            {
                sfx_BGMusic.Stop();
            }
        }
    }

    void SceneIDUpdate(Scene scene, LoadSceneMode loadSceneMode)
    {
        Scene SceneDetect = SceneManager.GetActiveScene();
        string SceneName = SceneDetect.name;
        if (SceneName == "RoomMeun")
        {
            if (CurruntSceneID != 0)
            {
                CurruntSceneID = 0;
            }
        }
        else if (SceneName == "Map")
        {
            if (CurruntSceneID != 1)
            {
                PlayEM = true;
                CurruntSceneID = 1;
            }
        }
        else if (SceneName == "Shop")
        {
            if (CurruntSceneID != 2)
            {
                PlayEM = true;
                CurruntSceneID = 2;
            }
        }
        else if (SceneName == "GameEnding")
        {
            if (CurruntSceneID != 3)
            {
                PlayEM = true;
                CurruntSceneID = 3;
            }
        }
        else
        {
            if (CurruntSceneID != -1)
            {
                PlayEM = false;
                CurruntSceneID = -1;
            }
        }
    }

    void BGMClipUpdater()
    {
        switch (CurruntSceneID)
        {
            case 0:
                sfx_BGMusic.clip = sfx_Music_Loobbi;
                break;
            case 1:
                sfx_BGMusic.clip = sfx_Music_Main;
                break;
            case 2:
                sfx_BGMusic.clip = sfx_Music_Shop;
                break;
            case 3:
                sfx_BGMusic.clip = sfx_Music_Ending;
                break;
        }
    }

    void EMClipUpdater()
    {
        switch (CurruntSceneID)
        {
            case 0:
                sfx_EventMusic.clip = sfx_Music_Round_Start;
                break;
            case 1:
                sfx_EventMusic.clip = sfx_Music_Round_Start;
                break;
            case 2:
                sfx_EventMusic.clip = sfx_Music_Round_End;
                break;
            case 3:
                sfx_EventMusic.clip = sfx_Music_Victory;
                break;
        }
    }
}