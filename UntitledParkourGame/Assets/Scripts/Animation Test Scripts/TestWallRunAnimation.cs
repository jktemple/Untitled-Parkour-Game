using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWallRunAnimation : MonoBehaviour
{
    [SerializeField]
    private Vector3 movementDirection;
    [SerializeField]
    private float loopTime;
    private Vector3 startingLoc;
    private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
       startingLoc = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position += movementDirection *Time.deltaTime;
        timer += Time.deltaTime;
        if(timer >= loopTime){
            timer = 0;
            transform.position = startingLoc;
        }
    }
}
