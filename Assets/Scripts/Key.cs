using UnityEngine;
using UnityEngine.InputSystem;

public class Key : MonoBehaviour
{
    [Tooltip("The name of the key. This corresponds with the key on the door")] public string keyName;

    public GameObject HoverIcon;

    [Header("(Optional)")]
    [Tooltip("(Optional.)")] public AudioClip CollectAudio;

    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            HoverIcon.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HoverIcon.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (CollectAudio != null) FindFirstObjectByType<KeyManager>().GetComponent<AudioSource>().PlayOneShot(CollectAudio);
            FindFirstObjectByType<KeyManager>().keysInInventory.Add(keyName);
            HoverIcon.SetActive(false);
            Destroy(this.gameObject);
        }
    }
}