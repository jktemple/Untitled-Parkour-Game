using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class LedgeGrabbing : NetworkBehaviour
{
    [Header("References")]
    public PlayerMovement pm;
    public Transform orientation;
    public Transform cam;
    public Rigidbody rb;

    [Header("Ledge Grabbing")]
    public float moveToLedgeSpeed;
    public float maxLedgeGrabDistance;

    public float minTimOnLedge;
    public float maxTimOnLedge;
    private float timeOnLedge;

    public bool holding;

    [Header("Ledge Jumping")]
    public KeyCode jumpKey = KeyCode.Space;
    public float ledgeJumpForwardForce;
    public float ledgeJumpUpwardForce;

    [Header("Ledge Getup")]
    public KeyCode ledgeGetupKey = KeyCode.W;
    public float playerHeight;
    public float playerRadius;
    public float heightOffset;
    public float getupTime;


    [Header("Ledge Detection")]
    public float ledgeDectectionLength;
    public float ledgeSphereCastRadius;
    public LayerMask whatIsLedge;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;

    [Header("Exciting")]
    public bool exitingLedge;
    public float exitLedgeTime;
    private float exitLedgeTimer;

    private PlayerControls inputs;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        LedgeDectection();
        SubStateMachine();
    }

    private void SubStateMachine()
    {
        float horizontalInput = inputs.PlayerMovement.Movement.ReadValue<Vector2>().x;
        //Input.GetAxisRaw("Horizontal");
        float verticalInput = inputs.PlayerMovement.Movement.ReadValue<Vector2>().y;
        bool anyInputKeyPressed = horizontalInput > 0.4f || verticalInput > 0.4f || horizontalInput <-0.4f || verticalInput < -0.4f;

        //substate 1 holding onto ledge
        if (holding)
        {
            FreezeRigidbodyOnLedge();
            timeOnLedge += Time.deltaTime;

            if(timeOnLedge> minTimOnLedge && inputs.PlayerMovement.LedgeGetUp.ReadValue<float>() > 0.5f)
            {
                LedgeGetup();
            }

            if(timeOnLedge > minTimOnLedge && anyInputKeyPressed) { ExitLedgeHold(); }

            if (inputs.PlayerMovement.Jump.triggered) LedgeJump();
        }
        //substate 2 exiting ledge
        else if(exitingLedge)
        {
            if (exitLedgeTime > 0) { exitLedgeTimer -= Time.deltaTime; }
            else exitingLedge = false;
        }
    }

    private void LedgeDectection()
    {
        bool ledgeDectected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDectectionLength, whatIsLedge);
        if(!ledgeDectected)
        {
            return;
        }
        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

        if (ledgeHit.transform == lastLedge) return;

        if(distanceToLedge < maxLedgeGrabDistance && !holding)
        {
            EnterLedgeHold();
        }
  
    }

    private void LedgeGetup()
    {
        if(ledgeHit.transform == null) return;
        ExitLedgeHold();
        Debug.Log("Ledge Getup");
        RaycastHit targetPos;

        Debug.Log(ledgeHit.transform);
        if (Physics.Raycast(transform.position + (ledgeHit.transform.forward * playerRadius/2) + (Vector3.up*heightOffset*playerHeight), Vector3.down, out targetPos, 2*playerHeight)){
            Debug.Log("Found place to land");
            StartCoroutine(LerpVault(targetPos.point + new Vector3(0f, playerHeight/2, 0f), getupTime));
        } 
    }





    IEnumerator LerpVault(Vector3 position, float duration)
    {
        float time = 0f;
        Vector3 startPos = transform.position;
        while(time < duration)
        {
            transform.position = Vector3.Lerp(startPos, position, time/duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = position;
    }

    private void LedgeJump()
    {
        ExitLedgeHold();
      
        Invoke(nameof(DelayedJumpForce),0.05f);
    }

    private void DelayedJumpForce()
    {
        Vector3 forceToApply = cam.forward * ledgeJumpForwardForce + orientation.up * ledgeJumpUpwardForce;
        rb.velocity = Vector3.zero;
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    private void EnterLedgeHold()
    {
        exitingLedge = true;
        exitLedgeTimer = exitLedgeTime;

        holding = true;
        pm.unlimited = true;
        pm.restricted = true;
        currLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity = false;
        rb.velocity= Vector3.zero;
    }

    private void FreezeRigidbodyOnLedge()
    {
        rb.useGravity=false;
        Vector3 directionToLedge = currLedge.position - transform.position;
        float distanceToLedge = Vector3.Distance(transform.position, currLedge.position);
        //move player towards ledge
        if (distanceToLedge > 1f)
        {
            if (rb.velocity.magnitude < moveToLedgeSpeed)
            {
                rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime);
            }
        }
        //hold onto ledge
        else
        {
            if (!pm.freeze) pm.freeze = true;
            if(pm.unlimited) { pm.unlimited = false; }
        }

        if(distanceToLedge > maxLedgeGrabDistance)
        {
            ExitLedgeHold();
        }
    }
    
    private void ExitLedgeHold()
    {
        holding = false;
        pm.restricted = false;
        pm.freeze = false;
        timeOnLedge = 0f;
        rb.useGravity = true;
        rb.velocity = Vector3.zero;

        Invoke(nameof(ResetLastLedge), 1f);
    }

    private void ResetLastLedge()
    {
        lastLedge = null;
    }
}
