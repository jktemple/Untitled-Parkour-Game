using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Boosting : MonoBehaviour
{
    [Header("Refrences")]
    public PlayerMovement pm;
    public Transform playerObj;
    public PlayerCam cam;
    public Rigidbody rb;
    public LayerMask whatIsPlayer;

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
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
        startYScale = playerObj.localScale.y;
        readyToBoostJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine();
    }

    void StateMachine()
    {
        Physics.SphereCast(transform.position, boostSphereCastRadius, Vector3.down, out boostHit, boostSphereCastDistance, whatIsPlayer);
        //Debug.Log(boostHit.transform);
        if (pm.grounded && inputs.PlayerMovement.Boost.ReadValue<float>() > 0.1f )
        {
            Debug.Log("Starting Boost");
            StartBoosting();
        }
        else if(!pm.grounded && inputs.PlayerMovement.Jump.ReadValue<float>() > 0.1f && readyToBoostJump)
        {
            
            if (boostHit.transform != null)
            {
                if (boostHit.transform.GetComponent<PlayerMovement>().boosting)
                {
                    //Debug.Log("Boost Jump");
                    BoostJump();
                    Invoke(nameof(ResetBoostJump), boostJumpCooldown);
                }
            }
        } else
        {
            StopBoosting();
        }
    }

    void StartBoosting()
    {
        boosting = true;
        pm.boosting = true;
        rb.velocity = Vector3.zero;
        playerObj.localScale = new Vector3(playerObj.localScale.x, boostYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    void StopBoosting()
    {
        boosting = false;
        pm.boosting = false;
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
