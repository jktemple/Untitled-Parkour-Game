using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    // Start is called before the first frame update

    public PlayerMovement script;
    public Slider bar;
    public float currentStamina;
    void Start()
    {
        currentStamina = script.currentStamina;
    }

    // Update is called once per frame
    void Update()
    {
        currentStamina = script.currentStamina;
        bar.value = currentStamina;
    }
}
