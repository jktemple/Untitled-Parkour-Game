using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sliding : NetworkBehaviour
{
    [Header("References")]
    [Tooltip("A Reference to a GameObject that holds the player’s orientation")]
    public Transform orientation;
    [Tooltip("A Reference to PlayerObj GameObject, used to scale the player down when sliding")]
    public Transform playerObj;
    public Transform cam;
    public float ForwardForce;
    public float UpwardForce;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    [Tooltip("How long the player is able to slide in seconds")]
    public float maxSlideTime;
    [Tooltip("How much force is applied to the player when they begin sliding")]
    public float slideForce;
    private float slideTimer;
    [Tooltip("Value from 0 to 1, determines how much the player is scaled down when they slide")]
    public float slideYScale;
    private float startYScale;

    [Header("Inputs")]
    [Tooltip("The Key that triggers the slide action")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private PlayerControls inputs;

    


    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        startYScale = playerObj.localScale.y;
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        horizontalInput = inputs.PlayerMovement.Movement.ReadValue<Vector2>().x;
        //Input.GetAxisRaw("Horizontal");
        verticalInput = inputs.PlayerMovement.Movement.ReadValue<Vector2>().y;
        //Debug.Log("Horizontal Input = " + horizontalInput + " , Verticle Input = " + verticalInput);
        if (inputs.PlayerMovement.Slide.triggered && (horizontalInput !=0 || verticalInput != 0))
        {
            StartSlide();
        }

        if (inputs.PlayerMovement.Slide.WasReleasedThisFrame() && pm.sliding)
        {
            Debug.Log("Stopping Slide because of Input");
            Vector3 forceToApply = (cam.forward * ForwardForce) + (orientation.up * UpwardForce);
            rb.velocity = Vector3.zero;
            rb.AddForce(forceToApply, ForceMode.Impulse);
            StopSlide();
        }

        //Debug.Log(playerObj.localScale);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (pm.sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        if(pm.sliding) { return; }
        //Debug.Log("Starting Slide " + Time.deltaTime);
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void StopSlide()
    {
        //Debug.Log("Stopping Slide " + Time.deltaTime);
        pm.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        //sliding noramally
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        //sliding downward
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }




        if(slideTimer <= 0)
        {
            Debug.Log("Stopping Slide because of Timer");
            StopSlide();
        }
    }
}
