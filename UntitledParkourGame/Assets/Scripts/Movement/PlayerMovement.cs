using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    [Tooltip("The player’s default movement speed when not sprinting, sliding, etc. Higher number = faster movement")]
    public float runSpeed;
    [Tooltip("the player’s movement speed when in the sprinting state. Higher number = faster movement")]
    public float sprintSpeed;
    //public float slidingSpeed;
    [Tooltip("the player’s movement speed when in the sprinting state. Higher number = faster movement;")]
    public float wallRunSpeed;
    [Tooltip("Value between 0 and 1. Determines how much control the player has over side to side movement while climbing walls.")]
    public float climbSpeed;
    [Tooltip("How much the player decelerates on the ground when there are no movement inputs. Higher number = faster deceleration.")]
    public float groundDrag;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    [Header("Jumping")]
    [Tooltip("How much upwards force is applied to the player when they jump. Higher number = larger force and higher jumps")]
    public float jumpForce;
    [Tooltip("How long the player must wait before jumping after landing from a jump. Higher number = longer cooldown\r\n")]
    public float jumpCooldown;
    [Tooltip("Value between 0 and 1. Determines how much control the player has over their velocity while in the air. Higher number = more control\r\n")]
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    [Tooltip("The Key that triggers the jump action")]
    public KeyCode jumpKey = KeyCode.Space;
    [Tooltip("The Key that triggers the spriting state")]
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    [Tooltip("How tall is the player object?")]
    public float playerHeight;
    [Tooltip("Layer that defines what the ground is")]
    public LayerMask whatIsGround;
    [Tooltip("Boolean that stores if the player is on the ground")]
    public bool grounded;

    [Header("Slope Handling")]
    [Tooltip("Defines the maximum angle of slope the player can move up")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    [Tooltip("A reference to the Climbing Script attached to the player")]
    public Climbing climpingScript;
    [Tooltip("A reference to a GameObject that holds the player’s orientation")]
    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    [Tooltip("an Enum that holds the current state of the player")]
    public MovementState state;

    public enum MovementState
    {
        freeze,
        unlimited,
        running,
        sprinting,
        sliding,
        wallrunning,
        climbing,
        air
    }

    public bool sliding;
    public bool climbing;
    public bool freeze;
    public bool unlimited;

    public bool restricted;

    private void stateHandler()
    {
        //Mode Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity= Vector3.zero;
        } 
        //mode unlimited
        else if (unlimited)
        {
           state = MovementState.unlimited;
            moveSpeed = 999f;
            return;
        } 
        //Mode Climbing
        else if(climbing)
        {
            state = MovementState.climbing;
            moveSpeed = climbSpeed;
        }
        //Mode Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
        }
        //Mode Sliding
        else if (sliding)
        {
            state = MovementState.sliding;
            desiredMoveSpeed = sprintSpeed;
        }
        // Mode - sprinting
        if (grounded && Input.GetKey(sprintKey))
        {
            //Debug.Log("mode sprinting");
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;

        }
        //Mode - Running
        else if (grounded)
        {
            state = MovementState.running;
            desiredMoveSpeed = runSpeed;
        }
        //Mode Air
        else
        {
            state = MovementState.air;

        }
        //check if desiredMoveSpeed has changed 
        if(Mathf.Abs(desiredMoveSpeed-lastDesiredMoveSpeed)>4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        } else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }
    

    public bool wallrunning;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        // top youtube comment sacred knowledge
        readyToJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        stateHandler();
        HandleDrag();
        
    }

    private void HandleDrag(){
        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()

    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        if (restricted) return;

        if(climpingScript.exitingWall) { return; }
        // calc movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // on ground
        if(grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        // in air
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        else if (wallrunning)
        {
            rb.AddForce(moveDirection.normalized * wallRunSpeed * 10f, ForceMode.Force);
        }

        if (!wallrunning)
        {
            rb.useGravity = !OnSlope();
        }
    }

    private void SpeedControl()
    {
        //limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        } else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed-moveSpeed);
        float startValue = moveSpeed;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }
}
