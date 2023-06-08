using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Xml;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.UI;

public class PlayerCam : NetworkBehaviour
{
    public GameObject thirdPersonMesh;
    public GameObject outlineMesh;
    public WallRunning wr;
    public PlayerMovement pm;
    public Shoving sh;
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

    //inputs
    private PlayerControls inputs;
    private PlayerInput playerInput;
    // camera rotation
    float xRotation;
    float yRotation;

    int LayerInvisible;
    int xrayHash;
    bool quickTurning = false;
    // Start is called before the first frame update

    //Sensitivity Sliders
    private SensHolder sensHolder;
    void Start()
    {
        xrayHash = LayerMask.NameToLayer("XRay");
        if (!IsOwner) 
        {
            return; 
        }
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
        playerInput = GameObject.Find("Main Camera").GetComponent<PlayerInput>();
        sensHolder = FindObjectOfType<SensHolder>();
        if(sensHolder != null )
        {
            mouseSensX = sensHolder.mouseSense;
            mouseSensY = sensHolder.mouseSense;
            gamepadSensY= sensHolder.padSense;
            gamepadSensX = sensHolder.padSense;
        }

        inputs.PlayerMovement.LookBehind.performed += LookBehind_performed;
        inputs.PlayerMovement.LookBehind.canceled += LookBehind_canceled;
    }
    private bool lookingBehind = false;
    private void LookBehind_canceled(InputAction.CallbackContext obj)
    {
        Debug.Log("LookBehind Canceled");
        lookingBehind = false;
    }

    private void LookBehind_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("LookBehind Preformed");
        lookingBehind= true;
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }
        //Debug.Log("Current layer: " + obj.layer);
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

    public void SetXRay(bool xray)
    {
        if (IsOwner) return;
        if(xray) 
        { 
            SetLayerRecursively(outlineMesh, LayerMask.NameToLayer("XRay")); 
        }
        else
        {
            SetLayerRecursively(outlineMesh, LayerMask.NameToLayer("Default"));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            if (sh.infected.Value == true && outlineMesh.layer == xrayHash) 
            {
                SetLayerRecursively(outlineMesh, LayerMask.NameToLayer("Default"));
            } else if(sh.infected.Value == false && outlineMesh.layer != xrayHash)
            {
                SetLayerRecursively(outlineMesh, LayerMask.NameToLayer("XRay"));
            }
            return; 
        }

        if (sensHolder != null)
        {
            mouseSensX = sensHolder.mouseSense;
            mouseSensY = sensHolder.mouseSense;
            gamepadSensY = sensHolder.padSense;
            gamepadSensX = sensHolder.padSense;
        }

        // get mouse input

        float mouseX;
        float mouseY; 
        if (playerInput.currentControlScheme == "Keyboard")
        {
           mouseX = inputs.PlayerMovement.HorizontalLook.ReadValue<float>() * Time.deltaTime * mouseSensX;
           mouseY = inputs.PlayerMovement.VerticalLook.ReadValue<float>() * Time.deltaTime * mouseSensY;
        } else
        {
            mouseX = inputs.PlayerMovement.HorizontalLook.ReadValue<float>() * Time.deltaTime * gamepadSensX;
            mouseY = inputs.PlayerMovement.VerticalLook.ReadValue<float>() * Time.deltaTime * gamepadSensY;
        }
        //Debug.Log("current control scheme = " + playerInput.currentControlScheme);
        //Debug.Log("Quick Turning = " + quickTurning);
        //Debug.Log("X input: " + inputs.PlayerMovement.HorizontalLook.ReadValue<float>() + " Y Input: " + inputs.PlayerMovement.VerticalLook.ReadValue<float>());
        // updating the cam rotation idk whats rly happening here
        yRotation += mouseX;

        xRotation -= mouseY;
        // can't look up or down more than 90 degrees
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        

        if (inputs.PlayerMovement.QuickTurn.triggered)
        {
            HandleQuickTurn();
        }
        else if (lookingBehind)
        {
            camHolder.rotation = Quaternion.Euler(camHolder.eulerAngles.z, 180+orientation.eulerAngles.y, camHolder.eulerAngles.z);
        }
        else if (!quickTurning)
        {
            // rotate cam and orientation
            
            camHolder.rotation = Quaternion.Euler(xRotation, yRotation, camHolder.eulerAngles.z);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        if (pm.wallrunning)
        {
            float temp = Quaternion.LookRotation(wr.GetWallNormal()).eulerAngles.y;
            Debug.Log("temp = " + temp);
        }
        
    }

    private void HandleQuickTurn()
    {
        if (!pm.wallrunning)
        {
            yRotation += 180;
            //Debug.Log(inputs.PlayerMovement.HorizontalLook.ReadValue<float>());
            if (inputs.PlayerMovement.HorizontalLook.ReadValue<float>() < 0f) DoQuickTurn(quickTurnTime,-180f);
            else DoQuickTurn(quickTurnTime, 180f);
        } else if (pm.wallrunning && wr.wallRight)
        {
            yRotation -= 90f;
            //DoQuickTurn(quickTurnTime*0.75f, -90f);
            float temp = Quaternion.LookRotation(wr.GetWallNormal()).eulerAngles.y;
            Debug.Log("temp = " + temp);
            DoTargetQuickTurn(quickTurnTime * 0.75f, temp);
        } else if(pm.wallrunning && wr.wallLeft)
        {
            yRotation += 90f;
            float temp = Quaternion.LookRotation(wr.GetWallNormal()).eulerAngles.y;
            Debug.Log("temp = " + temp);
            DoTargetQuickTurn(quickTurnTime * 0.75f, temp);
            //DoQuickTurn(quickTurnTime*0.75f, 90f);
        }
        pm.quickTurned = true;
    }

    public void DoFov(float endValue)
    {
        //Debug.Log(GameObject.Find("Main Camera").GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView);
        StopCoroutine(nameof(FOVChange));
        StartCoroutine(FOVChange(GameObject.Find("Main Camera").GetComponent<CinemachineVirtualCamera>(), endValue, 0.1f));
    }

    public void AddToFov(float valueAdded)
    {
        StopCoroutine(nameof(FOVChange));
        StartCoroutine(FOVChange(GameObject.Find("Main Camera").GetComponent<CinemachineVirtualCamera>(), fov + valueAdded, 0.1f));
    }

    public void ResetFov()
    {
        //Debug.Log(GameObject.Find("Main Camera").GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView);
        StopCoroutine(nameof(FOVChange));
        StartCoroutine(FOVChange(GameObject.Find("Main Camera").GetComponent<CinemachineVirtualCamera>(), fov, 0.1f));
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
            float tempRotation = Mathf.LerpAngle(startRotation, endAngle, t / time);
            camHolder.Rotate(0, 0, tempRotation-camHolder.eulerAngles.z);
            //Debug.Log("camHolder.eulerAngles.z = " + camHolder.eulerAngles.z);
            yield return null;
        }
    }

    void DoQuickTurn(float time, float rotation)
    {
        StopCoroutine(nameof(Rotate));
        StopCoroutine(nameof(RotatePlayerToTarget));
        StartCoroutine(Rotate(time, rotation));
    }

    void DoTargetQuickTurn(float time, float targetRoation)
    {
        StopCoroutine(nameof(RotatePlayerToTarget));
        StopCoroutine(nameof(Rotate));
        StartCoroutine(RotatePlayerToTarget(time, targetRoation));
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
        inputs.PlayerMovement.Disable();
        float startRotation = camHolder.eulerAngles.y;
        float endRotation = startRotation + rotation;
        float t = 0.0f;
        while(t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.LerpAngle(startRotation, endRotation, t / duration) % 360.0f;
            camHolder.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, camHolder.eulerAngles.z);
            orientation.eulerAngles = new Vector3(orientation.eulerAngles.x, yRotation, orientation.eulerAngles.z);
            yield return null;
        }
        inputs.PlayerMovement.Enable();
        quickTurning = false;
    }

    IEnumerator RotatePlayerToTarget(float duration, float targetRotation)
    {
        quickTurning = true;
        inputs.PlayerMovement.Disable();
        float startRotation = orientation.eulerAngles.y;
        float endRotation = targetRotation;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.LerpAngle(startRotation, endRotation, t / duration) % 360.0f;
            camHolder.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, camHolder.eulerAngles.z);
            orientation.eulerAngles = new Vector3(orientation.eulerAngles.x, yRotation, orientation.eulerAngles.z);
            yield return null;
        }
        inputs.PlayerMovement.Enable();
        quickTurning = false;
    }

    public bool isQuickTurning()
    {
        return quickTurning;
    }




}
