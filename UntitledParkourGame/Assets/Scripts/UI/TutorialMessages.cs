using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMessages : MonoBehaviour
{
    public Image backgroundImage;
    public TextMeshProUGUI message;
    private string whatMessage;

    // Start is called before the first frame update
    void Start()
    {
        backgroundImage.enabled = false;
        message.enabled = false;
        whatMessage = "default";
    }

    public void changeMessage(string newMessage)
    {
        message.text = newMessage;
    }
    public void showMessage()
    {
        backgroundImage.enabled = true;
        message.enabled = true;
    }

    public void hideMessage()
    {
        backgroundImage.enabled = false;
        message.enabled = false;
    }
}
