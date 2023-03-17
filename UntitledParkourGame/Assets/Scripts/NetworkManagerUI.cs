using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        // lambda expressions , details in codemonkey's "what are delegates"
        if(serverButton!=null)
        serverButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
        
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
        });
        if(clientButton!=null)
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }

}
