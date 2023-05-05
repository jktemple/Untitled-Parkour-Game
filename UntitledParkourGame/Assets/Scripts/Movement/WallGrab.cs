using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallGrab : NetworkBehaviour
{
    [Header("Refrences")]
    public Transform cam;
    public Transform orientation;
    
    [Header("Jump parameters")]
    public float jumps;
    public float ForwardForce;
    public float UpwardForce;

    private bool exitingWall;
    private float exitWallTime;
    private float exitWallTimer;
    private float jumpsLeft;

    private Rigidbody rb;
    public PlayerMovement pm;
    private PlayerControls inputs;
    

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        rb = GetComponent<Rigidbody>();
        //pm = GetComponent<PlayerMovement>();
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
        exitingWall = false;
        exitWallTime = 0.35f;
        exitWallTimer = exitWallTime;
        jumps = 1;
        jumpsLeft = jumps;
    }

    private void Update()
    {
        if(pm.grounded)
        {
            jumpsLeft = jumps;
        }
        if(pm.wallGrabbing && inputs.PlayerMovement.Jump.IsPressed() && jumpsLeft > 0)
        {
            pm.wallGrabbing = false;
            exitingWall = true;
            jumpsLeft--;
            Vector3 forceToApply = (cam.forward * ForwardForce) + (orientation.up * UpwardForce);
            rb.velocity = Vector3.zero;
            rb.AddForce(forceToApply, ForceMode.Impulse);
        }
        if(exitingWall)
        {
            if (exitWallTimer > 0)
            {

                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <= 0)
            {
                Debug.Log("got here");
                exitingWall = false;
                exitWallTimer = exitWallTime;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("whatIsWall"))
        {
            if (inputs.PlayerMovement.WallGrab.IsInProgress() && !exitingWall)
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
