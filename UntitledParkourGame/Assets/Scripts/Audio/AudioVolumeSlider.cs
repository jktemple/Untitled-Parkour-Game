using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/* This Script Modified from a Youtube Video
 * https://www.youtube.com/watch?v=rcBHIOjZDpk
 * 
 */ 


public class AudioVolumeSlider : MonoBehaviour
{
    private enum VolumeType{
        SFX,

        BGM
    }

    [Header ("Type")]
    [SerializeField] private VolumeType volumeType;

    private Slider volumeSlider;

    private void Awake(){
        volumeSlider = this.GetComponentInChildren<Slider>();
    }

    private void Update(){
        switch (volumeType){
            case VolumeType.SFX:
                volumeSlider.value = AudioManager.instance.SFXVolume;
                break;
            case VolumeType.BGM:
                volumeSlider.value = AudioManager.instance.BGMVolume;
                break;
            default:
                Debug.LogWarning("Volume Type not supported: " + volumeType);
                break;
            }

    }

    public void OnSliderChange(){
        switch (volumeType){
            case VolumeType.SFX:
                AudioManager.instance.SFXVolume = volumeSlider.value;
                break;
            case VolumeType.BGM:
                AudioManager.instance.BGMVolume = volumeSlider.value;
                break;
            default:
                Debug.LogWarning("Volume Type not supported: " + volumeType);
                break;
            }

    }


}
