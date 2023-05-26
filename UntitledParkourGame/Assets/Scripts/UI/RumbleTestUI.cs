using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ControllerRumbleManager;

public class RumbleTestUI : MonoBehaviour
{
    [Range(0.1f, 1.5f)]
    public float PulseFrequency = 0.25f;

    private float rumbleTime = 3f;
    private RumblePattern rumblePattern = RumblePattern.Constant;
    [SerializeField] Slider lowSlider;
    [SerializeField] Slider lowSliderEnd;
    [SerializeField] Slider highSlider;
    [SerializeField] Slider highSliderEnd;
    [SerializeField] Dropdown modeDropdown;
    [SerializeField] ControllerRumbleManager rumbler;
    private float[] timeDropdown = new float[] { 0.25f, 3f, 5f, 10f};
    private RumblePattern[] rumbleMode = new RumblePattern[] { RumblePattern.Constant, RumblePattern.Pulse, RumblePattern.Linear, RumblePattern.Burst};
    public void SetDurration(int selectedValue)
    {
        rumbleTime = timeDropdown[selectedValue];
    }

    public void SetRumbleMode()
    {
        rumblePattern = rumbleMode[modeDropdown.value];
    }

    public void StartPressed()
    {
        switch (rumblePattern)
        {
            case RumblePattern.Constant:
                rumbler.RumbleConstant(lowSlider.value, highSlider.value, rumbleTime);
                break;
            case RumblePattern.Pulse:
                rumbler.RumblePulse(lowSlider.value, highSlider.value, PulseFrequency, rumbleTime);
                break;
            case RumblePattern.Linear:
                rumbler.RumbleLinear(lowSlider.value, lowSliderEnd.value, highSlider.value, highSliderEnd.value, rumbleTime);
                break;
            case RumblePattern.Burst:
                rumbler.RumbleBurst(lowSlider.value, highSlider.value, PulseFrequency);
                break;
            default:
                break;
        }

    }
    public void StopPressed()
    {
        rumbler.StopRumble();
    }
}