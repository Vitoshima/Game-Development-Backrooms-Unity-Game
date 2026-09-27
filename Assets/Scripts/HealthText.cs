using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    private void Update()
    {
        if (playerHealth == null) return;

        int health = Mathf.RoundToInt(playerHealth.currentHealth);

        healthText.text = $"HEALTH: {health}%";

        if (health > 50) healthText.color = Color.green;
        else if (health > 20) healthText.color = Color.yellow;
        else healthText.color = Color.red;
    }
}