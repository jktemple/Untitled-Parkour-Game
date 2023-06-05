using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialMessages : MonoBehaviour
{
    public GameObject backgroundImage;
    private TextMeshProUGUI message;

    // Start is called before the first frame update
    void Start()
    {
            message = backgroundImage.GetComponent<TextMeshProUGUI>();
            Debug.Log("Icon == " + message.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
