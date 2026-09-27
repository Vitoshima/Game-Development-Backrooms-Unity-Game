using UnityEngine;
using UnityEngine.InputSystem;

public class LadderPiece : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 2f;

    [Header("UI Prompt")]
    public GameObject promptUI;

    [Header("Audio")]
    public AudioClip pickupSound;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactRange)
        {
            if (promptUI != null)
                promptUI.SetActive(true);

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Collect();
            }
        }
        else
        {
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    void Collect()
    {
        // Play pickup sound
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        LadderManager.Instance.CollectPiece();

        if (promptUI != null)
            promptUI.SetActive(false);

        Destroy(gameObject);
    }
}