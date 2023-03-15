using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Footstep Sfx")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }

    [field: Header("Sliding Sfx")]
    [field: SerializeField] public EventReference playerSlidingsfx { get; private set; }

    [field: Header("Wallrunning Sfx")]
    [field: SerializeField] public EventReference playerWallrunningsfx { get; private set; }

    [field: Header("Wallclimbing Sfx")]
    [field: SerializeField] public EventReference playerWallclimbingsfx { get; private set; }

    [field: Header("Boosting Sfx")]
    [field: SerializeField] public EventReference playerBoostingsfx { get; private set; }

    [field: Header("Jumping Sfx")]
    [field: SerializeField] public EventReference playerJumpingsfx { get; private set; }

    public static FMODEvents instance{ get; private set; }


    private void Awake(){
        if(instance != null){
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;
    }
}
