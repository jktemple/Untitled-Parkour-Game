using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        //var gamepad = Gamepad.current;
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            restart();
        } else if (Gamepad.current != null)
        {
            if(Gamepad.current.startButton.wasPressedThisFrame)
            restart();
        }
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}