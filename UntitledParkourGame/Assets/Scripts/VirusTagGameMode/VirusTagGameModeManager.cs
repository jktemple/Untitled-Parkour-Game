using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VirusTagGameModeManager : NetworkBehaviour
{
    public float roundLength;
    public int maxScore;
    private float roundLengthTimer;
    private bool roundOngoing;
    private bool gameOngoing;

    public NetworkVariable<float> currentTime = new NetworkVariable<float>();
    [SerializeField] private Button startRoundButton;

    public GameObject[] spawnPoints;
    Shoving[] shoveList;
    Queue<Shoving> scoreQueue;
    Stack<Shoving> orderStack;
   
    // Start is called before the first frame update
    void Start()
    {
        //spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        startRoundButton.onClick.AddListener(() => {
            if (!gameOngoing) StartGame();
            else if (!roundOngoing) StartRound();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        if(Keyboard.current.f5Key.wasPressedThisFrame)
        {
            if(!gameOngoing && !roundOngoing) StartGame();
            else if(gameOngoing && !roundOngoing) StartRound();
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
        bool r = true;
        foreach(Shoving s in shoveList)
        {
            if (!s.infected.Value) 
            {
                r = false;
            }
            else if(s.infected.Value && !scoreQueue.Contains(s))
            {
                scoreQueue.Enqueue(s);
            }
        }
        return r;
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

    void Shuffle(GameObject[] moveList) 
    {
          for (int i = 0; i < moveList.Length - 1; i++) 
          {
              int rnd = Random.Range(i,  moveList.Length);
              GameObject tempGO = moveList[rnd];
              moveList[rnd] = moveList[i];
              moveList[i] = tempGO;
          }
    }

    void Shuffle(Shoving[] moveList)
    {
        for (int i = 0; i < moveList.Length - 1; i++)
        {
            int rnd = Random.Range(i, moveList.Length);
            Shoving tempGO = moveList[rnd];
            moveList[rnd] = moveList[i];
            moveList[i] = tempGO;
        }
    }


    private void StartGame()
    {
        shoveList = FindObjectsOfType<Shoving>();
        Shuffle(shoveList);
        orderStack = new Stack<Shoving>(shoveList);
        foreach (Shoving s in shoveList)
        {
            s.score.Value = 0;
        }
        gameOngoing = true;
        StartRound();
    }

    private void EndGame()
    {
        gameOngoing = false;
    }
    public void StartRound()
    {
        if(roundOngoing) { return; }
        if (orderStack.Count <= 0)
        {
            EndGame();
            return;
        }
        AssignInfectedPlayer();
        MovePlayersToSpawnPoints();
        roundLengthTimer = roundLength;  
        roundOngoing = true;
        //shoveList = FindObjectsOfType<Shoving>();
    }

    public void EndRound()
    {
        //do something'
        roundOngoing= false;
        MovePlayersToSpawnPoints();
        currentTime.Value = 0;
        AssignScores();
        scoreQueue.Clear();
        if(orderStack.Count <= 0)
        {
            EndGame();
        }
    }

    void AssignScores()
    {
        foreach(Shoving s in shoveList)
        {
            if(s.infected.Value == false)
            {
                s.score.Value += maxScore;
            }
        }
        int increment = maxScore / shoveList.Length;
        int count = 0;
        while (scoreQueue.Count > 0)
        {
           Shoving s = scoreQueue.Dequeue();
           s.score.Value += (count * increment);
           count++;
        }
    }

    public void AssignInfectedPlayer()
    {
        if (!IsServer) return;
        //search for all the players in the current scene
        //randomly chose one to make infected
        foreach (Shoving sho in shoveList)
        {
            sho.infected.Value = false;
        }
        if (orderStack.Count > 0)
        {
            Shoving s = orderStack.Pop();
            s.infected.Value = true;
            scoreQueue.Enqueue(s);
        }
    }
}
