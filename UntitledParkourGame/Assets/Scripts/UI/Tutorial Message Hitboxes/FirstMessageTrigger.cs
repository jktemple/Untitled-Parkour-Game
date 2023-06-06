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
                newMessage = "Welcome!\nUse <sprite=0><sprite=1><sprite=2><sprite=3> to move and the mosue to look";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "Welcome!\nUse the left stick to move and the right stick to look";
            }
            else
            {
                newMessage = "Welcome!\nUse the left stick to move and the right stick to look";
            }

        }
        else if (this.gameObject.name == "Message (1)")
        {
            if (controlType == "Keyboard")
                newMessage = "Oh boy, a gap!\nJump over it with\n<sprite=9>!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "Oh boy, a gap!\nJump over it with\n<sprite=10>/<sprite=14>!";
            }
            else
            {
                newMessage = "Oh boy, a gap!\nJump over it with\n<sprite=20>/<sprite=24>!";
            }
        }
        else if (this.gameObject.name == "Message (2)")
        {
            if (controlType == "Keyboard")
                newMessage = "A bigger Gap means you need more Speed!\nSprint with <sprite=4>! (Drains stamina)";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "A bigger Gap means you need more Speed!\nSprint with <sprite=17>/<sprite=19>! (Drains stamina)";
            }
            else
            {
                newMessage = "A bigger Gap means you need more Speed!\nSprint with <sprite=27>/<sprite=29>! (Drains stamina)";
            }
        }
        else if (this.gameObject.name == "Message (3)")
        {
            if (controlType == "Keyboard")
                newMessage = "Watch Your Head!\nDon't worry, you can just walk under this one.";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "Watch Your Head!\nDon't worry, you can just walk under this one.";
            }
            else
            {
                newMessage = "Watch Your Head!\nDon't worry, you can just walk under this one.";
            }
        }
        else if (this.gameObject.name == "Message (4)")
        {
            if (controlType == "Keyboard")
                newMessage = "Rather small, huh?\nJust slide with\n<sprite=5> when moving!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "Rather small, huh?\nJust slide with\n<sprite=15> when moving!";
            }
            else
            {
                newMessage = "Rather small, huh?\nJust slide with\n<sprite=25> when moving!";
            }
        }
        else if (this.gameObject.name == "Message (5)")
        {
            if (controlType == "Keyboard")
                newMessage = "Jump towards the wall at an angle to wallrun! (Wallrunning recharges your stamina faster!)";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "Jump towards the wall at an angle to wallrun! (Wallrunning recharges your stamina faster!)";
            }
            else
            {
                newMessage = "Jump towards the wall at an angle to wallrun! (Wallrunning recharges your stamina faster!)";
            }
        }
        else if (this.gameObject.name == "Message (6)")
        {
            if (controlType == "Keyboard")
                newMessage = "You can chain wallruns into each other by jumping between them!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "You can chain wallruns into each other by jumping between them!";
            }
            else
            {
                newMessage = "You can chain wallruns into each other by jumping between them!";
            }
        }
        else if (this.gameObject.name == "Message (7)")
        {
            if (controlType == "Keyboard")
                newMessage = "Hold forward into a wall to start climbing up it! (Also recharges stamina)";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "Hold forward into a wall to start climbing up it! (Also recharges stamina)";
            }
            else
            {
                newMessage = "Hold forward into a wall to start climbing up it! (Also recharges stamina)";
            }
        }
        else if (this.gameObject.name == "Message (8)")
        {
            if (controlType == "Keyboard")
                newMessage = "Do a Quickturn with <sprite=8> while climbing up a wall and then jump! (don't move when turning or jumping)";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "Do a Quickturn with <sprite=11>/<sprite=18> while climbing up a wall and then jump! (don't move when turning or jumping)";
            }
            else
            {
                newMessage = "Do a Quickturn with <sprite=21>/<sprite=28> while climbing up a wall and then jump! (don't move when turning or jumping)";
            }
        }
        else if (this.gameObject.name == "Message (9)")
        {
            if (controlType == "Keyboard")
                newMessage = "You can even Quickturn then jump during a wallrun\nto jump away from the wall!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "You can even Quickturn then jump during a wallrun\nto jump away from the wall!";
            }
            else
            {
                newMessage = "You can even Quickturn then jump during a wallrun\nto jump away from the wall!";
            }
        }
        else if (this.gameObject.name == "Message (10)")
        {
            if (controlType == "Keyboard")
                newMessage = "Congrats on finishing the tutorial course!";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "Congrats on finishing the tutorial course!";
            }
            else
            {
                newMessage = "Congrats on finishing the tutorial course!";
            }
        }
        else if (this.gameObject.name == "Message (11)")
        {
            if (controlType == "Keyboard")
                newMessage = "Hold <sprite=7> while touching a wall to wallgrab. You can use this to precisely aim and then jump (Drains stamina)";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "Hold <sprite=17> while touching a wall to wallgrab. You can use this to precisely aim and then jump (Drains stamina)";
            }
            else
            {
                newMessage = "Hold <sprite=27> while touching a wall to wallgrab. You can use this to precisely aim and then jump (Drains stamina)";
            }
        }
        else if (this.gameObject.name == "Message (12)")
        {
            if (controlType == "Keyboard")
                newMessage = "Jump on top of another player's head to boost jump off of them! (Aim up to get more height)";
            else if (controller == "DualSenseGamepadHID" || controller == "DualShock4GamepadHID")
            {
                newMessage = "Jump on top of another player's head to boost jump off of them! (Aim up to get more height)";
            }
            else
            {
                newMessage = "Jump on top of another player's head to boost jump off of them! (Aim up to get more height)";
            }
        }
        else if (this.gameObject.name == "Message (13)")
        {
            newMessage = "press f1 to respawn.";
        }
        else
        {
            newMessage = "...";
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<PlayerMovement>() != null)
        {
            TutorialMessage.changeMessage(newMessage);
            TutorialMessage.showMessage();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<PlayerMovement>() != null)
        {
            TutorialMessage.hideMessage();
        }
    }

}
