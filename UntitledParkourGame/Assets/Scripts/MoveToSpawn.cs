using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = GameObject.Find("Spawn Point").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
