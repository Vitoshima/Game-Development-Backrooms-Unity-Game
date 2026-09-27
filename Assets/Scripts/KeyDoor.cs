using UnityEngine;
using UnityEngine.InputSystem;

public class KeyDoor : MonoBehaviour
{
    [Header("Attributes")]
    [Tooltip("The name of the key that is required.")] public string keyName = "";

    [Header("References")]
    public GameObject CursorHover;
    public Animation Door;
    public AudioSource DoorOpenSound;
    public AudioSource LockedDoorSound;

    [Header("Ladder Pieces")]
    public GameObject[] ladderPieces;

    private bool isUnlocked;
    private KeyManager km;
    private bool playerInRange = false;

    private void Start()
    {
        km = FindFirstObjectByType<KeyManager>();

        foreach (GameObject piece in ladderPieces)
        {
            if (piece != null) piece.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            CursorHover.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            CursorHover.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            foreach (string key in km.keysInInventory)
            {
                if (key.Trim().ToLower() == keyName.Trim().ToLower())
                {
                    isUnlocked = true;
                    playerInRange = false;
                    CursorHover.SetActive(false);
                    GetComponent<BoxCollider>().enabled = false;
                    Door.Play();
                    DoorOpenSound.Play();
                    km.keysInInventory.Remove(key);

                    LadderManager.Instance.OnDoorUnlocked();

                    foreach (GameObject piece in ladderPieces)
                    {
                        if (piece != null) piece.SetActive(true);
                    }

                    break;
                }
            }

            if (!isUnlocked)
            {
                LockedDoorSound.Play();
            }

            isUnlocked = false;
        }
    }
}