using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoostingHitbox : MonoBehaviour
{
    public bool playerInHitbox;

    private void Start()
    {
        playerInHitbox = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<PlayerMovement>() != null)
        {
            playerInHitbox = true;
            Debug.Log("playerInHitbox true");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<PlayerMovement>() != null)
        {
            playerInHitbox = false;
            Debug.Log("playerInHitbox false");
        }
    }
}
