using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//makes the camera follow the player
public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
