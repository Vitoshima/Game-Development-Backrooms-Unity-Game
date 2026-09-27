using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    public AudioClip[] footstepClips;
    public float stepInterval = 0.5f;   // seconds between steps

    private AudioSource audioSource;
    private CharacterController cc;
    private float stepTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Only play when grounded and actually moving
        bool isMoving = new Vector2(cc.velocity.x, cc.velocity.z).magnitude > 0.1f;

        if (cc.isGrounded && isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayRandomFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // reset so first step plays immediately
        }
    }

    void PlayRandomFootstep()
    {
        if (footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip);
    }
}