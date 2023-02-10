using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoveToSpawn : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!IsOwner) return;
        transform.position = GameObject.Find("Spawn Point").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
