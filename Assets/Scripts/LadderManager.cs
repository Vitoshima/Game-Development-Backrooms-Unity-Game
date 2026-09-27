using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LadderManager : MonoBehaviour
{
    public static LadderManager Instance;

    [Header("Ladder Settings")]
    public int totalPieces = 5;
    public int piecesCollected = 0;

    [Header("UI")]
    public TextMeshProUGUI ladderUI;

    [Header("Exit Ladder Stages")]
    public GameObject ladderStage0;
    public GameObject ladderStage1;
    public GameObject ladderStageFull;

    [Header("Win Screen")]
    public GameObject winScreenPanel;      // Drag your Win Screen UI panel here
    public bool pauseGameOnWin = true;     // Freeze the game when win screen shows
    public bool unlockCursorOnWin = true;  // Show/free the mouse cursor on win

    private bool hasWon = false;           // Prevents win screen from triggering twice

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ladderUI.gameObject.SetActive(false); // hidden until door unlocks

        // Make sure the win screen starts hidden
        if (winScreenPanel != null)
            winScreenPanel.SetActive(false);

        UpdateLadderVisual();
    }

    public void OnDoorUnlocked()
    {
        ladderUI.gameObject.SetActive(true);
        UpdateUI();
    }

    public void CollectPiece()
    {
        piecesCollected++;
        UpdateUI();
        UpdateLadderVisual();
    }

    void UpdateUI()
    {
        if (!ladderUI.gameObject.activeSelf) return; // don't update if hidden

        if (piecesCollected < totalPieces)
        {
            ladderUI.text = "Ladder Pieces: " + piecesCollected + "/" + totalPieces;
        }
        else
        {
            ladderUI.text = "Ladder Complete! Find the exit!";
        }
    }

    void UpdateLadderVisual()
    {
        if (piecesCollected == 0)
        {
            if (ladderStage0 != null) ladderStage0.SetActive(true);
            if (ladderStage1 != null) ladderStage1.SetActive(false);
            if (ladderStageFull != null) ladderStageFull.SetActive(false);
        }
        else if (piecesCollected < totalPieces)
        {
            if (ladderStage0 != null) ladderStage0.SetActive(false);
            if (ladderStage1 != null) ladderStage1.SetActive(true);
            if (ladderStageFull != null) ladderStageFull.SetActive(false);
        }
        else
        {
            if (ladderStage0 != null) ladderStage0.SetActive(false);
            if (ladderStage1 != null) ladderStage1.SetActive(false);
            if (ladderStageFull != null) ladderStageFull.SetActive(true);
        }
    }

    public bool IsComplete()
    {
        return piecesCollected >= totalPieces;
    }

    // ─── Call this when the player reaches/clicks the final ladder to escape ──
    // Replaces the old fade-to-black behaviour with the win screen panel.
    public void WinGame()
    {
        if (hasWon) return; // prevent double-trigger
        hasWon = true;

        if (winScreenPanel != null)
        {
            winScreenPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("LadderManager: winScreenPanel is not assigned in the Inspector!");
        }

        if (pauseGameOnWin)
            Time.timeScale = 0f;

        if (unlockCursorOnWin)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}