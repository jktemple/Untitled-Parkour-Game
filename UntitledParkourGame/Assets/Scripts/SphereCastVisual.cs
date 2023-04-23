using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCastVisual : MonoBehaviour
{

    public float presistTimer;
    private float currTime = 0;
    public float diameter;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(diameter, diameter, diameter);
    }

    // Update is called once per frame
    void Update()
    {
        if(currTime < presistTimer)
        {
            currTime += Time.deltaTime;
        } else
        {
            Destroy(gameObject);
        }
    }
}
