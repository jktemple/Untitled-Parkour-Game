using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{

    [Header("References")]
    public Rigidbody rb;
    public Animator animator;
    public PlayerMovement pm;
    int VelcotiyHash;
    int GroundHash;
    int AirHash;
    int SlideHash;
    // Start is called before the first frame update
    void Start()
    {
        VelcotiyHash = Animator.StringToHash("Velocity");
        GroundHash = Animator.StringToHash("Grounded");
        AirHash = Animator.StringToHash("InAir");
        SlideHash = Animator.StringToHash("Sliding");
    }

    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log("velocity.magnitude = " + rb.velocity.magnitude);
        animator.SetFloat(VelcotiyHash, rb.velocity.magnitude);
        animator.SetBool(GroundHash, pm.grounded);
        if(pm.state == PlayerMovement.MovementState.air)
        {
            animator.SetBool(AirHash, true);
        } else
        {
            animator.SetBool(AirHash, false);
        }

        if(pm.state == PlayerMovement.MovementState.sliding) 
        { 
            animator.SetBool(SlideHash, true);
        } else
        {
            animator.SetBool(SlideHash, false);
        }
    }
}
