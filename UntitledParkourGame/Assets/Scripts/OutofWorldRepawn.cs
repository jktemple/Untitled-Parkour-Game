using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OutofWorldRepawn : MonoBehaviour
{

    [SerializeField] Rigidbody rb;
    [SerializeField] float threshhold;
    Shoving shovescript;

  
    void FixedUpdate()
    {
        if (transform.position.y < threshhold)
        {
            GetComponent<MoveToSpawn>().MovetoSpawnPoint();
            rb.velocity = Vector3.zero;
            //If not infected, become infected
            /*if (!shovescript.infected.Value)
            {
                shovescript.infected.Value = true;
            }*/
        }
    }
}
