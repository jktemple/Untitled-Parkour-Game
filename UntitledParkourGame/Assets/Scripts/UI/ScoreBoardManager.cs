using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    public TextMeshProUGUI t;

    Shoving[] playerList;

    class ShovingComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((Shoving) x).score.Value.CompareTo(((Shoving) y).score.Value);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerList = FindObjectsOfType<Shoving>();
        Array.Sort(playerList, new ShovingComparer());
        string s = "";
        foreach(Shoving shove in playerList)
        {
            s += "Player " + shove.playerNumber.Value + ": " + shove.score.Value + "\n";
        }
        t.SetText(s);
    }

    void SortByScore()
    {
        
    }

}
