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
        DontDestroyOnLoad(this.gameObject);
        menuMusic = RuntimeManager.CreateInstance(musicEvent);
        menuMusic.start();
    }

    public void Stop()
    {
        menuMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    // Update is called once per frame


}
