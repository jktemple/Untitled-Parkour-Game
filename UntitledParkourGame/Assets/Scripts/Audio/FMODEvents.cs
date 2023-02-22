using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Footstep Sfx")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }

    public static FMODEvents instance{ get; private set; }


    private void Awake(){
        if(instance != null){
            Debug.LogError("Found more than one Aduio Manager in the scene.");
        }
        instance = this;
    }
}
