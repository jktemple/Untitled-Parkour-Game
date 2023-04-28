using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallGrab : NetworkBehaviour
{
    private Rigidbody rb;
    private PlayerMovement pm;
    private PlayerControls inputs;
    

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("whatIsWall"))
        {
            if (inputs.PlayerMovement.WallGrab.IsInProgress())
            {
                pm.wallGrabbing = true;
            }
            else
            {
                pm.wallGrabbing = false;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("whatIsWall"))
        {
            pm.wallGrabbing = false;
        }  
    }
}
