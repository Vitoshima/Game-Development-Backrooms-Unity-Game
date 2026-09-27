using UnityEngine;

public class ConstantAmbientSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float targetVolume = 0.3f;   // ← Change this to your desired constant volume

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (audioSource == null) return;

        audioSource.volume = targetVolume;
        audioSource.loop = true;
        
        // Stop any previous playing and restart at correct volume
        audioSource.Stop();
        audioSource.Play();

        // Keep enforcing the volume every frame for the first few seconds
        InvokeRepeating(nameof(EnforceVolume), 0f, 0.1f);
    }

    private void EnforceVolume()
    {
        if (audioSource != null)
            audioSource.volume = targetVolume;
    }

    void OnDestroy()
    {
        CancelInvoke(nameof(EnforceVolume));
    }
}