// HUDManager.cs
using UnityEngine;
using UnityEngine.UI; // Use TMPro se preferir
using System.Text;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("UI Elements")]
    public Text healthText;
    public Text ammoText;
    public Text roundText;
    public Text reloadingText;

    private StringBuilder sb = new StringBuilder();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        reloadingText.gameObject.SetActive(false);
    }

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        sb.Clear();
        sb.Append("Vida: ");
        sb.Append(currentHealth.ToString("F1")); // Formata para uma casa decimal
        sb.Append(" / ");
        sb.Append(maxHealth.ToString("F1"));
        healthText.text = sb.ToString();
    }

    public void UpdateAmmoUI(int currentClip, int totalAmmo)
    {
        sb.Clear();
        sb.Append("Munição: ");
        sb.Append(currentClip);
        sb.Append(" / ");
        sb.Append(totalAmmo);
        ammoText.text = sb.ToString();
    }

    public void UpdateRoundUI(int round)
    {
        roundText.text = "Round: " + round;
    }

    public void ShowReloadingText(bool show)
    {
        reloadingText.gameObject.SetActive(show);
    }
}