using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(AudioSource))]
public class IntroText : MonoBehaviour
{
    [Header("Settings")]
    public string message = "What is this place...\nI need to find a way out of here.";
    public float typingSpeed = 0.25f;
    public float displayTime = 2f;
    public float fadeSpeed = 1f;

    [Header("Sound")]
    public AudioClip typingSound;
    [Range(0f, 1f)] public float volume = 0.5f;

    [Header("Debug")]
    public bool resetOnStart = false;   // ← Turn this ON to test

    private TextMeshProUGUI tmpText;
    private AudioSource audioSource;

    void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Reset for testing (remove or set to false later)
        if (resetOnStart)
        {
            PlayerPrefs.DeleteKey("HasSeenIntro");
            PlayerPrefs.Save();
        }

        // Check if we've already seen it
        if (PlayerPrefs.GetInt("HasSeenIntro", 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        tmpText.text = "";
        tmpText.alpha = 1f;
        StartCoroutine(ShowIntro());
    }

    IEnumerator ShowIntro()
    {
        foreach (char letter in message)
        {
            tmpText.text += letter;

            if (letter != ' ' && letter != '\n' && typingSound != null)
            {
                audioSource.Stop();
                audioSource.clip = typingSound;
                audioSource.volume = volume;
                audioSource.Play();
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        audioSource.Stop();

        yield return new WaitForSeconds(displayTime);

        while (tmpText.alpha > 0)
        {
            tmpText.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        gameObject.SetActive(false);

        // Mark as seen
        PlayerPrefs.SetInt("HasSeenIntro", 1);
        PlayerPrefs.Save();
    }
}