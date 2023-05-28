using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BoostingHitbox : NetworkBehaviour
{
    public bool playerInHitbox;

    private void Start()
    {
        playerInHitbox = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsOwner) return;
        if (other.gameObject.GetComponentInChildren<PlayerMovement>() != null)
        {
            playerInHitbox = true;
            Debug.Log("playerInHitbox true");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;
        if (other.gameObject.GetComponentInChildren<PlayerMovement>() != null)
        {
            playerInHitbox = false;
            Debug.Log("playerInHitbox false");
        }
    }
}
