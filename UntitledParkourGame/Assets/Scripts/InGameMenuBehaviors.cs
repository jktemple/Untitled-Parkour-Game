using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameMenuBehaviors : MonoBehaviour
{
    public GameObject theMenu;
    public GameObject scoreBoard;
    public static bool isPaused;
    private bool showingScore;
    void Start()
    {
        theMenu.SetActive(false);
        if(scoreBoard!=null)
        scoreBoard.SetActive(false);

        Application.targetFrameRate = 120;
    }
    void Update()
    {
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame))
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

        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if(showingScore)
            {
                HideScore();
            } else
            {
                ShowScore();
            }
        }
    }

    void ShowScore()
    {
        if(scoreBoard==null) return;
        scoreBoard.SetActive(true);
        showingScore= true;
    }

    void HideScore()
    {
        if(scoreBoard==null) return;
        scoreBoard.SetActive(false);
        showingScore = false;
    }

    public void PauseGame()
    {
        if(isPaused) { return; }
        theMenu.SetActive(true);
        //Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame() {
        if(!isPaused) { return; }
        theMenu.SetActive(false);
        //Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Main Menu");
        Debug.Log("Quit");
    }
}
