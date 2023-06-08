using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using FMOD.Studio;
using TMPro;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    [Tooltip("The player�s default movement speed when not sprinting, sliding, etc. Higher number = faster movement")]
    public float runSpeed;
    public float infectedRunSpeed;
    [Tooltip("the player�s movement speed when in the sprinting state. Higher number = faster movement")]
    public float sprintSpeed;
    [Tooltip("The modifier applier the player when moving backwards. Between 0 and 1")]
    public float backwardsModifier;
    //public float slidingSpeed;
    [Tooltip("the player�s movement speed when in the Wallrunning state. Higher number = faster movement;")]
    public float wallRunSpeed;
    [Tooltip("Value between 0 and 1. Determines how much control the player has over side to side movement while climbing walls.")]
    public float climbSpeed;
    [Tooltip("How much the player decelerates on the ground when there are no movement inputs. Higher number = faster deceleration.")]
    public float groundDrag;

    [SerializeField]
    [Tooltip("fine at 100")]
    private float maxStamina;
    [SerializeField]
    [Tooltip("fine at 25")]
    private float staminaDrainRate;
    [SerializeField]
    [Tooltip("fine at 10")]
    private float staminaRechargeRate;
    [SerializeField]
    private float WallrunningStaminaRechargeRate;
    [Tooltip("fine at 30")]


    private Shoving shoving;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float currentStamina;
    
    //private float sprintDelayTime; // for if sprint delay is added
    //private float delayTimeLeft; // for if sprint delay is added

    private EventInstance playerFootsteps;
    private EventInstance playerSlidingsfx;
    private EventInstance playerWallrunningsfx;
    private EventInstance playerWallclimbingsfx;
    private EventInstance playerBoostingsfx;
    private EventInstance playerJumpingsfx;


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
    private bool unpaused;

    [Header("Slope Handling")]
    [Tooltip("Defines the maximum angle of slope the player can move up")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    [Tooltip("A reference to the Climbing Script attached to the player")]
    public Climbing climbingScript;
    [Tooltip("A reference to a GameObject that holds the player�s orientation")]
    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    [Tooltip("an Enum that holds the current state of the player")]

    public PlayerCam cam;
    public MovementState state;

    // Movement state indication UI
    private GameObject canvas;
    private GameObject movementIcon;
    private TextMeshProUGUI icon;


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
        wallGrabbing,
        air
    }

    public bool sliding;
    public bool climbing;
    public bool freeze;
    public bool unlimited;
    public bool wallGrabbing;
    public bool quickTurned = false;
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

            if (icon != null)
            {
                icon.text = "<sprite=1>";
            }

        } else if (boosting.Value)
        {
            state = MovementState.boosting;
            rb.velocity= Vector3.zero;

            if (icon != null)
            {
                icon.text = "<sprite=5>";
            }
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
            //cam.DoFov(40f);
            state = MovementState.climbing;
            moveSpeed = climbSpeed;

            if (icon != null)
            {
                icon.text = "<sprite=2>";
            }

            // sprint stamina handling
            if (currentStamina < maxStamina)
            {
                // wont go above max
                if (currentStamina + staminaRechargeRate * Time.deltaTime > maxStamina)
                {
                    currentStamina = maxStamina;
                }
                else
                {
                    currentStamina += WallrunningStaminaRechargeRate * Time.deltaTime;
                }

                //  Debug.Log("Current Stamina wallrunning: " + currentStamina);
            }
        }
        //Mode Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;

            if (icon != null)
            {
                icon.text = "<sprite=3>";
            }

            // sprint stamina handling
            if (currentStamina < maxStamina)
            {
                // wont go above max
                if (currentStamina + staminaRechargeRate * Time.deltaTime > maxStamina)
                {
                    currentStamina = maxStamina;
                }
                else
                {
                    currentStamina += WallrunningStaminaRechargeRate * Time.deltaTime;
                }

                //  Debug.Log("Current Stamina wallrunning: " + currentStamina);
            }
        }
        //Mode Sliding
        else if (sliding)
        {
            if (icon != null)
            {
                icon.text = "<sprite=4>";
            }

            //cam.DoFov(40f);
            state = MovementState.sliding;
            desiredMoveSpeed = shoving.infected.Value ? infectedRunSpeed : runSpeed;
        }
        // Mode - sprinting
        // else if (grounded && (inputs.PlayerMovement.Sprint.ReadValue<float>() > 0.1f) && currentStamina > 0)
        else if (grounded && (inputs.PlayerMovement.Sprint.ReadValue<float>() > 0.1f) && ((inputs.PlayerMovement.Movement.ReadValue<Vector2>().x != 0) || (inputs.PlayerMovement.Movement.ReadValue<Vector2>().y != 0)) && currentStamina > 0)
        {
            if(inputs.PlayerMovement.Sprint.triggered){cam.AddToFov(5f); }
            
            //Debug.Log("mode sprinting");
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;

            if (icon != null)
            {
                icon.text = "<sprite=0>";
            }

            // sprint stamina handling
            if (currentStamina < staminaDrainRate * Time.deltaTime)
            {
                currentStamina = 0;
                // Debug.Log("Current Stamina Sprinting depleted: " + currentStamina);
            }
            else
            {
                currentStamina -= staminaDrainRate * Time.deltaTime;
                // Debug.Log("Current Stamina Sprinting: " + currentStamina);
            }
        }
        //Mode - Running
        else if (grounded)
        {
            state = MovementState.running;
           // Debug.Log("RunSpeed.value = " + runSpeed.Value);
            desiredMoveSpeed = shoving.infected.Value ? infectedRunSpeed : runSpeed;
            //Debug.Log("desiredMoveSpeed = " + desiredMoveSpeed);
            if (icon != null)
            {
                icon.text = "<sprite=1>";
            }

            // sprint stamina handling
            if (currentStamina < maxStamina)
            {
                // wont go above max
                if(currentStamina + staminaRechargeRate * Time.deltaTime > maxStamina)
                {
                    currentStamina = maxStamina;
                }
                else
                {
                    currentStamina += staminaRechargeRate * Time.deltaTime;
                }
                
                // Debug.Log("Current Stamina running: " + currentStamina);
            }
        }
        else if (wallGrabbing)
        {
            state = MovementState.wallGrabbing;
            rb.velocity = Vector3.zero;

            if (icon != null)
            {
                icon.text = "<sprite=3>";
            }

            // stamina handling
            if (currentStamina < staminaDrainRate/2 * Time.deltaTime)
            {
                currentStamina = 0;
                // Debug.Log("Current Stamina Sprinting depleted: " + currentStamina);
            }
            else
            {
                currentStamina -= staminaDrainRate/2 * Time.deltaTime;
                // Debug.Log("Current Stamina Sprinting: " + currentStamina);
            }
        }
        //Mode Air
        else
        {
            state = MovementState.air;

            if (icon != null)
            {
                icon.text = "<sprite=6>";
            }

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

        if(state != MovementState.sprinting && state != MovementState.wallrunning){
            cam.ResetFov();
        }
    }

    public bool wallrunning;
    // Start is called before the first frame update

    public bool boostTest;

    PlayerAnimatorController animatorController;
    void Start()
    {
        if (!IsOwner) return;
        rb = GetComponent<Rigidbody>();
        shoving = GetComponent<Shoving>();
        climbingScript = GetComponent<Climbing>();
        rb.freezeRotation = true;

        inputs = new PlayerControls();
        
        inputs.PlayerMovement.Enable();
        
        // top youtube comment sacred knowledge
        readyToJump = true;

        currentStamina = maxStamina;
        // sprintDelayTime = 1; // in seconds // for if these are implemented later
        // delayTimeLeft = sprintDelayTime; 

        


        unpaused = true;
        groundCoyoteTimer = groundCoyoteTime;

        // Movement state indication UI
        movementIcon = GameObject.Find("Movement Icon UI").gameObject;
        
        if(movementIcon == null ) 
        {
            Debug.Log("Couldn't find Movement Icon");
        }

        if (movementIcon != null)
        {
            icon = movementIcon.GetComponent<TextMeshProUGUI>();
            Debug.Log("Icon == " + icon.ToString());
        }
        else
        {
            Debug.Log("Couldn't find Movement Icon UI");
        }

        animatorController = GetComponent<PlayerAnimatorController>();
        if(animatorController == null)
        {
            Debug.Log("Animator Controller is null");
        }

        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
        playerSlidingsfx = AudioManager.instance.CreateInstance(FMODEvents.instance.playerSlidingsfx);
        playerWallrunningsfx = AudioManager.instance.CreateInstance(FMODEvents.instance.playerWallrunningsfx);
        playerWallclimbingsfx = AudioManager.instance.CreateInstance(FMODEvents.instance.playerWallclimbingsfx);
        playerBoostingsfx = AudioManager.instance.CreateInstance(FMODEvents.instance.playerBoostingsfx);
        playerJumpingsfx = AudioManager.instance.CreateInstance(FMODEvents.instance.playerJumpingsfx);
    }


    public float groundCoyoteTime;
    private float groundCoyoteTimer;
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        if (!grounded && groundCoyoteTimer > 0)
        {
            groundCoyoteTimer -= Time.deltaTime;
        } else if(grounded)
        {
            groundCoyoteTimer = groundCoyoteTime;
        }
        MyInput();
        SpeedControl();
        stateHandler();
        HandleDrag();
        UpdateSound();
        if (grounded)
        {
            quickTurned = false;
        }
        
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
        if(jumpInput && readyToJump && (grounded || groundCoyoteTimer > 0))
        {
            if (!grounded)
            {
                Debug.Log("Coyote Time Jump");
            }

            readyToJump = false;

            // sry for nesting it's just one layer
            if(state == MovementState.sprinting)
            {
                if (currentStamina >= 10)
                {
                    currentStamina -= 10;
                }
                else
                {
                    currentStamina = 0;
                }
            }
            

            PLAYBACK_STATE jumpingplaybackState;
            playerJumpingsfx.getPlaybackState (out jumpingplaybackState);
         
            if(jumpingplaybackState.Equals(PLAYBACK_STATE.STOPPED)){
                playerJumpingsfx.start();
            }

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);

            playerJumpingsfx.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void MovePlayer()
    {
        
        if (restricted) return;

        if (boostTest) { 
            boosting.Value = true;
            return;
        }

        if(climbingScript.exitingWall) { return; }
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
        else if(!grounded && !collidingWithWall)
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

        if(animatorController!= null)
        animatorController.SetGroundJumpTrigger();
        // FMODUnity.RuntimeManager.PlayOneShot("event:/Jumping", GetComponent<Transform>().position);
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



    private void UpdateSound(){
        if(Keyboard.current.escapeKey.wasPressedThisFrame || Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame || Keyboard.current.tabKey.wasPressedThisFrame){
            if (unpaused == true){
                unpaused = false;
            }
            else{
                unpaused = true;
            }
        }

        if((rb.velocity.x != 0 || rb.velocity.y != 0) && grounded && unpaused){
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState (out playbackState);
         
            if(playbackState.Equals(PLAYBACK_STATE.STOPPED)){
                playerFootsteps.start();
            }
        }
        else{
            playerFootsteps.stop(STOP_MODE.IMMEDIATE);
        }


        if(sliding){
            PLAYBACK_STATE slidingplaybackState;
            playerSlidingsfx.getPlaybackState (out slidingplaybackState);
         
            if(slidingplaybackState.Equals(PLAYBACK_STATE.STOPPED)){
                playerSlidingsfx.start();
            }
        }
        else{
            playerSlidingsfx.stop(STOP_MODE.ALLOWFADEOUT);
        }

        
        if(wallrunning){
            PLAYBACK_STATE wallrunningplaybackState;
            playerWallrunningsfx.getPlaybackState (out wallrunningplaybackState);
         
            if(wallrunningplaybackState.Equals(PLAYBACK_STATE.STOPPED)){
                playerWallrunningsfx.start();
            }
        }
        else{
            playerWallrunningsfx.stop(STOP_MODE.ALLOWFADEOUT);
        }


        if(climbing){
            PLAYBACK_STATE climbingplaybackState;
            playerWallclimbingsfx.getPlaybackState (out climbingplaybackState);
         
            if(climbingplaybackState.Equals(PLAYBACK_STATE.STOPPED)){
                playerWallclimbingsfx.start();
            }
        }
        else{
            playerWallclimbingsfx.stop(STOP_MODE.ALLOWFADEOUT);
        }

        if(boosting.Value == true){
            PLAYBACK_STATE boostingplaybackstate;
            playerBoostingsfx.getPlaybackState (out boostingplaybackstate);
         
            if(boostingplaybackstate.Equals(PLAYBACK_STATE.STOPPED)){
                playerBoostingsfx.start();
            }
        }
        else{
            playerBoostingsfx.stop(STOP_MODE.ALLOWFADEOUT);
        }

    }

    public void ResetStamina()
    {
        currentStamina = maxStamina;
    }

    bool collidingWithWall = false;
    private void OnCollisionStay(Collision collision)
    {
        
        if(whatIsGround == (whatIsGround | (1 << collision.gameObject.layer)))
        {
            collidingWithWall = !climbingScript.wallBack;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (whatIsGround == (whatIsGround | (1 << collision.gameObject.layer)))
        {
            collidingWithWall = false;
        }
    }
}
