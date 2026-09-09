using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// GameManager principal de TapMon: Mi Compi Virtual.
/// Administra monedas por toque, sistema de mejoras, AdMob y persistencia con PlayerPrefs.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("Texto TMP que muestra el contador de monedas.")]
    [SerializeField] private TMP_Text coinDisplay;

    [Header("Coin Settings")]
    [Tooltip("Cantidad de monedas otorgadas por toque base.")]
    [SerializeField] private int baseCoinsPerTap = 1;

    [Tooltip("Monedas máximas permitidas (0 = sin límite).")]
    [SerializeField] private int maxCoins = 0;

    [Header("AdMob Rewards")]
    [Tooltip("Cantidad de monedas otorgadas por anuncio recompensado.")]
    [SerializeField] private int rewardCoins = 500;

    [Header("Events")]
    public UnityEvent<int> OnCoinsChanged;

    [Tooltip("Evento invocado cuando se compra una mejora.")]
    public UnityEvent<UpgradeSO> OnUpgradePurchased;

    private int _currentCoins;
    private float _autoClickTimer;
    private int _autoClickAccumulated;
    private int _autoClickLevel;

    public int CurrentCoins
    {
        get => _currentCoins;
        private set
        {
            _currentCoins = (maxCoins > 0) ? Mathf.Clamp(value, 0, maxCoins) : Mathf.Max(0, value);
        }
    }

    public int TotalCoinsEarned { get; private set; }
    public int TotalTaps { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadProgress();
        UpdateCoinDisplay();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) HandleTap(Input.mousePosition);
        HandleAutoClick();
    }

    public void HandleTap(Vector2 screenPosition)
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("[GameManager] No hay una cámara principal para detectar el toque.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity))
        {
            AddCoins(GetCoinsPerTap());
            TotalTaps++;
            SaveProgress();
        }
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        Debug.Log($"[GameManager] Toque recibido: +{amount} moneda(s).");
        CurrentCoins += amount;
        TotalCoinsEarned += amount;
        UpdateCoinDisplay();
        OnCoinsChanged?.Invoke(CurrentCoins);
        SaveProgress();
    }

    public void WatchAdForCoins()
    {
        if (AdManager.Instance == null)
        {
            Debug.LogWarning("[GameManager] AdManager no está presente en la escena.");
            return;
        }

        AdManager.Instance.ShowRewardedAd((success) =>
        {
            if (success)
            {
                Debug.Log("[GameManager] Anuncio completado, esperando recompensa");
            }
            else
            {
                Debug.LogWarning("[GameManager] No se pudo mostrar el anuncio recompensado");
            }
        });
    }

    public void OnRewardEarned()
    {
        AddCoins(rewardCoins);
        Debug.Log($"[GameManager] ¡Recompensa entregada! +{rewardCoins} monedas");
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || CurrentCoins < amount) return false;

        CurrentCoins -= amount;
        UpdateCoinDisplay();
        OnCoinsChanged?.Invoke(CurrentCoins);
        SaveProgress();
        return true;
    }

    private int GetCoinsPerTap()
    {
        return Mathf.Max(1, baseCoinsPerTap);
    }

    private void HandleAutoClick()
    {
        const float autoClickInterval = 1f;
        if (_autoClickLevel <= 0) return;

        _autoClickTimer += Time.deltaTime;

        if (_autoClickTimer < autoClickInterval) return;

        int autoClicks = Mathf.FloorToInt(_autoClickTimer / autoClickInterval);
        _autoClickTimer %= autoClickInterval;
        AddCoins(autoClicks * _autoClickLevel);
    }

    public bool PurchaseUpgrade(UpgradeSO upgrade)
    {
        if (upgrade == null || !upgrade.isUnlocked)
        {
            Debug.LogWarning("[GameManager] La mejora no es válida o está bloqueada.");
            return false;
        }

        int currentCost = upgrade.GetCurrentCost();
        if (!SpendCoins(currentCost))
        {
            Debug.LogWarning($"[GameManager] Monedas insuficientes. Necesitas {currentCost}.");
            return false;
        }

        ApplyUpgradeEffect(upgrade);
        upgrade.currentPurchaseCount++;
        OnUpgradePurchased?.Invoke(upgrade);
        SaveProgress();
        return true;
    }

    private void ApplyUpgradeEffect(UpgradeSO upgrade)
    {
        if (upgrade.effectValue == 1) baseCoinsPerTap++;
        if (upgrade.effectValue == 2) _autoClickLevel++;
    }

    public void UpdateCoinDisplay()
    {
        if (coinDisplay != null) coinDisplay.text = CurrentCoins.ToString();
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("TapMon_Coins", CurrentCoins);
        PlayerPrefs.SetInt("TapMon_TotalCoinsEarned", TotalCoinsEarned);
        PlayerPrefs.SetInt("TapMon_TotalTaps", TotalTaps);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        CurrentCoins = PlayerPrefs.GetInt("TapMon_Coins", 0);
        TotalCoinsEarned = PlayerPrefs.GetInt("TapMon_TotalCoinsEarned", 0);
        TotalTaps = PlayerPrefs.GetInt("TapMon_TotalTaps", 0);
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
    }
}