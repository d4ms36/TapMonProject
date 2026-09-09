using System;
using GoogleMobileAds.Api;
using UnityEngine;

/// <summary>
/// Gestiona los formatos de Google Mobile Ads con la API 9.x.
/// Mantener useTestAds activado hasta terminar las pruebas en dispositivo.
/// </summary>
public class AdManager : MonoBehaviour
{
    private const string ProductionAppId = "ca-app-pub-9771091826001795~9872406537";
    private const string ProductionBannerId = "ca-app-pub-9771091826001795/2154181115";
    private const string ProductionInterstitialId = "ca-app-pub-9771091826001795/2971956266";
    private const string ProductionRewardedId = "ca-app-pub-9771091826001795/1609783455";

    private const string TestAppId = "ca-app-pub-3940256099942544~3347511713";
    private const string TestBannerId = "ca-app-pub-3940256099942544/6300978111";
    private const string TestInterstitialId = "ca-app-pub-3940256099942544/1033173712";
    private const string TestRewardedId = "ca-app-pub-3940256099942544/5224354917";

    private static AdManager instance;
    public static AdManager Instance { get { return instance; } }

    [SerializeField] private bool useTestAds = true;
    [SerializeField] private int rewardedCoins = 500;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private bool rewardDelivered;

    private string BannerAdUnitId => useTestAds ? TestBannerId : ProductionBannerId;
    private string InterstitialAdUnitId => useTestAds ? TestInterstitialId : ProductionInterstitialId;
    private string RewardedAdUnitId => useTestAds ? TestRewardedId : ProductionRewardedId;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        string appIdToUse = useTestAds ? TestAppId : ProductionAppId;
        MobileAds.Initialize(_ =>
        {
            Debug.Log($"[AdManager] SDK inicializado con App ID {appIdToUse}.");
            LoadBanner();
            LoadInterstitial();
            LoadRewardedAd();
        });
    }

    public void LoadBanner()
    {
        DestroyBanner();
        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        bannerView = new BannerView(BannerAdUnitId, adaptiveSize, AdPosition.Bottom);
        bannerView.OnBannerAdLoaded += () => Debug.Log("[AdManager] Banner cargado.");
        bannerView.OnBannerAdLoadFailed += error => Debug.LogWarning($"[AdManager] Error banner: {error.GetMessage()}");
        bannerView.LoadAd(new AdRequest());
    }

    public void LoadInterstitial()
    {
        interstitialAd?.Destroy();
        interstitialAd = null;
        InterstitialAd.Load(InterstitialAdUnitId, new AdRequest(), (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"[AdManager] Error interstitial: {error?.GetMessage()}");
                return;
            }

            interstitialAd = ad;
            interstitialAd.OnAdFullScreenContentClosed += LoadInterstitial;
            interstitialAd.OnAdFullScreenContentFailed += loadError => Debug.LogWarning($"[AdManager] Error mostrando interstitial: {loadError.GetMessage()}");
            Debug.Log("[AdManager] Interstitial cargado.");
        });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            return;
        }

        Debug.Log("[AdManager] Interstitial no disponible; solicitando otro.");
        LoadInterstitial();
    }

    public void LoadRewardedAd()
    {
        rewardedAd?.Destroy();
        rewardedAd = null;
        RewardedAd.Load(RewardedAdUnitId, new AdRequest(), (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"[AdManager] Error rewarded: {error?.GetMessage()}");
                return;
            }

            rewardedAd = ad;
            rewardedAd.OnAdFullScreenContentClosed += LoadRewardedAd;
            rewardedAd.OnAdFullScreenContentFailed += loadError => Debug.LogWarning($"[AdManager] Error mostrando rewarded: {loadError.GetMessage()}");
            Debug.Log("[AdManager] Rewarded cargado.");
        });
    }

    public void ShowRewardedAd()
    {
        ShowRewardedAd(null);
    }

    public void ShowRewardedAd(Action<bool> onShown)
    {
        if (rewardedAd == null || !rewardedAd.CanShowAd())
        {
            Debug.Log("[AdManager] Rewarded no disponible; solicitando otro.");
            LoadRewardedAd();
            onShown?.Invoke(false);
            return;
        }

        rewardDelivered = false;
        rewardedAd.Show(reward =>
        {
            if (rewardDelivered) return;

            rewardDelivered = true;
            Debug.Log($"[AdManager] Recompensa recibida: {reward.Amount} {reward.Type}.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRewardEarned();
            }
            else
            {
                Debug.LogWarning($"[AdManager] GameManager no disponible para entregar {rewardedCoins} monedas.");
            }
        });
        onShown?.Invoke(true);
    }

    private void DestroyBanner()
    {
        if (bannerView == null) return;
        bannerView.Destroy();
        bannerView = null;
    }

    private void OnDestroy()
    {
        if (instance != this) return;

        DestroyBanner();
        interstitialAd?.Destroy();
        rewardedAd?.Destroy();
        instance = null;
    }
}