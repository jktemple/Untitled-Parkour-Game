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
            newMessage = "Welcome!\nTo move, use <sprite=0><sprite=1><sprite=2><sprite=3>";
        }
        else if(this.gameObject.name == "Message (1)")
        {
            newMessage = "Oh boy, a gap!\nJump over it with\n<sprite=9> !";
        }
        else if (this.gameObject.name == "Message (2)")
        {
            newMessage = "A bigger Gap means you need more Speed!\nSprint with <sprite=4>!";
        }
        else if (this.gameObject.name == "Message (3)")
        {
            newMessage = "Watch Your Head!\nDon't worry, you can just walk under this one.";
        }
        else if (this.gameObject.name == "Message (4)")
        {
            newMessage = "Rather small, huh?\nJust slide with\n<sprite=5> when moving!";
        }
        else if (this.gameObject.name == "Message (5)")
        {
            newMessage = "Jump towards the wall at an angle to wallrun!";
        }
        else if (this.gameObject.name == "Message (6)")
        {
            newMessage = "You can chain wallruns into each other by jumping across!";
        }
        else if (this.gameObject.name == "Message (7)")
        {
            newMessage = "Hold forward into a wall to start climbing up it!";
        }
        else if (this.gameObject.name == "Message (8)")
        {
            newMessage = "Do a Quickturn with <sprite=8> while climbing up a wall and then jump! (don't move when turning or jumping)";
        }
        else if (this.gameObject.name == "Message (9)")
        {
            newMessage = "You can even Quickturn then jump during a wallrun\nto jump away from the wall!";
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
