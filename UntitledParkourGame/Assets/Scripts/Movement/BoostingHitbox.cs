using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BoostingHitbox : NetworkBehaviour
{
    public bool canBoost;

    private void Start()
    {
        canBoost = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsOwner) return;
        if (other.gameObject.GetComponentInChildren<PlayerMovement>() != null)
        {
            canBoost = true;
            Debug.Log("canBoost true");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;
        if (other.gameObject.GetComponentInChildren<PlayerMovement>() != null)
        {
            canBoost = false;
            Debug.Log("canBoost false");
        }
    }
}
