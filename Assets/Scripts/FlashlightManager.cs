using UnityEngine;
using UnityEngine.InputSystem;

public enum FlashlightState
{
    Off,
    On,
    Dead
}

[RequireComponent(typeof(AudioSource))]
public class FlashlightManager : MonoBehaviour
{
    [Header("Options")]
    [Tooltip("Starting battery when the game begins")] 
    [SerializeField] int startBattery = 100;

    [Tooltip("Maximum battery the player can hold")] 
    [SerializeField] int maxBattery = 100;

    [Tooltip("How fast battery drains while flashlight is on")] 
    [Range(0.0f, 2f)] 
    [SerializeField] float batteryLossTick = 0.5f;

    [Tooltip("Current battery amount")] 
    public int currentBattery;

    [Tooltip("The current state of the flashlight.")] 
    public FlashlightState state = FlashlightState.Off;

    private bool flashlightOn;

    [Header("References")]
    [SerializeField] GameObject flashlightLight;
    [SerializeField] AudioClip flashlightOn_FX, flashlightOff_FX;

    private void Start()
    {
        currentBattery = startBattery;
        state = FlashlightState.Off;           // Start off by default
        InvokeRepeating(nameof(LoseBattery), 0, batteryLossTick);
    }

    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame) 
            ToggleFlashlight();

        // Update light visibility
        if (state == FlashlightState.On)
            flashlightLight.SetActive(true);
        else
            flashlightLight.SetActive(false);

        // Prevent negative battery
        if (currentBattery <= 0)
        {
            currentBattery = 0;
            state = FlashlightState.Dead;
            flashlightOn = false;
        }
    }

    public void GainBattery(int amount)
    {
        currentBattery += amount;

        // Cap at max battery
        if (currentBattery > maxBattery)
            currentBattery = maxBattery;

        // If flashlight was dead, turn it back on
        if (state == FlashlightState.Dead && currentBattery > 0)
        {
            state = FlashlightState.On;
            flashlightOn = true;
        }
    }

    private void LoseBattery()
    {
        if (state == FlashlightState.On && currentBattery > 0)
            currentBattery--;
    }

    private void ToggleFlashlight()
    {
        if (state == FlashlightState.Dead) 
            return;

        flashlightOn = !flashlightOn;

        if (flashlightOn)
        {
            GetComponent<AudioSource>().PlayOneShot(flashlightOn_FX);
            state = FlashlightState.On;
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(flashlightOff_FX);
            state = FlashlightState.Off;
        }

        FlashlightStunChecker checker = FindFirstObjectByType<FlashlightStunChecker>();
        if (checker != null)
            checker.OnFlashlightToggled();
    }
}