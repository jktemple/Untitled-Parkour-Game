using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimatorController : NetworkBehaviour
{

    [Header("References")]
    public Rigidbody rb;
    public Animator animator;
    public PlayerMovement pm;
    WallRunning wallRunning;
    int VelcotiyHash;
    int GroundHash;
    int FreeFallHash;
    int AirHash;
    int SlideHash;
    int JumpHash;
    int ClimbHash;
    int WallRunLeftHash;
    int WallRunRightHash;
    int CrouchHash;
    // Start is called before the first frame update
    void Start()
    {
        VelcotiyHash = Animator.StringToHash("Velocity");
        GroundHash = Animator.StringToHash("Grounded");
        FreeFallHash = Animator.StringToHash("FreeFall");
        SlideHash = Animator.StringToHash("Sliding");
        JumpHash = Animator.StringToHash("GroundedJump");
        ClimbHash = Animator.StringToHash("Climbing");
        AirHash = Animator.StringToHash("InAir");
        WallRunLeftHash = Animator.StringToHash("WallRunLeft");
        WallRunRightHash = Animator.StringToHash("WallRunRight");
        CrouchHash = Animator.StringToHash("Crouching");
        wallRunning = GetComponent<WallRunning>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;
        
        //Debug.Log("velocity.magnitude = " + rb.velocity.magnitude);
        animator.SetFloat(VelcotiyHash, rb.velocity.magnitude);
        animator.SetBool(GroundHash, pm.grounded);
        animator.SetBool(FreeFallHash, pm.state == PlayerMovement.MovementState.air && rb.velocity.y < 0);
        animator.SetBool(SlideHash, pm.state == PlayerMovement.MovementState.sliding);
        animator.SetBool(ClimbHash, pm.state == PlayerMovement.MovementState.climbing);
        animator.SetBool(AirHash, pm.state == PlayerMovement.MovementState.air);
        animator.SetBool(WallRunLeftHash, pm.state == PlayerMovement.MovementState.wallrunning && wallRunning.wallLeft);
        animator.SetBool(WallRunRightHash, pm.state == PlayerMovement.MovementState.wallrunning && wallRunning.wallRight);
        animator.SetBool(CrouchHash, pm.state == PlayerMovement.MovementState.boosting);
    }

    public void SetGroundJumpTrigger()
    {
        if(!IsOwner) return;
        animator.SetTrigger(JumpHash);
    }
}
