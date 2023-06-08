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
    private RaycastHit backWallHit;
    public bool wallBack;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    private PlayerControls inputs;
    private WallGrab wallgrabScript;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
        lg = GetComponent<LedgeGrabbing>();
        wr = GetComponent<WallRunning>();
        wallgrabScript = GetComponent<WallGrab>();
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
        else if (wallFront && inputs.PlayerMovement.Movement.ReadValue<Vector2>().y > 0.5f && wallLookAngle < maxWallLookAngle && !exitingWall && !wallgrabScript.exitingWall)
        {
            if (!climbing && climbTimer > 0)
            {
               // Debug.Log("staring a climb");
                StartClimbing();
            }

            if (pm.wallGrabbing)
            {
                StopClimbing();
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
        if(((wallFront||wallBack) && inputs.PlayerMovement.Jump.triggered && climbJumpsLeft > 0 && !pm.wallrunning && !wr.exitingWall && minClimbTimer<=0) && !pm.wallGrabbing)
        {
            Debug.Log("Normal Clmb Jump");
            Debug.Log("WallBack = " + wallBack);
            ClimbJump(!wallFront);
        }
        /*
        if (coyoteTimer > 0 && coyoteJumpAvailable && inputs.PlayerMovement.Jump.triggered && climbJumpsLeft > 0 && !pm.wallrunning)
        {
            Debug.Log("Coyote Time Climb Jump");
            ClimbJump(true);
        }
        */

    }

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallBack = Physics.SphereCast(transform.position, sphereCastRadius, -orientation.forward, out backWallHit, detectionLength*1.5f, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);
        if (wallBack) Debug.Log("wallBack = true");

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
        if (pm.wallGrabbing) return;
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

    private void ClimbJump(bool isBackwards)
    {
        if (exitingWall || pm.wallGrabbing) return;
        if (pm.grounded || lg.holding || lg.exitingLedge) return;
        exitingWall = true;
       exitWallTimer = exitWallTime;
        Vector3 forceToApply;
        if (isBackwards)
        {
            forceToApply = transform.up * climbJumpUpForce + backWallHit.normal * climbJumpBackForce;
        } else
        {
            forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;
        }


        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply,ForceMode.Impulse);
        climbJumpsLeft--;
        //Debug.Log("climb jump");
    }
}
