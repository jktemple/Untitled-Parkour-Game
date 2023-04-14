using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;



public class RelayManagerUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeField;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private int numConnections;
    InGameMenuBehaviors menu;

    private void Awake()
    {
        // lambda expressions , details in codemonkey's "what are delegates"
        hostButton.onClick.AddListener(() => {
            CreateRelay();
            menu = FindObjectOfType<InGameMenuBehaviors>();
            menu.ResumeGame();
        });

        clientButton.onClick.AddListener(() => {
            JoinRelay(codeField.text);
            menu = FindObjectOfType<InGameMenuBehaviors>();
            menu.ResumeGame();
        });
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(numConnections);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            codeField.text = joinCode;
            RelayServerData relayServer = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServer);
            NetworkManager.Singleton.StartHost();

        } catch (RelayServiceException e)
        {
            Debug.Log(e.ToString());
        }
    }

    private async void JoinRelay(string code)
    {
        try
        {
            Debug.Log("Joining Relay with " + code);
           JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);

            RelayServerData relayServer = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServer);
            NetworkManager.Singleton.StartClient();
        } catch(RelayServiceException e) { 
            Debug.Log(e.ToString());
        }
    }


}
