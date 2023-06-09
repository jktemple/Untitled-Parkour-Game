using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;

public class MenuMusic : MonoBehaviour
{
    public static MenuMusic Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update

    public EventReference musicEvent;
    public EventInstance menuMusic;
    
    void Start()
    {
        MenuMusic[] menuMusics = FindObjectsOfType<MenuMusic>();
        if (menuMusics.Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        Debug.Log(menuMusics.Length);
        DontDestroyOnLoad(this.gameObject);
        MenuMusic.Instance.menuMusic.getPlaybackState(out PLAYBACK_STATE pLAYBACK_STATE);
        Debug.Log("Playback state = " + pLAYBACK_STATE);
        
        
        menuMusic = RuntimeManager.CreateInstance(musicEvent);
        menuMusic.start();
    }

    public void Stop()
    {
        menuMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StartMusic()
    {
        menuMusic.start();
    }


    // Update is called once per frame


}
