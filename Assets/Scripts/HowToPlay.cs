using UnityEngine;
using UnityEngine.UI;

public class HowToPlayManager : MonoBehaviour
{
    [Header("How To Play Panel")]
    public GameObject howToPlayPanel;

    [Header("Buttons")]
    public Button howToPlayButton;     // Button on main menu
    public Button closeButton;         // Button inside the panel

    private void Start()
    {
        // Make sure panel starts hidden
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);

        // Hook up buttons
        if (howToPlayButton != null)
            howToPlayButton.onClick.AddListener(ShowHowToPlay);

        if (closeButton != null)
            closeButton.onClick.AddListener(HideHowToPlay);
    }

    public void ShowHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    public void HideHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }
}