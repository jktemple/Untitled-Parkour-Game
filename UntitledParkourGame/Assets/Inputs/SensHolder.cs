using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SensHolder : MonoBehaviour
{
    // Start is called before the first frame update

    public float mouseSense;
    public float padSense;
    Slider mouseSlider;
    Slider gamepadSlider;
    void Start()
    {
        mouseSlider = GameObject.Find("Mouse Sensitivity").GetComponent<Slider>();
        gamepadSlider = GameObject.Find("Controller Sensitivity").GetComponent<Slider>();
        if (mouseSlider != null)
        {
            mouseSlider.value = mouseSense;
            mouseSlider.onValueChanged.AddListener(delegate { MouseSensitivityUpdate(); });
        }
        if (gamepadSlider != null)
        {
            gamepadSlider.value = padSense;
            gamepadSlider.onValueChanged.AddListener(delegate { GamepadSensitivityUpdate(); });
        }


    }

    void MouseSensitivityUpdate()
    {
        mouseSense = mouseSlider.value;
    }

    void GamepadSensitivityUpdate()
    {
        padSense = gamepadSlider.value;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
