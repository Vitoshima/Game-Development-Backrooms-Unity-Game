using UnityEngine;

public class FlashlightStunChecker : MonoBehaviour
{
    [Header("Stun Detection Settings")]
    [SerializeField] private float maxDistance = 999f;        // Unlimited
    [SerializeField] private float mediumForgiveness = 90f;   // Medium forgiveness
    [SerializeField] private float closeForgiveness = 180f;   // Easier when very close
    [SerializeField] private float closeRangeThreshold = 6f;  // Under 6 meters = easier

    private FlashlightManager flashlight;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        flashlight = FindFirstObjectByType<FlashlightManager>();
    }

    private void Update()
    {
        if (flashlight == null || flashlight.state != FlashlightState.On) return;

        FlashlightStun[] enemies = FindObjectsByType<FlashlightStun>(FindObjectsSortMode.None);

        foreach (FlashlightStun enemyStun in enemies)
        {
            if (enemyStun.IsStunned() || !enemyStun.CanBeStunned()) continue;

            Transform enemy = enemyStun.transform;

            // Aim at chest/head level instead of feet
            Vector3 targetPoint = enemy.position + Vector3.up * 1.4f;
            Vector3 screenPos = cam.WorldToScreenPoint(targetPoint);

            float distance = Vector3.Distance(cam.transform.position, enemy.position);

            // Choose forgiveness based on distance
            float forgiveness = (distance < closeRangeThreshold) ? closeForgiveness : mediumForgiveness;

            bool isInView =
                screenPos.x > Screen.width / 2 - forgiveness &&
                screenPos.x < Screen.width / 2 + forgiveness &&
                screenPos.y > Screen.height / 2 - forgiveness &&
                screenPos.y < Screen.height / 2 + forgiveness &&
                screenPos.z > 0 && screenPos.z < maxDistance;

            if (isInView)
            {
                enemyStun.TryStun();
                break; // Only stun one at a time
            }
        }
    }

    // Instant check when flashlight is turned ON
    public void OnFlashlightToggled()
    {
        // Force an immediate check
        Update();
    }
}