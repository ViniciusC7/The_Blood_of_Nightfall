// UpgradeManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public enum UpgradeType { Health, Speed, Damage, AmmoCapacity }

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Referências")]
    public GameObject upgradePanel;
    public Button[] upgradeButtons; // 3 botões na UI
    public Text[] upgradeButtonTexts;

    private PlayerController playerController;
    private WeaponController weaponController;

    private List<UpgradeType> availableUpgrades = new List<UpgradeType>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        weaponController = FindObjectOfType<WeaponController>();
        upgradePanel.SetActive(false);
    }

    public void OfferUpgrades()
    {
        Time.timeScale = 0f; // Pausa o jogo
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        upgradePanel.SetActive(true);

        // Pega 3 upgrades aleatórios e únicos
        availableUpgrades = System.Enum.GetValues(typeof(UpgradeType)).Cast<UpgradeType>().ToList();
        var randomUpgrades = availableUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            int index = i; // Captura de variável para o listener
            upgradeButtons[i].onClick.RemoveAllListeners(); // Limpa listeners antigos
            upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(randomUpgrades[index]));
            upgradeButtonTexts[i].text = GetUpgradeDescription(randomUpgrades[i]);
        }
    }

    private string GetUpgradeDescription(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Health: return "+1 Barra de Vida";
            case UpgradeType.Speed: return "+10% Velocidade";
            case UpgradeType.Damage: return "+25% Dano da Arma";
            case UpgradeType.AmmoCapacity: return "+4 Balas no Pente";
            default: return "UPGRADE";
        }
    }

    public void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Health:
                playerController.IncreaseMaxHealth(1f);
                break;
            case UpgradeType.Speed:
                playerController.IncreaseSpeed(1.1f); // Aumento de 10%
                break;
            case UpgradeType.Damage:
                weaponController.IncreaseDamage(1.25f); // Aumento de 25%
                break;
            case UpgradeType.AmmoCapacity:
                weaponController.IncreaseAmmoCapacity(4);
                break;
        }

        CloseUpgradePanel();
    }

    private void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f; // Despausa o jogo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Avisa o HordeManager para começar o próximo round
        HordeManager.Instance.ResumeAfterUpgrade();
    }
}