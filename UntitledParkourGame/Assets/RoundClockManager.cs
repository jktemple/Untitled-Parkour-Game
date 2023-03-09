using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class RoundClockManager : MonoBehaviour
{

    VirusTagGameModeManager mode;
    public TextMeshProUGUI t;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mode == null)
            mode = FindObjectOfType<VirusTagGameModeManager>();
        int time = (int) mode.currentTime.Value;
        if(time < 30) t.color = Color.red;
        else t.color = Color.black;
        t.SetText(time.ToString());
    }
}
