using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMessageTrigger : MonoBehaviour
{

    public TutorialMessages TutorialMessage;
    private string newMessage;

    private void Start()
    {
        newMessage = "unset";
        if(this.gameObject.name == "Message")
        {
            newMessage = "Welcome!\nTo move, use the left stick / WASD keys";
        }
        else if(this.gameObject.name == "Message (1)")
        {
            newMessage = "Oh boy, a gap!\nJump over it!\n(RB / R1 / R / Spacebar)";
        }
        else if (this.gameObject.name == "Message (2)")
        {
            newMessage = "Bigger Gap means More Speed!\nSprint!(LT / L2 / ZL / LeftShift)";
        }
        else if (this.gameObject.name == "Message (3)")
        {
            newMessage = "Watch Your Head!\nDon't worry, you can just walk under this one.";
        }
        else if (this.gameObject.name == "Message (4)")
        {
            newMessage = "Rather small, huh?\nJust slide!\n(LB / L1 / L / LeftCtrl while moving) ";
        }
        else if (this.gameObject.name == "Message (5)")
        {
            newMessage = "To run across a wall, just move into it after a jump.";
        }
        else if (this.gameObject.name == "Message (6)")
        {
            newMessage = "You can chain wallruns into each other by jumping across!";
        }
        else if (this.gameObject.name == "Message (7)")
        {
            newMessage = "Climbing up the wall is simple: just move/jump into the wall and move ahead.";
        }
        else if (this.gameObject.name == "Message (8)")
        {
            newMessage = "You can do a Quickturn!\n(Click Right Stick or Q)\nCareful, though: for best results, don't move when jumping off walls!";
        }
        else if (this.gameObject.name == "Message (9)")
        {
            newMessage = "You can even use the Quickturn to chain a wallrun into a wallclimb!";
        }
        else if (this.gameObject.name == "Message (10)")
        {
            newMessage = "Congrats on finishing the tutorial course!";
        }
        else
        {
            newMessage = "...";
        }
    }

    private void Update()
    {
        
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
