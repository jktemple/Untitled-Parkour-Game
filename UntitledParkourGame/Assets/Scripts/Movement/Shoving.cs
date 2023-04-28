using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using FMOD.Studio;

public class Shoving : NetworkBehaviour
{
    
    public LayerMask playersMask;
    public Transform orientation;
    public Transform playerObject;
    private PlayerMovement pm;
    private PlayerControls inputs;
    public Rigidbody rb;
    public float shoveDistance;
    public float shoveSpherecastRadius;
    public float shoveForce;
    public float shoveCooldown;
    private bool ableToShove;
    public NetworkVariable<bool> shoved = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector3> shoveDir = new NetworkVariable<Vector3>();
    public NetworkVariable<bool> infected = new NetworkVariable<bool>();
    public NetworkVariable<int> score = new NetworkVariable<int>();
    public NetworkVariable<int> playerNumber = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private bool inShoveLag = false;

    public Animator animator;
    private int taggedHash;
    public SphereCastVisual sphereCastVisual;

    public bool hitBoxVisuals;

    private EventInstance playerShovingsfx;
    // Start is called before the first frame update
    void Start()
    {


        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        Debug.Log("shove RigidBody = " + rb);
        if (!IsOwner) return;
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
        ableToShove = true;
        shoved.Value = false;
        taggedHash = Animator.StringToHash("Tagged");
        playerNumber.Value = FindObjectsOfType<Shoving>().Length;

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (ableToShove && inputs.PlayerMovement.Shove.triggered)
        {
            ShoveServerRPC(playerObject.position, orientation.forward, infected.Value);
            ableToShove=false;
            Invoke(nameof(ResetShove), shoveCooldown);
        }
        if(shoved.Value && !inShoveLag)
        {
            rb.AddForce(shoveDir.Value.normalized * shoveForce, ForceMode.Impulse);
            ResetShoveServerRPC();
            inShoveLag = true;
            Invoke(nameof(ResetShoveLag), 0.5f);
        }
        animator.SetBool(taggedHash, infected.Value);
        updateSound();

    }

    [ServerRpc]
    public void ShoveServerRPC(Vector3 position, Vector3 direction, bool infected)
    {
        //if(!IsServer) return;
        Debug.Log("Shove Executing");
        RaycastHit hit;
        if (Physics.SphereCast(position, shoveSpherecastRadius, direction, out hit, shoveDistance, playersMask))
        {
            Debug.Log("Shove Hit");
            Shoving s = hit.transform.GetComponentInParent<Shoving>();
            s.shoved.Value = true;
            s.shoveDir.Value = direction;
            if(infected)
            {
                s.infected.Value = true;
            }
        }
        if (hitBoxVisuals)
        {
            float diam = shoveSpherecastRadius * 2;
            SphereCastVisual st = Instantiate<SphereCastVisual>(sphereCastVisual);
            st.transform.position = position;
            st.diameter = diam;
            SphereCastVisual m = Instantiate<SphereCastVisual>(sphereCastVisual);
            m.transform.position = position + direction.normalized * (shoveDistance / 2);
            m.diameter = diam;
            SphereCastVisual l = Instantiate<SphereCastVisual>(sphereCastVisual);
            l.transform.position = position + direction.normalized * shoveDistance;
            l.diameter = diam;
            st.GetComponent<NetworkObject>().Spawn();
            m.GetComponent<NetworkObject>().Spawn();
            l.GetComponent<NetworkObject>().Spawn();
        }
        //shoot a sphere cast out from the center of the player object in the direction of orientation
        //if it hits call the client rpc to the owner of that player object

    }

    [ServerRpc]
    public void ResetShoveServerRPC()
    {
        shoved.Value = false;
        shoveDir.Value = Vector3.zero;
    }

    private void ResetShove()
    {
        ableToShove = true;
    }

    private void ResetShoveLag()
    {
        inShoveLag = false;
    }


    private void updateSound(){
        if(infected.Value == true){
            PLAYBACK_STATE shovingplaybackState;
            playerShovingsfx.getPlaybackState (out shovingplaybackState);
         
            if(shovingplaybackState.Equals(PLAYBACK_STATE.STOPPED)){
                playerShovingsfx.start();
            }
        }
        else{
            playerShovingsfx.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}
