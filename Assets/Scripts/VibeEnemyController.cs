using UnityEngine;

public class VibeEnemyController : MonoBehaviour
{
    [Header("Patrol Points (Four Corners)")]
    public Transform PointA;
    public Transform PointB;
    public Transform PointC;
    public Transform PointD;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2.2f;
    [SerializeField] private float reachDistance = 0.6f;
    
    [Header("Waiting Settings")]
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 4f;

    [Header("Footsteps - Loud Settings")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float footstepInterval = 0.4f;           // Slightly faster steps
    [SerializeField] private float footstepVolume = 2.5f;            // Very high (Unity allows >1)
    [SerializeField] private float playerProximityDistance = 30f;    // Increased range

    // Animator
    private Animator anim;

    // Private variables
    private Vector3 targetPosition;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float waitDuration = 0f;

    private float minX, maxX, minZ, maxZ;

    private Rigidbody rb;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    [SerializeField] private float stuckTimeLimit = 1.5f;

    private float footstepTimer = 0f;
    private Transform playerTransform;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        
        if (footstepAudioSource == null)
            footstepAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        CalculateBounds();
        targetPosition = PickRandomPoint();
        lastPosition = transform.position;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogWarning("Player not found! Tag your player 'Player'.");

        SetWalk(true);
    }

    void FixedUpdate()
    {
        if (isWaiting)
        {
            waitTimer += Time.fixedDeltaTime;
            if (waitTimer >= waitDuration)
            {
                isWaiting = false;
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
            isWaiting = true;
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

        HandleFootsteps();
    }

    private void HandleFootsteps()
    {
        if (footstepClip == null || footstepAudioSource == null || playerTransform == null)
            return;

        bool isWalking = anim.GetBool("Walk") && anim.GetCurrentAnimatorStateInfo(0).IsName("Walk");

        if (!isWalking)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer += Time.fixedDeltaTime;

        if (footstepTimer >= footstepInterval)
        {
            PlayFootstep();
            footstepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > playerProximityDistance)
            return;

        // === MAXIMUM LOUDNESS SETUP ===
        float originalSpatial = footstepAudioSource.spatialBlend;
        footstepAudioSource.spatialBlend = 0f;           // Full 2D
        footstepAudioSource.priority = 0;                // Highest priority
        footstepAudioSource.volume = 1f;                 // Force max base volume

        // Add a bit of natural variation
        float finalVolume = footstepVolume * Random.Range(0.95f, 1.05f);

        footstepAudioSource.pitch = Random.Range(0.92f, 1.08f);
        footstepAudioSource.PlayOneShot(footstepClip, finalVolume);

        footstepAudioSource.spatialBlend = originalSpatial;
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

    private void SetWalk(bool value)
    {
        if (anim != null)
            anim.SetBool("Walk", value);
    }

    public void StopMovement()
    {
        SetWalk(false);
        rb.linearVelocity = Vector3.zero;
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
        Gizmos.DrawSphere(targetPosition, 0.25f);
    }
}