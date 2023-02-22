using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {


        pm = GetComponent<PlayerMovement>();
        //rb = GetComponent<Rigidbody>();
        Debug.Log("shove RigidBody = " + rb);
        if (!IsOwner) return;
        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
        ableToShove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (ableToShove && inputs.PlayerMovement.Shove.triggered)
        {
            ShoveServerRPC(playerObject.position, orientation.forward);
            ableToShove=false;
            Invoke(nameof(ResetShove), shoveCooldown);
        }
    }

    [ServerRpc]
    public void ShoveServerRPC(Vector3 position, Vector3 direction)
    {
        //if(!IsServer) return;
        Debug.Log("Shove Executing");
        RaycastHit hit;
        if (Physics.SphereCast(position, shoveSpherecastRadius, direction, out hit, shoveDistance, playersMask))
        {
            Debug.Log("Shove Hit");
           ulong hitID = hit.transform.GetComponentInParent<NetworkObject>().OwnerClientId;
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { hitID }
                }
            };
            Debug.Log("OwnerClientId of the hit object " + hitID);
            //var client = NetworkManager.ConnectedClients[hitID];
            //client.PlayerObject.GetComponentInChildren<Rigidbody>().AddForce(orientation.forward * shoveForce, ForceMode.Impulse);
            ShoveClientRPC(orientation.forward, clientRpcParams);
            //hit.transform.GetComponentInParent<Rigidbody>().AddForce(orientation.forward * shoveForce, ForceMode.Impulse);
        }

        //shoot a sphere cast out from the center of the player object in the direction of orientation
        //if it hits call the client rpc to the owner of that player object

    }

    [ClientRpc]
    public void ShoveClientRPC(Vector3 direction, ClientRpcParams clientRpcParams)
    {
        Debug.Log("Recvied Shove RPC, IsOwner = " + IsOwner);
        
    }

    private void ResetShove()
    {
        ableToShove = true;
    }
}
