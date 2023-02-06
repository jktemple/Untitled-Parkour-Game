using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{

    public GameObject thirdPersonMesh;
    public LayerMask invisible;
    // camera sensitivity
    public float mouseSensX;
    public float mouseSensY;
    public float gamepadSensX;
    public float gamepadSensY;

    // player orientation
    public Transform orientation;
    public Transform camHolder;

    private PlayerControls inputs;

    // camera rotation
    float xRotation;
    float yRotation;

    int LayerInvisible;
    // Start is called before the first frame update
    void Start()
    {
        // curser locked to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        // curser invisible
        Cursor.visible = false;

        inputs = new PlayerControls();
        inputs.PlayerMovement.Enable();
         LayerInvisible = LayerMask.NameToLayer("invisible");
        thirdPersonMesh.layer = LayerInvisible;
       
        SetLayerRecursively(thirdPersonMesh, LayerInvisible);
        var gamepad = Gamepad.current;
        var mouse = Mouse.current;
        Debug.Log("current gamepad " + gamepad);
        Debug.Log("current mouse " + mouse);

    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }
        Debug.Log("Current layer: " + obj.layer);
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var gamepad = Gamepad.current;

        // get mouse input

        float mouseX;
        float mouseY; 
        if (null == gamepad)
        {
           mouseX = inputs.PlayerMovement.HorizontalLook.ReadValue<float>() * Time.deltaTime * mouseSensX;
           mouseY = inputs.PlayerMovement.VerticalLook.ReadValue<float>() * Time.deltaTime * mouseSensY;
        } else
        {
            mouseX = inputs.PlayerMovement.HorizontalLook.ReadValue<float>() * Time.deltaTime * gamepadSensX;
            mouseY = inputs.PlayerMovement.VerticalLook.ReadValue<float>() * Time.deltaTime * gamepadSensY;
        }
        // updating the cam rotation idk whats rly happening here
        yRotation += mouseX;

        xRotation -= mouseY;
        // can't look up or down more than 90 degrees
        xRotation = Mathf.Clamp(xRotation, -90f, 55f);

        // rotate cam and orientation
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }


}
