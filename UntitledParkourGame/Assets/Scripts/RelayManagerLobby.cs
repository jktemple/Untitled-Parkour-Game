using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

public class RelayManagerLobby : MonoBehaviour
{
    public static RelayManagerLobby Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

    }
    
    public async Task<String> CreateRelay(int numConnections)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(numConnections);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayServer = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServer);
            NetworkManager.Singleton.StartHost();
            return joinCode;

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    public async void JoinRelay(string code)
    {
        try
        {
            Debug.Log("Joining Relay with " + code);
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);

            RelayServerData relayServer = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServer);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e.ToString());
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
