using UnityEngine;

public class FlashlightStun : MonoBehaviour
{
    [Header("Stun Settings")]
    public float stunDuration = 5f;
    public float cooldownAfterStun = 1f;

    private bool isStunned = false;
    private float stunTimer = 0f;
    private float cooldownTimer = 0f;

    private Animator anim;
    private EnemyController enemyController;
    private Rigidbody rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                EndStun();
            }
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void TryStun()
    {
        if (cooldownTimer > 0f || enemyController == null) return;

        isStunned = true;
        stunTimer = stunDuration;
        cooldownTimer = stunDuration + cooldownAfterStun;

        // Let EnemyController handle ALL freezing logic
        enemyController.Freeze(stunDuration);

        if (anim) anim.speed = 0f; // Optional: freeze animation mid-frame (creepy effect)

        Debug.Log("Enemy frozen by flashlight!");
    }

    private void EndStun()
    {
        isStunned = false;

        if (anim) anim.speed = 1f;
        
        // Re-enable movement (EnemyController will handle unlocking)
        // No need to call anything extra — the Freeze timer already ends it
    }

    public bool IsStunned() => isStunned;
    public bool CanBeStunned() => cooldownTimer <= 0f;
}