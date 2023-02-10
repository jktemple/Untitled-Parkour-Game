using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Climbing : NetworkBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public PlayerMovement pm;
    public LedgeGrabbing lg;
    private WallRunning wr;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    public float minClimbTime;
    private float minClimbTimer;
    private float climbTimer;

    private bool climbing;

    [Header("Climb Jumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;
    public KeyCode jumpKey = KeyCode.Space;
    public int climbJumps;
    private int climbJumpsLeft;

   

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    private PlayerControls inputs;

    // Start is called before the first frame update
    void Start()
    {
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
        lg = GetComponent<LedgeGrabbing>();
        wr = GetComponent<WallRunning>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        WallCheck();
        StateMachine();

        if (climbing && !exitingWall)
        {
            ClimbingMovement();
        }

    }

    private void StateMachine()
    {

        //Debug.Log("Forward Input = " + inputs.PlayerMovement.Movement.ReadValue<Vector2>().y);
        //State  0 - ledge Grabbing
        if (lg.holding)
        {
            if(climbing) StopClimbing();
        }

        //state 1 - climbing
        else if (wallFront && inputs.PlayerMovement.Movement.ReadValue<Vector2>().y > 0.5f && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!climbing && climbTimer > 0)
            {
               // Debug.Log("staring a climb");
                StartClimbing();
            }

            if (climbTimer > 0) { climbTimer -= Time.deltaTime; }
            if (climbTimer < 0) { StopClimbing(); }
            if (minClimbTimer > 0)
            {
                minClimbTimer -= Time.deltaTime;
            }
        }
        //state 2 - exiting
        else if (exitingWall)
        {
            if(climbing) { StopClimbing(); }

            if(exitWallTimer > 0) { exitWallTimer -= Time.deltaTime; }
            if(exitWallTimer < 0) { exitingWall = false; }
            
        }
        //State 3 - None
        else
        {
            if (climbing) { StopClimbing(); }
        }
        //Debug.Log("Climb Jummp input =" + inputs.PlayerMovement.Jump.ReadValue<float>());
        if(wallFront && inputs.PlayerMovement.Jump.triggered && climbJumpsLeft > 0 && !pm.wallrunning && !wr.exitingWall && minClimbTimer<=0)
        {
            ClimbJump();
        }
    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;
        
        if ((wallFront && newWall) || pm.grounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    private void StartClimbing()
    {
        //if(climbing) { return; }
        climbing = true;
        pm.climbing = true;
        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
        minClimbTimer = minClimbTime;
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void StopClimbing()
    {
        climbing = false;
        pm.climbing = false;
        //Debug.Log("stopping climbing"); 
        if (!wallFront) { rb.velocity = new Vector3(rb.velocity.x, 0.1f, rb.velocity.z); }
    }

    private void ClimbJump()
    {
        if (exitingWall) return;
        if (pm.grounded || lg.holding || lg.exitingLedge) return;
        exitingWall = true;
        exitWallTimer = exitWallTime;
        
        Vector3 forceToApply = transform.up*climbJumpUpForce + frontWallHit.normal*climbJumpBackForce;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply,ForceMode.Impulse);
        climbJumpsLeft--;
        //Debug.Log("climb jump");
    }
}
