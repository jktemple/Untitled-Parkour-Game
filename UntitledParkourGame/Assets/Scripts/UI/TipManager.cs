using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TipManager : MonoBehaviour
{
    // Start is called before the first frame update

    TextMeshProUGUI text;
    string playstationTip = "INFECTED players are red and cannot be seen through objects.\r\n\r\nTag/shove players using R2 or O\r\n\r\nFalling off the level as a non-infected player will make you infected";
    string xboxString = "INFECTED players are red and cannot be seen through objects.\r\n\r\nTag/shove players using RT or B\r\n\r\nFalling off the level as a non-infected player will make you infected";
    string switchString = "INFECTED players are red and cannot be seen through objects.\r\n\r\nTag/shove players using the ZR or A\r\n\r\nFalling off the level as a non-infected player will make you infected";
    string keyboardString = "INFECTED players are red and cannot be seen through objects.\r\n\r\nTag/shove players using the LMB\r\n\r\nFalling off the level as a non-infected player will make you infected";
    
    //PlayerInput input;
    void Start()
    {
        text= GetComponent<TextMeshProUGUI>();
        //input = GameObject.Find("Main Camera").GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Gamepad.current != null)
        {
            if(Gamepad.current.name == "DualShock4GamepadHID" || Gamepad.current.name == "DualSenseGamepadHID")
            {
                text.SetText(playstationTip);
            } else if (Gamepad.current.name == "SwitchProControllerHID")
            {
                text.SetText(switchString);
            }
            else
            {
                text.SetText(xboxString);
            }
        } else
        {
            text.SetText(keyboardString);
        }
    }
}
