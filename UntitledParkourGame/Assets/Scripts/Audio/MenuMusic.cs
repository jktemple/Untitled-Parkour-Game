using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusic : MonoBehaviour
{
    // Start is called before the first frame update
    Scene tutorial;
    Scene bigmap;
    Scene smallmap;
    Scene texturedBigMap;
    Scene texturedSmallMap;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        tutorial = SceneManager.GetSceneByName("New Tutorial Level");
        bigmap = SceneManager.GetSceneByName("LobbyMap");
        smallmap = SceneManager.GetSceneByName("SmallMap");
        texturedBigMap = SceneManager.GetSceneByName("BigConTextured");
        texturedSmallMap = SceneManager.GetSceneByName("SmallConTextured");

        
    }

    // Update is called once per frame

    
}
