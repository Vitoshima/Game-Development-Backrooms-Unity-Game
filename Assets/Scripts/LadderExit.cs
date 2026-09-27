using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using StarterAssets;
using UnityEngine.InputSystem;

public class LadderExit : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 2f;
    public string nextSceneName = "Level1"; // No longer used if showWinScreenInstead is true

    [Header("UI")]
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public Image fadeImage;

    [Header("Fade")]
    public float fadeDuration = 1.5f;

    [Header("Win Screen")]
    [Tooltip("If true, shows the Win Screen panel via LadderManager after fading to black, instead of loading a new scene.")]
    public bool showWinScreenInstead = true;

    private Transform player;
    private bool isClimbing = false;
    private FirstPersonController fpsController;
    private StarterAssetsInputs inputSystem;
    private CharacterController charController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fpsController = player.GetComponent<FirstPersonController>();
        inputSystem = player.GetComponent<StarterAssetsInputs>();
        charController = player.GetComponent<CharacterController>();

        if (promptUI != null) promptUI.SetActive(false);

        // Make sure fade starts transparent
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    void Update()
    {
        if (isClimbing) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactRange)
        {
            if (promptUI != null) promptUI.SetActive(true);

            if (LadderManager.Instance.IsComplete())
            {
                if (promptText != null)
                    promptText.text = "E";

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    StartCoroutine(FadeAndLoad());
                }
            }
            else
            {
                if (promptText != null)
                    promptText.text = "You need all 5 ladder pieces";
            }
        }
        else
        {
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    IEnumerator FadeAndLoad()
    {
        isClimbing = true;

        // Disable input only, NOT the CharacterController
        fpsController.enabled = false;
        inputSystem.enabled = false;

        if (promptUI != null) promptUI.SetActive(false);

        // Fade to black
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }

        // ─── Win screen instead of loading a new scene ────────────────────
        if (showWinScreenInstead)
        {
            LadderManager.Instance.WinGame();
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}