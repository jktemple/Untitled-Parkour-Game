using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Boosting : NetworkBehaviour
{
    [Header("Refrences")]
    public PlayerMovement pm;
    public Transform playerObj;
    public Transform orientation;
    public Transform cam;
    public Transform playerObject;
    public Rigidbody rb;
    public LayerMask whatIsPlayer;
    public SphereCastVisual sphereCastVisual;

    public float adjustAmount;

    [Header("Boosting")]
    public float boostSphereCastDistance;
    public float boostSphereCastRadius;
    public float boostYScale;
    private float startYScale;
    public float boostForwardForce;
    public float boostUpwardForce;
    public float boostJumpCooldown;
    //private float boostJumpTimer;
    private bool boosting;
    private bool readyToBoostJump;

    RaycastHit boostHit;

    private PlayerControls inputs;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
        startYScale = playerObj.localScale.y;
        readyToBoostJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        StateMachine();
    }

    void StateMachine()
    {
        Vector3 adjustment = transform.position;
        adjustment.y += adjustAmount;
        Physics.SphereCast(adjustment, boostSphereCastRadius, Vector3.down, out boostHit, boostSphereCastDistance, whatIsPlayer);



        //Debug.Log(boostHit.transform);
        if (pm.grounded && inputs.PlayerMovement.Boost.IsPressed() && !pm.sliding)
        {
            Debug.Log("Starting Boost");
            StartBoosting();
        }
        else if(!pm.grounded && inputs.PlayerMovement.Jump.triggered && readyToBoostJump)
        {
            // the spot where canBoost will go

            if (boostHit.transform != null)
            {
                if (boostHit.transform.GetComponent<PlayerMovement>().boosting.Value)
                {
                    //Debug.Log("Boost Jump");
                    BoostJump();
                    Invoke(nameof(ResetBoostJump), boostJumpCooldown);
                }
            }
        } else if(pm.boosting.Value)
        {
            StopBoosting();
        }
    }

    void StartBoosting()
    {
        boosting = true;
        pm.boosting.Value = true;
        rb.velocity = Vector3.zero;
        playerObj.localScale = new Vector3(playerObj.localScale.x, boostYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    void StopBoosting()
    {
        boosting = false;
        pm.boosting.Value = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }

    void BoostJump()
    {

        Debug.Log("Boost Jump");
        Vector3 forceToApply = transform.up * boostUpwardForce + cam.transform.forward * boostForwardForce;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
        readyToBoostJump = false;
    }

    private void ResetBoostJump()
    {
        readyToBoostJump = true;
    }
}
