using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstMessageTrigger : MonoBehaviour
{

    public TutorialMessages TutorialMessage;
    public Camera camera;
    private string controlType;
    private string controller;
    private string newMessage;

    private void Start()
    {
        controlType = camera.GetComponent<PlayerInput>().currentControlScheme;
        controller = "nothin";
        newMessage = "unset";
        
    }

    private void Update()
    {
        if(Gamepad.current != null)
        {
            controller = Gamepad.current.name;
        }
        
        controlType = camera.GetComponent<PlayerInput>().currentControlScheme;

        if (this.gameObject.name == "Message")
        {
            if (controlType == "Keyboard")
                newMessage = "Welcome!\nTo move, use <sprite=0><sprite=1><sprite=2><sprite=3>";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }

        }
        else if (this.gameObject.name == "Message (1)")
        {
            if (controlType == "Keyboard")
                newMessage = "Oh boy, a gap!\nJump over it with\n<sprite=9> !";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }
        }
        else if (this.gameObject.name == "Message (2)")
        {
            if (controlType == "Keyboard")
                newMessage = "A bigger Gap means you need more Speed!\nSprint with <sprite=4>!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }
        }
        else if (this.gameObject.name == "Message (3)")
        {
            if (controlType == "Keyboard")
                newMessage = "Watch Your Head!\nDon't worry, you can just walk under this one.";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }
        }
        else if (this.gameObject.name == "Message (4)")
        {
            if (controlType == "Keyboard")
                newMessage = "Rather small, huh?\nJust slide with\n<sprite=5> when moving!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }
        }
        else if (this.gameObject.name == "Message (5)")
        {
            if (controlType == "Keyboard")
                newMessage = "Jump towards the wall at an angle to wallrun!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }
        }
        else if (this.gameObject.name == "Message (6)")
        {
            if (controlType == "Keyboard")
                newMessage = "You can chain wallruns into each other by jumping across!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }
        }
        else if (this.gameObject.name == "Message (7)")
        {
            if (controlType == "Keyboard")
                newMessage = "Hold forward into a wall to start climbing up it!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }
        }
        else if (this.gameObject.name == "Message (8)")
        {
            if (controlType == "Keyboard")
                newMessage = "Do a Quickturn with <sprite=8> while climbing up a wall and then jump! (don't move when turning or jumping)";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }
        }
        else if (this.gameObject.name == "Message (9)")
        {
            if (controlType == "Keyboard")
                newMessage = "You can even Quickturn then jump during a wallrun\nto jump away from the wall!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }
        }
        else if (this.gameObject.name == "Message (10)")
        {
            if (controlType == "Keyboard")
                newMessage = "Congrats on finishing the tutorial course!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "ps4";
            }
            else
            {
                newMessage = "xbox";
            }
        }
        else
        {
            newMessage = "...";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TutorialMessage.changeMessage(newMessage);
        TutorialMessage.showMessage();
    }

    private void OnTriggerExit(Collider other)
    {
        TutorialMessage.hideMessage();
    }

}
