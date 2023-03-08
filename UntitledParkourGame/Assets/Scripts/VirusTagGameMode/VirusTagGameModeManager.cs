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

    public NetworkVariable<float> currentTime = new NetworkVariable<float>();
    [SerializeField] private Button startRoundButton;

    public GameObject[] spawnPoints;
    Shoving[] shoveList;
    // Start is called before the first frame update
    void Start()
    {
        //spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        startRoundButton.onClick.AddListener(() => { if(!roundOngoing) StartRound(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        if(Keyboard.current.f5Key.wasPressedThisFrame && !roundOngoing)
        {
            StartRound();
        }

        if (roundLengthTimer > 0 && roundOngoing)
        {
            roundLengthTimer -= Time.deltaTime;
            if (allInfected())
            {
                EndRound();
            }
        } else if(roundOngoing && roundLengthTimer < 0)
        {
            EndRound();
        }
        currentTime.Value = roundLengthTimer;
        
    }

    private bool allInfected()
    {
        foreach(Shoving s in shoveList)
        {
            if(!s.infected.Value)
                return false;
        }
        return true;
    }

    public void MovePlayersToSpawnPoints()
    {
        if (!IsServer) { return; }
        MoveToSpawn[] moveList = FindObjectsOfType<MoveToSpawn>();
        Shuffle(spawnPoints);
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

    public void Shuffle(GameObject[] moveList) 
    {
          for (int i = 0; i < moveList.Length - 1; i++) 
          {
              int rnd = Random.Range(i,  moveList.Length);
              GameObject tempGO = moveList[rnd];
              moveList[rnd] = moveList[i];
              moveList[i] = tempGO;
          }
    }
    

    public void StartRound()
    {
        if(roundOngoing) { return; }
        AssignInfectedPlayer();
        MovePlayersToSpawnPoints();
        roundLengthTimer = roundLength;  
        roundOngoing = true;
        shoveList = FindObjectsOfType<Shoving>();
    }

    public void EndRound()
    {
        //do something'
        roundOngoing= false;
        MovePlayersToSpawnPoints();
        currentTime.Value = 0;
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
        playerList[randIndex].infected.Value = true;
    }
}
