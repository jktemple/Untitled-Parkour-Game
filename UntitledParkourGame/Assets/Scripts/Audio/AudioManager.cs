using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake(){
        if (instance != null){
            Debug.LogError("Found more than one Aduio Manager in the scene.");
        }
    }

    public EventInstance CreateInstance(EventReference eventReference){
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }

}
