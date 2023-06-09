using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]

    [Range(0, 1)]
    public float SFXVolume = 1;
    [Range(0, 1)]
    public float BGMVolume = 1;


    private Bus SFXBus;
    private Bus BGMBus;

    public static AudioManager instance { get; private set; }

    private void Awake(){
        if (instance != null){
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;
        SFXBus = RuntimeManager.GetBus("bus:/SFX");
        BGMBus = RuntimeManager.GetBus("bus:/BGMusic");
    }

    public EventInstance CreateInstance(EventReference eventReference){
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos){
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    private void Update(){
        SFXBus.setVolume(SFXVolume);
        BGMBus.setVolume(BGMVolume);
    }

    private void Start()
    {
        MenuMusic.Instance.Stop();
    }
}
