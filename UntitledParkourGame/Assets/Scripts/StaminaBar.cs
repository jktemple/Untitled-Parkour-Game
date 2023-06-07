using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : NetworkBehaviour
{
    // Start is called before the first frame update

    PlayerMovement script;
    Slider bar;
    public float currentStamina;
    Shoving shoveScript;
    void Start()
    {
        if (!IsOwner) return;
        script= GetComponent<PlayerMovement>();
        currentStamina = script.currentStamina;
        bar = GameObject.FindGameObjectWithTag("StaminaSlider").GetComponent<Slider>();
        shoveScript = GetComponent<Shoving>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        currentStamina = script.currentStamina;
        bar.value = currentStamina;
        if (shoveScript.infected.Value) {
            bar.fillRect.GetComponent<Image>().color = Color.red;
            bar.handleRect.GetComponent<Image>().color = Color.red;
        } else
        {
            bar.fillRect.GetComponent<Image>().color = Color.white;
            bar.handleRect.GetComponent<Image>().color = Color.white;
        }
    }
}
