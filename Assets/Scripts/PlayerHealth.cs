using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("Player starts at this health (0–100).")]
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }

    [Header("Invincibility Frames")]
    [Tooltip("Seconds of invincibility after taking damage (prevents instant kill from standing in enemy).")]
    public float iFrameDuration = 0.5f;

    [Header("Regen")]
    [Tooltip("Automatically regenerate health over time when not being damaged.")]
    public bool autoRegen = false;
    public float regenPerSecond = 2f;
    public float regenDelay     = 5f;

    [Header("Death")]
    [Tooltip("UI panel to show on death (can be a black screen with Restart button).")]
    public GameObject deathUI;
    [Tooltip("Scene to reload on death. Defaults to current scene if blank.")]
    public string restartSceneName = "";
    [Tooltip("Freeze the entire game (enemies, animations, sounds) when the player dies.")]
    public bool pauseGameOnDeath = true;

    [Header("Screen Flash")]
    public UnityEngine.UI.Image damageFlashImage;
    public float flashDuration = 0.3f;

    // ── Private ───────────────────────────────────────────────────────────────
    private bool  isDead        = false;
    private bool  isInvincible  = false;
    private float regenTimer    = 0f;

    void Start()
    {
        currentHealth = maxHealth;

        if (deathUI != null) deathUI.SetActive(false);
        if (damageFlashImage != null)
        {
            Color c = damageFlashImage.color;
            c.a = 0f;
            damageFlashImage.color = c;
        }
    }

    void Update()
    {
        if (isDead) return;

        if (autoRegen && currentHealth < maxHealth)
        {
            regenTimer -= Time.deltaTime;
            if (regenTimer <= 0f)
            {
                currentHealth = Mathf.Min(currentHealth + regenPerSecond * Time.deltaTime, maxHealth);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead || isInvincible) return;

        currentHealth -= amount;
        regenTimer     = regenDelay;

        if (damageFlashImage != null)
            StartCoroutine(DamageFlash());

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
        else
        {
            StartCoroutine(IFrames());
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Lock controls
        var fps = GetComponent<StarterAssets.FirstPersonController>();
        if (fps != null) fps.enabled = false;

        var input = GetComponent<StarterAssets.StarterAssetsInputs>();
        if (input != null) input.enabled = false;

        // Show death screen
        if (deathUI != null) deathUI.SetActive(true);

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        // Freeze the entire game — this stops enemy movement, attack
        // timers, animations, and any audio tied to Time.deltaTime /
        // Time.fixedDeltaTime, all in one line. No need to touch enemy
        // scripts individually.
        if (pauseGameOnDeath)
            Time.timeScale = 0f;
    }

    // ── Called by a Restart button in the death UI ────────────────────────────
    public void RestartGame()
    {
        // Always restore timescale before loading a scene, otherwise the
        // reloaded scene starts frozen too.
        Time.timeScale = 1f;

        string scene = string.IsNullOrEmpty(restartSceneName)
            ? SceneManager.GetActiveScene().name
            : restartSceneName;

        SceneManager.LoadScene(scene);
    }

    IEnumerator IFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(iFrameDuration);
        isInvincible = false;
    }

    IEnumerator DamageFlash()
    {
        float elapsed = 0f;
        Color c = damageFlashImage.color;

        while (elapsed < flashDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / (flashDuration * 0.5f)) * 0.5f;
            damageFlashImage.color = c;
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < flashDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(1f - elapsed / (flashDuration * 0.5f)) * 0.5f;
            damageFlashImage.color = c;
            yield return null;
        }

        c.a = 0f;
        damageFlashImage.color = c;
    }
}