using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//makes the camera follow the player
public class MoveCamera : NetworkBehaviour
{
    public Transform cameraPosition;

    private void Start()
    {
        Debug.Log("IsOwner " + IsOwner);
       if(IsOwner)
        {
            CameraPlayerFollow.Instance.CameraFollow(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("isOwner " + IsOwner + "ID " + gameObject.GetInstanceID());
        //Debug.Log("isHost " + IsHost);
        if (!IsOwner) return;
        transform.position = cameraPosition.position;
    }
}

