using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OutofWorldRepawn : NetworkBehaviour
{

    [SerializeField] Rigidbody rb;
    [SerializeField] float threshhold;
    Shoving shovescript;
    //MoveToSpawn spawnScript;

  
    void FixedUpdate()
    {
        if (!IsServer) return;
        if (transform.position.y < threshhold)
        {
           // GetComponent<MoveToSpawn>().MovetoSpawnPoint();
            GetComponent<MoveToSpawn>().moveToSpawn.Value = true;
            shovescript = GetComponent<Shoving>();
            //If not infected, become infected
            if (!shovescript.infected.Value)
            {
                shovescript.infected.Value = true;
            }
        }
    }
}
