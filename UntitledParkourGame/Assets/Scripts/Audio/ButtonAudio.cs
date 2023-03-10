using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ButtonAudio : MonoBehaviour
{
    
    [field: Header("Events")]
    [field: SerializeField] public EventReference fEvent { get; private set; }

    public bool PlayonAwake;
    public void PlayOneShot()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(fEvent, gameObject);
    }

    // Update is called once per frame
    private void Start()
    {
        if(PlayonAwake){
            PlayOneShot();
        }
    }
}
