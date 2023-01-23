using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    // camera sensitivity
    public float sensX;
    public float sensY;

    // player orientation
    public Transform orientation;

    // camera rotation
    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        // curser locked to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        // curser invisible
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // updating the cam rotation idk whats rly happening here
        yRotation += mouseX;

        xRotation -= mouseY;
        // can't look up or down more than 90 degrees
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
