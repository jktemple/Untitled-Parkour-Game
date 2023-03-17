using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class InGameMenu : MonoBehaviour
{
    public void LoadScene(int number)
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(number);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Worked");
    }

}
