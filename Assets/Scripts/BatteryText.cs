using UnityEngine;
using TMPro;

public class BatteryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI batteryText;

    private FlashlightManager fm;

    private void Start()
    {
        fm = FindFirstObjectByType<FlashlightManager>();
    }

    private void Update()
    {
        if (fm == null) return;

        int percent = Mathf.RoundToInt((float)fm.currentBattery / 100f * 100);

        if (fm.state == FlashlightState.Dead)
        {
            batteryText.text = "BATTERY: DEAD";
            batteryText.color = Color.red;
        }
        else if (fm.state == FlashlightState.Off)
        {
            batteryText.text = $"BATTERY: {percent}%";
            batteryText.color = Color.grey;
        }
        else
        {
            batteryText.text = $"BATTERY: {percent}%";

            // colour changes based on how much battery is left
            if (percent > 50) batteryText.color = Color.green;
            else if (percent > 20) batteryText.color = Color.yellow;
            else batteryText.color = Color.red;
        }
    }
}