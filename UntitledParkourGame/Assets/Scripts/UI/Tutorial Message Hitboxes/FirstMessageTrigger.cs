using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMessageTrigger : MonoBehaviour
{

    public TutorialMessages TutorialMessage;

    private void OnTriggerEnter(Collider other)
    {
        TutorialMessage.changeMessage("FIRST MESSAGE");
        TutorialMessage.showMessage();
    }

    private void OnTriggerExit(Collider other)
    {
        TutorialMessage.hideMessage();
    }

}
