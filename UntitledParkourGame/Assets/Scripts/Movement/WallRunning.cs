 using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunnig")]
    [Tooltip("Layer that defines what Walls are")]
    public LayerMask whatIsWall;
    [Tooltip("Layer that defines what the Ground is")]
    public LayerMask whatIsGround;
    [Tooltip("How much force is applied to the player when wallrunning")]
    public float wallRunForce;
    [Tooltip("How long the player can wall run for")]
    public float maxWallRunTime;
    private float wallRunTimer;
    

    [Header("Wall Jumping")]
    [Tooltip("How much upwards force is applied to the player after jumping out of a wall run")]
    public float wallJumpUpForce;
    [Tooltip("How much lateral force is applied to the player after jumping out of the wall")]
    public float wallJumpSideForce;

    [Header("Inputs")]
    [Tooltip("The key that triggers the wall jump action")]
    public KeyCode jumpKey = KeyCode.Space;
    private float horizontalInput;
    private float verticalInput;

    [Header("Dection")]
    [Tooltip("The how close to the wall does the player need to be to be able to wall run")]
    public float wallCheckDistance;
    [Tooltip("How high off the ground must the player be to begin a wall run")]
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    [Tooltip("Bool that indicates if the player is currently exiting a wall")]
    public bool exitingWall;
    [Tooltip("How long the player must wait to begin a wall run after exiting a wall run")]
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Gravity")]
    [Tooltip("Is the player affected by gravity when wall running?")]
    public bool useGravity;
    [Tooltip("If the player is affected by gravity, this determines the amount of force applied to counter gravity")]
    public float gravityCounterForce;

    [Header("Camera Effects")]
    [Tooltip("How far the camera tilts while wall running")]
    public float tiltValue;

    [Header("References")]
    [Tooltip("A reference to a GameObject that stores the player’s orientation")]
    public Transform orientation;
    [Tooltip("A reference to the player camera")]
    public PlayerCam cam;
    private PlayerMovement pm;
    private Rigidbody rb;
    private LedgeGrabbing lg;
    private PlayerControls inputs;

    // Start is called before the first frame update
    void Start()
    {   
       rb = GetComponent<Rigidbody>();
       pm = GetComponent<PlayerMovement>();
        lg = GetComponent<LedgeGrabbing>();
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        //getting inputs
        horizontalInput = inputs.PlayerMovement.Movement.ReadValue<Vector2>().x; 
            //Input.GetAxisRaw("Horizontal");
        verticalInput = inputs.PlayerMovement.Movement.ReadValue<Vector2>().y;

        //State 1 - Wall Running
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            Debug.Log("Wall jump input = " + inputs.PlayerMovement.Jump.ReadValue<float>());
             //Start Wallrun here
             if(!pm.wallrunning)
            {
                StartWallRun();
            }

             if(wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

             if(wallRunTimer<= 0 && pm.wallrunning)
            {
                exitingWall= true;
                exitWallTimer = exitWallTime;
            }

            if (inputs.PlayerMovement.Jump.triggered)
            {
                WallJump();
            }
        }
        //State 2 - Exiting
        else if (exitingWall)
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }

            if(exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if(exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }

        //State 3 None
        else
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //apply camera effects
        cam.DoFov(90f);
        if (wallLeft) cam.DoTilt(-tiltValue);
        if(wallRight) cam.DoTilt(tiltValue);
    }

    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;
       
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        //forward force
        rb.AddForce(wallForward*wallRunForce, ForceMode.Force);

        //weaken Gravity
        if(useGravity)
        {
            rb.AddForce(transform.up*gravityCounterForce, ForceMode.Force);
        }

    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        //reset camera effects
        cam.DoFov(80f);
        cam.DoTilt(0f);
    }

    private void WallJump()
    {
        Debug.Log("Wall Jump");
        if (lg.holding || lg.exitingLedge) return;
        exitingWall = true;
        exitWallTimer = exitWallTime;
        Vector3 wallNormal = wallRight ? rightWallHit.normal: leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal*wallJumpSideForce;
        //add force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
