using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameMenuBehaviors : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject theMenu;
    public static bool isPaused;

    void Start()
    {
        theMenu.SetActive(true);   
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        theMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame() {
        theMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
