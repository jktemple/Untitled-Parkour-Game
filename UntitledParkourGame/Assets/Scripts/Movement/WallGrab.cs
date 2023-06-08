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
    public float ForwardForce;
    public float UpwardForce;

    public bool exitingWall;
    private float exitWallTime;
    private float exitWallTimer;

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
        exitWallTime = 0.5f;
        exitWallTimer = exitWallTime;
    }

    private void Update()
    {
        if(!IsOwner) { return; }

        if(pm.wallGrabbing && inputs.PlayerMovement.Jump.IsPressed() && pm.currentStamina > 1)
        {
            pm.wallGrabbing = false;
            exitingWall = true;
            Vector3 forceToApply = (cam.forward * ForwardForce) + (orientation.up * UpwardForce);
            rb.velocity = Vector3.zero;
            rb.AddForce(forceToApply, ForceMode.Impulse);

            if(pm.currentStamina > 10)
            {
                pm.currentStamina -= 10;
            }
            else
            {
                pm.currentStamina = 0;
            }
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
        if (!IsOwner) { return; }
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
        if(!IsOwner) { return; }
        if (other.gameObject.layer == LayerMask.NameToLayer("whatIsWall"))
        {
            pm.wallGrabbing = false;
        }  
    }
}
