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

    [Header("Jump forces")]
    public float ForwardForce;
    public float UpwardForce;

    [Header("Exiting")]
    [Tooltip("Bool that indicates if the player is currently exiting a wall")]
    public bool exitingWall;
    private float exitWallTime;
    private float exitWallTimer;


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
        exitingWall = false;
        exitWallTime = 0.35f;
        exitWallTimer = exitWallTime;
    }

    private void Update()
    {
        if(pm.wallGrabbing && inputs.PlayerMovement.Jump.IsPressed())
        {
            pm.wallGrabbing = false;
            exitingWall = true;
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
