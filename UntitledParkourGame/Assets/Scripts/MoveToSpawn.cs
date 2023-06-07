using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoveToSpawn : NetworkBehaviour
{
    GameObject[] spawnPoints;
    // Start is called before the first frame update
    public NetworkVariable<Vector3> spawnPointTransfrom = new NetworkVariable<Vector3>();
    public NetworkVariable<bool> moveToSpawn = new NetworkVariable<bool>();
    private bool ableToMove;
    void Start()
    {
        
        if(!IsOwner) return;
        transform.position = GameObject.Find("Spawn Point").transform.position;
        
        //spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        ableToMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        if(moveToSpawn.Value && ableToMove) {
            ableToMove= false;
            MovetoSpawnPoint();
            ResetMoveToSpawnServerRPC();
            Invoke(nameof(ResetAbleToMove), 10f);
            Debug.Log("Moving to spawn");
        }
    }

    public void MovetoSpawnPoint()
    {
        if (!IsOwner) return;
        if (spawnPointTransfrom.Value == Vector3.zero)
        {
            transform.position = GameObject.Find("Spawn Point").transform.position;
        }
        else
        {
            transform.position = spawnPointTransfrom.Value;
        }
        GetComponent<PlayerMovement>().ResetStamina();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    [ServerRpc]
    public void ResetMoveToSpawnServerRPC()
    {
        moveToSpawn.Value = false;
    }

    void ResetAbleToMove()
    {
        ableToMove = true;
    }
}
