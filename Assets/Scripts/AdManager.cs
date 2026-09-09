using System;
using UnityEngine;

#if ADMOB_ENABLED
using GoogleMobileAds.Api;
#endif

/// <summary>
/// Gestiona Banner, Interstitial y anuncios recompensados de Google AdMob.
/// El SDK se compila solo cuando se define ADMOB_ENABLED.
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [Header("Configuracion")]
    [Tooltip("Mantener activado durante desarrollo. Desactivar solo para produccion.")]
    [SerializeField] private bool useTestAds = true;

    [Tooltip("Recompensa entregada al completar un anuncio recompensado.")]
    [SerializeField] private int rewardedCoins = 500;

    [Header("IDs de aplicacion")]
    [SerializeField] private string androidAppId = "TODO_REEMPLAZAR_APP_ID: ca-app-pub-9771091826001795~9872406537";

    [Header("IDs reales de TapMon")]
    [SerializeField] private string productionBannerId = "TODO_REEMPLAZAR_BANNER_ID: ca-app-pub-9771091826001795/2154181115";
    [SerializeField] private string productionInterstitialId = "TODO_REEMPLAZAR_INTERSTITIAL_ID: ca-app-pub-9771091826001795/2971956266";
    [SerializeField] private string productionRewardedId = "TODO_REEMPLAZAR_REWARDED_ID: ca-app-pub-9771091826001795/1609783455";

#if ADMOB_ENABLED
    private const string TestBannerId = "ca-app-pub-3940256099942544/6300978111";
    private const string TestInterstitialId = "ca-app-pub-3940256099942544/1033173712";
    private const string TestRewardedId = "ca-app-pub-3940256099942544/5224354917";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
#endif

    private bool rewardDelivered;

#if ADMOB_ENABLED
    private string BannerId => useTestAds ? TestBannerId : productionBannerId;
    private string InterstitialId => useTestAds ? TestInterstitialId : productionInterstitialId;
    private string RewardedId => useTestAds ? TestRewardedId : productionRewardedId;
#endif

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
#if ADMOB_ENABLED
        MobileAds.Initialize(_ =>
        {
            Debug.Log("[AdManager] SDK de AdMob inicializado.");
            LoadBanner();
            LoadInterstitial();
            LoadRewardedAd();
        });
#else
        Debug.Log("[AdManager] AdMob desactivado. Define ADMOB_ENABLED despues de instalar el SDK.");
#endif
    }

    /// <summary>Carga un banner adaptable anclado en la parte inferior.</summary>
    public void LoadBanner()
    {
#if ADMOB_ENABLED && UNITY_ANDROID
        DestroyBanner();
        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        bannerView = new BannerView(BannerId, adaptiveSize, AdPosition.Bottom);
        bannerView.OnAdLoaded += () => Debug.Log("[AdManager] Banner cargado.");
        bannerView.OnAdFailedToLoad += error => Debug.LogWarning($"[AdManager] Error en banner: {error.LoadAdError.GetMessage()}");
        bannerView.LoadAd(new AdRequest.Builder().Build());
#endif
    }

    /// <summary>Carga un interstitial para mostrarlo entre escenas o misiones.</summary>
    public void LoadInterstitial()
    {
#if ADMOB_ENABLED && UNITY_ANDROID
        interstitialAd = new InterstitialAd(InterstitialId);
        interstitialAd.OnAdLoaded += () => Debug.Log("[AdManager] Interstitial cargado.");
        interstitialAd.OnAdFailedToLoad += error => Debug.LogWarning($"[AdManager] Error en interstitial: {error.LoadAdError.GetMessage()}");
        interstitialAd.OnAdClosed += () =>
        {
            interstitialAd.Destroy();
            LoadInterstitial();
        };
        interstitialAd.LoadAd(new AdRequest.Builder().Build());
#endif
    }

    /// <summary>Muestra el interstitial si esta cargado; si no, solicita otro.</summary>
    public void ShowInterstitial()
    {
#if ADMOB_ENABLED && UNITY_ANDROID
        if (interstitialAd != null && interstitialAd.IsLoaded())
        {
            interstitialAd.Show();
            return;
        }

        LoadInterstitial();
#else
        Debug.Log("[AdManager] Interstitial omitido porque AdMob esta desactivado.");
#endif
    }

    /// <summary>Carga el anuncio recompensado y prepara una recompensa de 500 monedas.</summary>
    public void LoadRewardedAd()
    {
#if ADMOB_ENABLED && UNITY_ANDROID
        rewardedAd = new RewardedAd(RewardedId);
        rewardedAd.OnAdLoaded += () => Debug.Log("[AdManager] Rewarded cargado.");
        rewardedAd.OnAdFailedToLoad += error => Debug.LogWarning($"[AdManager] Error en rewarded: {error.LoadAdError.GetMessage()}");
        rewardedAd.OnAdClosed += () =>
        {
            rewardedAd.Destroy();
            LoadRewardedAd();
        };
        rewardedAd.OnUserEarnedReward += (_, args) =>
        {
            if (rewardDelivered) return;

            rewardDelivered = true;
            Debug.Log($"[AdManager] Recompensa recibida: {args.GetReward()}. Entregando {rewardedCoins} monedas.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRewardEarned();
            }
        };
        rewardedAd.LoadAd(new AdRequest.Builder().Build());
#endif
    }

    /// <summary>
    /// Muestra un rewarded y notifica si pudo abrirse. La moneda se entrega
    /// desde OnUserEarnedReward, nunca solo por pulsar el boton.
    /// </summary>
    public void ShowRewardedAd(Action<bool> onShown)
    {
#if ADMOB_ENABLED && UNITY_ANDROID
        if (rewardedAd != null && rewardedAd.IsLoaded())
        {
            rewardDelivered = false;
            rewardedAd.Show();
            onShown?.Invoke(true);
            return;
        }

        LoadRewardedAd();
        onShown?.Invoke(false);
#else
        Debug.LogWarning("[AdManager] AdMob desactivado. Instala el SDK y define ADMOB_ENABLED.");
        onShown?.Invoke(false);
#endif
    }

#if ADMOB_ENABLED
    private void DestroyBanner()
    {
        if (bannerView == null) return;
        bannerView.Destroy();
        bannerView = null;
    }
#endif

    private void OnDestroy()
    {
        if (Instance != this) return;

#if ADMOB_ENABLED
        DestroyBanner();
        interstitialAd?.Destroy();
        rewardedAd?.Destroy();
#endif
        Instance = null;
    }
}