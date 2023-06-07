using System;
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
    public GameObject reticle;
    public GameObject hud;
    public GameObject networkManagerObject;
    public static bool isPaused;
   // public string playerName = string.Empty;
    private bool showingScore;

    void Start()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("New Tutorial Level"))
        {
            theMenu.SetActive(false);
        }
        if(scoreBoard!=null)
        scoreBoard.SetActive(false);
        reticle.SetActive(false);
        if(hud!=null) hud.SetActive(false);
        Application.targetFrameRate = 120;
        if (LobbyManager.Instance != null)
        LobbyManager.Instance.OnGameStarted += LobbyManager_OnGameStarted;
    }

    private void LobbyManager_OnGameStarted(object sender, EventArgs e)
    {
        reticle.SetActive(true);
        hud.SetActive(true);
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

    public void ShowScore()
    {
        if(scoreBoard==null) return;
        scoreBoard.SetActive(true);
        showingScore= true;
    }

    public void HideScore()
    {
        if(scoreBoard==null) return;
        scoreBoard.SetActive(false);
        showingScore = false;
        GameObject go = GameObject.Find("GameOverUI");
        if(go != null) { go.SetActive(false); }
        GameObject go1 = GameObject.Find("RoundOverUI");
        if (go1 != null) { go1.SetActive(false); }
    }

    public void PauseGame()
    {
        if(isPaused) { return; }
        theMenu.SetActive(true);
        //Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        reticle.SetActive(false);
    }

    public void ResumeGame() {
        if(!isPaused) { return; }
        theMenu.SetActive(false);
        //Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        reticle.SetActive(true);
        GameObject go = GameObject.Find("GameOverUI");
        if (go != null) { go.SetActive(false); }
        GameObject go1 = GameObject.Find("RoundOverUI");
        if (go1 != null) { go1.SetActive(false); }
    }

    public void QuitGame()
    {
        Destroy(networkManagerObject);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Main Menu");
        Debug.Log("Quit");
    }
}
