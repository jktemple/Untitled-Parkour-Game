using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunnig")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Inputs")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Dection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {   
       rb = GetComponent<Rigidbody>();
       pm = GetComponent<PlayerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        CheckForWall();
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
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //State 1 - Wall Running
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
             //Start Wallrun here
        }
    }

    private void StartWallRun()
    {

    }

    private void WallRunningMovement()
    {

    }

    private void StopWallRun()
    {

    }
}
