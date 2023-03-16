using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Xml;
using Unity.Netcode;
using Cinemachine;

public class PlayerCam : NetworkBehaviour
{

    public GameObject thirdPersonMesh;
    public LayerMask invisible;
    // camera sensitivity
    public float mouseSensX;
    public float mouseSensY;
    public float gamepadSensX;
    public float gamepadSensY;

    public float quickTurnTime;
    public float fov;
    public float tiltTime;

    // player orientation
    public Transform orientation;
    public Transform camHolder;

    private PlayerControls inputs;

    // camera rotation
    float xRotation;
    float yRotation;

    int LayerInvisible;

    bool quickTurning = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
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
        if (!IsOwner) return;
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
        //Debug.Log("Quick Turning = " + quickTurning);
        //Debug.Log("X input: " + inputs.PlayerMovement.HorizontalLook.ReadValue<float>() + " Y Input: " + inputs.PlayerMovement.VerticalLook.ReadValue<float>());
        // updating the cam rotation idk whats rly happening here
        yRotation += mouseX;

        xRotation -= mouseY;
        // can't look up or down more than 90 degrees
        xRotation = Mathf.Clamp(xRotation, -90f, 55f);

        if (inputs.PlayerMovement.QuickTurn.triggered)
        {
            yRotation += 180;
            //Debug.Log(inputs.PlayerMovement.HorizontalLook.ReadValue<float>());
            if (inputs.PlayerMovement.HorizontalLook.ReadValue<float>() < 0f) DoQuickTurn(-180f);
            else DoQuickTurn(180f);
        }
        else if (!quickTurning)
        {
            // rotate cam and orientation
            
            camHolder.rotation = Quaternion.Euler(xRotation, yRotation, camHolder.eulerAngles.z);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        
    }

    public void DoFov(float endValue)
    {
        //Debug.Log(GameObject.Find("Main Camera").GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView);
        StopCoroutine(nameof(FOVChange));
        StartCoroutine(FOVChange(GameObject.Find("Main Camera").GetComponent<CinemachineVirtualCamera>(), endValue, 0.25f));
    }

    public void AddToFov(float valueAdded)
    {
        StopCoroutine(nameof(FOVChange));
        StartCoroutine(FOVChange(GameObject.Find("Main Camera").GetComponent<CinemachineVirtualCamera>(), fov + valueAdded, 0.25f));
    }

    public void ResetFov()
    {
        //Debug.Log(GameObject.Find("Main Camera").GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView);
        StopCoroutine(nameof(FOVChange));
        StartCoroutine(FOVChange(GameObject.Find("Main Camera").GetComponent<CinemachineVirtualCamera>(), fov, 0.25f));
    }

    public void DoTilt(float endAngle)
    {

        StopCoroutine(nameof(CameraTilt));
        StartCoroutine(CameraTilt(endAngle, tiltTime));  
    }

    IEnumerator CameraTilt(float endAngle, float time)
    {
        float startRotation = camHolder.eulerAngles.z;
        float t = 0.0f;
        while(t<time)
        {
            t += Time.deltaTime;
            startRotation = Mathf.LerpAngle(startRotation, endAngle, t / time);
            camHolder.eulerAngles = new Vector3(camHolder.eulerAngles.x, camHolder.eulerAngles.y, startRotation);
            yield return null;
        }
    }

    void DoQuickTurn(float rotation)
    {
        StopAllCoroutines();
        StartCoroutine(Rotate(quickTurnTime, rotation));
    }

    IEnumerator FOVChange(CinemachineVirtualCamera cam, float endValue, float time)
    {
        // to access the class field 'fov' use 'this.fov'
        float fov = cam.m_Lens.FieldOfView;
        //float angle = Mathf.Abs((fov / 2) - fov);
        float t = 0.0f;
        while (t < time)
        {
            fov = Mathf.LerpAngle(fov, endValue, 20*Time.deltaTime);
            cam.m_Lens.FieldOfView = fov;
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Rotate(float duration, float rotation)
    {
        quickTurning= true;
        float startRotation = camHolder.eulerAngles.y;
        float endRotation = startRotation + rotation;
        float t = 0.0f;
        while(t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            camHolder.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, camHolder.eulerAngles.z);
            orientation.eulerAngles = new Vector3(orientation.eulerAngles.x, yRotation, orientation.eulerAngles.z);
            yield return null;
        }
        quickTurning= false;
    }




}
