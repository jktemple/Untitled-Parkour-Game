using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VirusTagGameModeManager : NetworkBehaviour
{
    public float roundLength;
    private float roundLengthTimer;
    private bool roundOngoing;
    [SerializeField] private Button startRoundButton;

    public GameObject[] spawnPoints;
   
    // Start is called before the first frame update
    void Start()
    {
        //spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        startRoundButton.onClick.AddListener(() => { StartRound(); });
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.f5Key.wasPressedThisFrame)
        {
            StartRound();
        }

        if (roundLengthTimer > 0)
        {
            roundLengthTimer -= Time.deltaTime;
        } else if(roundOngoing && roundLengthTimer < 0)
        {
            EndRound();
        }
    }

    public void MovePlayersToSpawnPoints()
    {
        if (!IsServer) { return; }
        MoveToSpawn[] moveList = FindObjectsOfType<MoveToSpawn>();
        for (int i = 0; i < moveList.Length; i++)
        {
            if (i < spawnPoints.Length) 
            {
                moveList[i].spawnPointTransfrom.Value = spawnPoints[i].transform.position;
            }
            else
            {
                moveList[i].spawnPointTransfrom.Value = spawnPoints[0].transform.position;
            }
            moveList[i].moveToSpawn.Value = true;
        }
    }
    

    public void StartRound()
    {
        if(roundOngoing) { return; }
        AssignInfectedPlayer();
        MovePlayersToSpawnPoints();
        roundLengthTimer = roundLength;  
        roundOngoing = true;
    }

    public void EndRound()
    {
        //do something'
        roundOngoing= false;
        MovePlayersToSpawnPoints();
    }

    public void AssignInfectedPlayer()
    {
        if (!IsServer) return;
        //search for all the players in the current scene
        //randomly chose one to make infected
        Shoving[] playerList = FindObjectsOfType<Shoving>();
        foreach (Shoving sho in playerList)
        {
            sho.infected.Value = false;
        }
        int randIndex = Random.Range(0, playerList.Length);
        playerList[randIndex].infected.Value= true;
    }
}
