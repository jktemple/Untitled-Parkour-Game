
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/* This Script Modified from a Code Monkey Example Project 
 * https://www.youtube.com/watch?v=-KDlEBfCBiU
 * 
 */
public class EditPlayerName : MonoBehaviour
{
    
    public static EditPlayerName Instance { get; private set; }

    public event EventHandler OnNameChanged;

    [SerializeField] private TextMeshProUGUI playerNameText;

    private string playerName = "ParkourPlayer";
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;

        GetComponent<Button>().onClick.AddListener(() => {
            InputWindowUI.Show_Static("Player Name", playerName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
            () => {
                // Cancel
            },
            (string newName) => {
                playerName = newName;

                playerNameText.text = playerName;

                OnNameChanged?.Invoke(this, EventArgs.Empty);
            });
        });

        playerNameText.text = playerName;
    }
    void Start()
    {
        OnNameChanged += EditPlayerName_OnNameChanged;
    }

    private void EditPlayerName_OnNameChanged(object sender, EventArgs e)
    {
        LobbyManager.Instance.UpdatePlayerName(GetPlayerName());
    }

    public string GetPlayerName()
    {
        return playerName;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
