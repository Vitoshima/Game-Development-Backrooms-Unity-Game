using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Patrol Area Corners")]
    public Transform PointA;
    public Transform PointB;
    public Transform PointC;
    public Transform PointD;

    [Header("Patrol Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float reachDistance = 0.5f;
    [SerializeField] private float minWaitTime = 0.5f;
    [SerializeField] private float maxWaitTime = 2f;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.6f;
    [SerializeField] private float attackDamage = 15f;

    [Header("Audio")]
    public AudioSource audioSource;      // Looping movement sounds
    public AudioSource sfxSource;        // One-shot SFX
    public AudioClip freezeSound;
    public AudioClip chaseStartSound;
    public AudioClip attackSound;

    [Header("Movement Sounds")]
    public AudioClip walkSound;
    public AudioClip runSound;
    [Range(0f, 2f)] public float walkVolume = 0.7f;
    [Range(0f, 2f)] public float runVolume = 0.9f;

    [Header("Stuck Detection")]
    [SerializeField] private float stuckTimeLimit = 1f;

    // Private variables
    private Rigidbody rb;
    private Animator anim;
    private Vector3 targetPosition;
    private bool waiting = false;
    private float waitTimer = 0f;
    private float waitDuration = 0f;
    private float minX, maxX, minZ, maxZ;
    private Transform player = null;
    private PlayerHealth playerHealth = null;
    private bool chasing = false;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float attackTimer = 0f;
    private bool isFrozen = false;
    private float freezeTimer = 0f;

    private bool isRunningThisFrame = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
    }

    void Start()
    {
        CalculateBounds();
        targetPosition = PickRandomPoint();
        lastPosition = transform.position;
        SetWalk(true);
    }

    void FixedUpdate()
    {
        if (GetDie()) return;

        attackTimer -= Time.fixedDeltaTime;
        isRunningThisFrame = false;

        if (isFrozen)
        {
            HandleFrozenState();
        }
        else if (chasing && player != null)
        {
            ChasePlayer();
        }
        else
        {
            Wander();
        }

        UpdateMovementAudio();
    }

    private void UpdateMovementAudio()
    {
        if (isRunningThisFrame)
        {
            if (audioSource.clip != runSound || !audioSource.isPlaying)
            {
                audioSource.clip = runSound;
                audioSource.loop = true;
                audioSource.volume = runVolume;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    private void StopAllAudioExceptSFX()
    {
        AudioSource[] allSources = GetComponentsInChildren<AudioSource>(true);
        foreach (AudioSource src in allSources)
        {
            if (src == sfxSource) continue;
            src.Stop();
            src.clip = null;
        }
    }

    private void LockRigidbody()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void UnlockRigidbody()
    {
        rb.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void HandleFrozenState()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        freezeTimer -= Time.fixedDeltaTime;

        if (freezeTimer <= 0f)
        {
            isFrozen = false;
            UnlockRigidbody();
            if (anim != null) anim.speed = 1f;
        }
    }

    public void OnPlayerEnter(Transform playerTransform)
    {
        player = playerTransform;
        playerHealth = playerTransform.GetComponent<PlayerHealth>();
        chasing = true;
        waiting = false;

        if (!isFrozen)
        {
            SetRun(true);
            SetWalk(false);
            SetAttack(false);
            if (chaseStartSound != null)
                sfxSource.PlayOneShot(chaseStartSound, 1f);
        }
    }

    public void OnPlayerExit()
    {
        chasing = false;
        player = null;
        playerHealth = null;
        SetRun(false);
        SetAttack(false);
        SetWalk(true);
        targetPosition = PickRandomPoint();
        attackTimer = 0f;
    }

    public void Freeze(float duration = 3f)
    {
        isFrozen = true;
        freezeTimer = duration;

        LockRigidbody();
        SetWalk(false);
        SetRun(false);
        SetAttack(false);

        StopAllAudioExceptSFX();

        if (freezeSound != null)
            sfxSource.PlayOneShot(freezeSound, 1f);

        if (anim != null) anim.speed = 0f;   // Freeze animation
    }

    private void ChasePlayer()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer <= attackRange)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            SetRun(false);
            isRunningThisFrame = false;

            Vector3 lookDir = (player.position - transform.position);
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);

            if (attackTimer <= 0f && playerHealth != null)
            {
                SetAttack(true);
                attackTimer = attackCooldown;
                playerHealth.TakeDamage(attackDamage);
                if (attackSound != null)
                    sfxSource.PlayOneShot(attackSound, 1f);
                Invoke(nameof(ResetAttackAnimation), 0.6f);
            }
            return;
        }

        // Moving
        SetAttack(false);
        SetRun(true);
        isRunningThisFrame = true;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        Vector3 newPos = rb.position + direction * chaseSpeed * Time.fixedDeltaTime;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
        newPos.y = transform.position.y;

        rb.MovePosition(newPos);

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    private void ResetAttackAnimation()
    {
        SetAttack(false);
    }

    private void Wander()
    {
        if (waiting)
        {
            waitTimer += Time.fixedDeltaTime;
            if (waitTimer >= waitDuration)
            {
                waiting = false;
                targetPosition = PickRandomPoint();
                SetWalk(true);
            }
            return;
        }

        // Stuck detection
        float moved = Vector3.Distance(transform.position, lastPosition);
        if (moved < 0.01f)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckTimeLimit)
            {
                transform.rotation = transform.rotation * Quaternion.Euler(0f, 45f, 0f);
                targetPosition = PickRandomPoint();
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;

        float dist = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(targetPosition.x, 0, targetPosition.z));

        if (dist <= reachDistance)
        {
            SetWalk(false);
            waiting = true;
            waitTimer = 0f;
            waitDuration = Random.Range(minWaitTime, maxWaitTime);
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 dir = (targetPosition - transform.position).normalized;
        dir.y = 0f;

        rb.MovePosition(rb.position + dir * walkSpeed * Time.fixedDeltaTime);

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    private void CalculateBounds()
    {
        float[] xs = { PointA.position.x, PointB.position.x, PointC.position.x, PointD.position.x };
        float[] zs = { PointA.position.z, PointB.position.z, PointC.position.z, PointD.position.z };
        minX = Mathf.Min(xs); maxX = Mathf.Max(xs);
        minZ = Mathf.Min(zs); maxZ = Mathf.Max(zs);
    }

    private Vector3 PickRandomPoint()
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);
        return new Vector3(x, transform.position.y, z);
    }

    private void SetWalk(bool value) => anim?.SetBool("Walk", value);
    private void SetRun(bool value) => anim?.SetBool("Run", value);
    private void SetAttack(bool value) => anim?.SetBool("Attack", value);
    private void SetDie(bool value) => anim?.SetBool("Die", value);
    private bool GetDie() => anim != null && anim.GetBool("Die");

    public void TriggerDie()
    {
        SetDie(true);
        LockRigidbody();
        StopAllAudioExceptSFX();
        if (anim != null) anim.speed = 0f;
    }

    void OnDrawGizmos()
    {
        if (PointA == null || PointB == null || PointC == null || PointD == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(PointA.position, PointB.position);
        Gizmos.DrawLine(PointB.position, PointC.position);
        Gizmos.DrawLine(PointC.position, PointD.position);
        Gizmos.DrawLine(PointD.position, PointA.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPosition, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}