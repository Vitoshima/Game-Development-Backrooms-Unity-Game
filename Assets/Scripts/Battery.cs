using UnityEngine;
using UnityEngine.InputSystem;

public class Battery : MonoBehaviour
{
    [Header("Options")]
    [Tooltip("How much battery the player gains when picked up")]
    [SerializeField] int batteryAmount = 50;

    [Header("References")]
    [Tooltip("UI objects to show when player is near (hover text, etc.)")]
    [SerializeField] GameObject[] HoverObjects;

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            SetHoverActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            SetHoverActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            FlashlightManager fm = FindFirstObjectByType<FlashlightManager>();
            
            if (fm != null)
            {
                fm.GainBattery(batteryAmount);
                
                // Fix: Hide hover BEFORE destroying
                SetHoverActive(false);
                Destroy(gameObject);
            }
        }
    }

    // Helper method to avoid code duplication
    private void SetHoverActive(bool active)
    {
        foreach (GameObject obj in HoverObjects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }
}