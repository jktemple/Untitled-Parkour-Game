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
    public float sensX;
    public float sensY;

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
        Debug.Log("Current layer: " + thirdPersonMesh.layer);
        SetLayerRecursively(thirdPersonMesh, LayerInvisible);

    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

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
        // get mouse input
        float mouseX = inputs.PlayerMovement.HorizontalLook.ReadValue<float>() * Time.deltaTime * sensX;
        float mouseY = inputs.PlayerMovement.VerticalLook.ReadValue<float>() * Time.deltaTime * sensY;

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
