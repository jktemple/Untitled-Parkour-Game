using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AssignInfected : NetworkBehaviour
{
    // Start is called before the first frame update

    public void AssignInfectedPlayer()
    {
        if (!IsServer) return;
        //search for all the players in the current scene
        //randomly chose one to make infected
        Shoving[] playerList = FindObjectsOfType<Shoving>();
        int randIndex = Random.Range(0, playerList.Length);
        playerList[randIndex].infected.Value= true;
    }
}
