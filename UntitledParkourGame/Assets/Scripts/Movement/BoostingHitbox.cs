using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostingHitbox : MonoBehaviour
{
    public bool canBoost;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.GetComponentInChildren<PlayerMovement>() != null && other.gameObject.GetComponentInChildren<PlayerMovement>().boosting.Value)
        {
            canBoost = true;
            Debug.Log("canBoost true");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<PlayerMovement>() != null && other.gameObject.GetComponentInChildren<PlayerMovement>().boosting.Value)
        {
            canBoost = false;
            Debug.Log("canBoost false");
        }
    }
}
