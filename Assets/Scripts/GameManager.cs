using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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

    [Tooltip("Imagen del botón central que representa a la mascota.")]
    [SerializeField] private Image tapButtonImage;

    [Header("Mejoras disponibles")]
    [SerializeField] private UpgradeSO mejoraDanio;
    [SerializeField] private UpgradeSO mejoraAuto;
    [SerializeField] private UpgradeSO mejoraMascota;

    [Header("Misiones diarias")]
    [SerializeField] private List<DailyMission> dailyMissions = new List<DailyMission>();

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

    public UnityEvent OnDailyMissionsChanged;

    private int _currentCoins;
    private float _autoClickTimer;
    private int _autoClickLevel;
    private int _damageLevel;

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
        ResetDailyMissions();
        ApplyLoadedUpgrades();
        UpdateCoinDisplay();
        OnCoinsChanged?.Invoke(CurrentCoins);
        OnDailyMissionsChanged?.Invoke();
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
            AddTapCoins();
        }
    }

    public void AddTapCoins()
    {
        AddCoins(GetCoinsPerTap());
        TotalTaps++;
        CheckMissionProgress(MissionType.Toques, 1);
        SaveProgress();
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
        AddCoins(500);
        Debug.Log("[GameManager] ¡Recompensa entregada! +500 monedas");

        if (UITouchFeedback.Instance != null)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 coinDisplayPos = GameObject.Find("CoinDisplay") != null ? (Vector2)GameObject.Find("CoinDisplay").transform.position : screenCenter + new Vector2(0, 300);
            UITouchFeedback.Instance.SpawnCoinBurst(screenCenter, coinDisplayPos, 20);
            UITouchFeedback.Instance.PlayCashSound();
            UITouchFeedback.Instance.TriggerVibration();
            UITouchFeedback.Instance.SpawnFloatingText(screenCenter, "+500 🪙", new Color(1f, 0.843f, 0f, 1f));
        }
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || CurrentCoins < amount) return false;

        CurrentCoins -= amount;
        CheckMissionProgress(MissionType.Gastos, amount);
        UpdateCoinDisplay();
        OnCoinsChanged?.Invoke(CurrentCoins);
        SaveProgress();
        return true;
    }

    public int GetCoinsPerTap()
    {
        return Mathf.Max(1, baseCoinsPerTap * (int)Mathf.Pow(2, _damageLevel));
    }

    public void SetTapButtonImage(Image image)
    {
        tapButtonImage = image;
        ApplyLoadedUpgrades();
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
        if (upgrade == null)
        {
            Debug.LogWarning("[GameManager] La mejora no es válida.");
            return false;
        }

        int currentLevel = GetUpgradeLevel(upgrade);
        int currentCost = upgrade.GetCostForLevel(currentLevel);
        if (!SpendCoins(currentCost))
        {
            Debug.LogWarning($"[GameManager] Monedas insuficientes. Necesitas {currentCost}.");
            return false;
        }

        ApplyUpgradeEffect(upgrade);
        SetUpgradeLevel(upgrade, currentLevel + 1);
        CheckMissionProgress(MissionType.Mejoras, 1);
        OnUpgradePurchased?.Invoke(upgrade);
        SaveProgress();
        return true;
    }

    private void ApplyUpgradeEffect(UpgradeSO upgrade)
    {
        switch (upgrade.efecto)
        {
            case UpgradeSO.UpgradeEffect.Danio:
                _damageLevel++;
                break;
            case UpgradeSO.UpgradeEffect.AutoClicker:
                _autoClickLevel++;
                break;
            case UpgradeSO.UpgradeEffect.Mascota:
                if (tapButtonImage != null && upgrade.icono != null)
                    tapButtonImage.sprite = upgrade.icono;
                break;
        }
    }

    public int GetUpgradeLevel(UpgradeSO upgrade)
    {
        if (upgrade == null) return 0;
        return PlayerPrefs.GetInt(GetUpgradeKey(upgrade), 0);
    }

    public int GetUpgradeCost(UpgradeSO upgrade)
    {
        return upgrade == null ? 0 : upgrade.GetCostForLevel(GetUpgradeLevel(upgrade));
    }

    private void SetUpgradeLevel(UpgradeSO upgrade, int level)
    {
        PlayerPrefs.SetInt(GetUpgradeKey(upgrade), Mathf.Max(0, level));
    }

    private string GetUpgradeKey(UpgradeSO upgrade)
    {
        return $"TapMon_Upgrade_{upgrade.name}";
    }

    private void ApplyLoadedUpgrades()
    {
        _damageLevel = GetUpgradeLevel(mejoraDanio);
        _autoClickLevel = GetUpgradeLevel(mejoraAuto);

        if (tapButtonImage != null && mejoraMascota != null && GetUpgradeLevel(mejoraMascota) > 0)
            tapButtonImage.sprite = mejoraMascota.icono;
    }

    public void CheckMissionProgress(MissionType type, int amount)
    {
        if (amount <= 0) return;

        bool changed = false;
        for (int index = 0; index < dailyMissions.Count; index++)
        {
            DailyMission mission = dailyMissions[index];
            if (mission == null || mission.missionType != type || IsMissionClaimed(mission)) continue;

            int progress = GetMissionProgress(mission);
            int updatedProgress = Mathf.Min(mission.target, progress + amount);
            if (updatedProgress == progress) continue;

            PlayerPrefs.SetInt(GetMissionProgressKey(mission), updatedProgress);
            changed = true;
        }

        if (changed)
        {
            PlayerPrefs.Save();
            OnDailyMissionsChanged?.Invoke();
        }
    }

    public int GetMissionProgress(DailyMission mission)
    {
        return mission == null ? 0 : PlayerPrefs.GetInt(GetMissionProgressKey(mission), 0);
    }

    public bool IsMissionClaimed(DailyMission mission)
    {
        return mission != null && PlayerPrefs.GetInt(GetMissionClaimedKey(mission), 0) == 1;
    }

    public bool CanClaimMission(DailyMission mission)
    {
        return mission != null && !IsMissionClaimed(mission) && GetMissionProgress(mission) >= mission.target;
    }

    public bool ClaimMission(DailyMission mission)
    {
        if (!CanClaimMission(mission)) return false;

        PlayerPrefs.SetInt(GetMissionClaimedKey(mission), 1);
        PlayerPrefs.Save();
        AddCoins(mission.reward);
        OnDailyMissionsChanged?.Invoke();
        return true;
    }

    public void ResetDailyMissions()
    {
        long lastResetTicks = (long)(uint)PlayerPrefs.GetInt("TapMon_DailyMissionResetTicksHigh", 0) << 32;
        lastResetTicks |= (uint)PlayerPrefs.GetInt("TapMon_DailyMissionResetTicksLow", 0);
        long nowTicks = DateTime.UtcNow.Ticks;

        if (lastResetTicks > 0 && nowTicks - lastResetTicks < TimeSpan.FromHours(24).Ticks) return;

        for (int index = 0; index < dailyMissions.Count; index++)
        {
            DailyMission mission = dailyMissions[index];
            if (mission == null) continue;
            PlayerPrefs.SetInt(GetMissionProgressKey(mission), 0);
            PlayerPrefs.SetInt(GetMissionClaimedKey(mission), 0);
        }

        PlayerPrefs.SetInt("TapMon_DailyMissionResetTicksHigh", (int)(nowTicks >> 32));
        PlayerPrefs.SetInt("TapMon_DailyMissionResetTicksLow", (int)nowTicks);
        PlayerPrefs.Save();
        OnDailyMissionsChanged?.Invoke();
    }

    private string GetMissionProgressKey(DailyMission mission)
    {
        return $"TapMon_DailyMission_{mission.name}_Progress";
    }

    private string GetMissionClaimedKey(DailyMission mission)
    {
        return $"TapMon_DailyMission_{mission.name}_Claimed";
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