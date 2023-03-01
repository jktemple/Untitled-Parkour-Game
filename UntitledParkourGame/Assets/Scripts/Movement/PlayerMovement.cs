using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    [Tooltip("The player’s default movement speed when not sprinting, sliding, etc. Higher number = faster movement")]
    public float runSpeed;
    [Tooltip("the player’s movement speed when in the sprinting state. Higher number = faster movement")]
    public float sprintSpeed;
    [Tooltip("The modifier applier the player when moving backwards. Between 0 and 1")]
    public float backwardsModifier;
    //public float slidingSpeed;
    [Tooltip("the player’s movement speed when in the Wallrunning state. Higher number = faster movement;")]
    public float wallRunSpeed;
    [Tooltip("Value between 0 and 1. Determines how much control the player has over side to side movement while climbing walls.")]
    public float climbSpeed;
    [Tooltip("How much the player decelerates on the ground when there are no movement inputs. Higher number = faster deceleration.")]
    public float groundDrag;

    [SerializeField]
    [Tooltip("good at 2000")]
    private float maxStamina;
    [SerializeField]
    [Tooltip("good at 10")]
    private float staminaDrainRate;
    [SerializeField]
    [Tooltip("good at 5")]
    private float staminaRechargeRate;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private float currentStamina;
    
    //private float sprintDelayTime; // for if sprint delay is added
    //private float delayTimeLeft; // for if sprint delay is added

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
        boosting,
        air
    }

    public bool sliding;
    public bool climbing;
    public bool freeze;
    public bool unlimited;
    public NetworkVariable<bool> boosting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public bool restricted;

    //public bool notControllable;
    private PlayerControls inputs;
    private void stateHandler()
    {
        //Mode Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity= Vector3.zero;
        } else if (boosting.Value)
        {
            state = MovementState.boosting;
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
        else if (grounded && (inputs.PlayerMovement.Sprint.ReadValue<float>() > 0.1f) && currentStamina > 0)
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

    public bool boostTest;
    void Start()
    {
        if (!IsOwner) return;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        inputs = new PlayerControls();
        
        inputs.PlayerMovement.Enable();
        
        // top youtube comment sacred knowledge
        readyToJump = true;

        currentStamina = maxStamina;
        // sprintDelayTime = 1; // in seconds // for if these are implemented later
        // delayTimeLeft = sprintDelayTime; 
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        stateHandler();
        HandleDrag();
        
    }

    private void HandleDrag()
    {
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
        if (!IsOwner) return;
        MovePlayer();

        // vvv sprint stamina stuff, move into functions when done vvv

        if (state == MovementState.sprinting)
        {
            if (currentStamina > 0)
            {
                if(currentStamina < staminaDrainRate)
                {
                    currentStamina = 0;
                }
                else
                {
                    currentStamina -= staminaDrainRate;
                }
            }
            else
            {
                state = MovementState.running;
                desiredMoveSpeed = runSpeed;
            }
        }
        else if (state == MovementState.running)
        {
            if(currentStamina < maxStamina)
            {
                currentStamina += staminaRechargeRate;
            }
        }
        else { }

        // ^^^ sprint stamina stuff, move into functions when done ^^^


    }

    private void MyInput()
    {
        if (boostTest)
        {
            return;
        }
        horizontalInput = inputs.PlayerMovement.Movement.ReadValue<Vector2>().x; 
            //Input.GetAxisRaw("Horizontal");
        verticalInput = inputs.PlayerMovement.Movement.ReadValue<Vector2>().y;
        //Debug.Log("Horizontal Input = " + horizontalInput + " , Verticle Input = " + verticalInput);
        // when to jump
        bool jumpInput = inputs.PlayerMovement.Jump.triggered;
        if(jumpInput && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        
        if (restricted) return;

        if (boostTest) { 
            boosting.Value = true;
            return;
        }

        if(climpingScript.exitingWall) { return; }
        // calc movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        float targetSpeed = moveSpeed;
        if (verticalInput< 0)
        {
            targetSpeed *= backwardsModifier;
        }

        //on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * targetSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // on ground
        if(grounded)
        {
            rb.AddForce(moveDirection.normalized * targetSpeed * 10f, ForceMode.Force);
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

    public void GetShoved(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}
