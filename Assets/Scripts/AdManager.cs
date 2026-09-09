using System;
using UnityEngine;

#if ADMOB_ENABLED
using GoogleMobileAds.Api;
#endif

/// <summary>
/// Gestor de publicidad para TapMon: Mi Compi Virtual.
/// Maneja banners e anuncios recompensados de AdMob.
/// </summary>
public class AdManager : MonoBehaviour
{
    private static AdManager instance;
    public static AdManager Instance { get { return instance; } }

#if ADMOB_ENABLED
    private BannerView bannerView;
    private RewardedAd rewardedAd;
#endif
    private bool isRewardPending = false;

    // IDs de prueba (cambiar por los reales antes de publicar)
    // Banner: ca-app-pub-3940256099942544/6300978111
    // Recompensado: ca-app-pub-3940256099942544/5224354917
    [SerializeField] private string bannerUnitId = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] private string rewardedUnitId = "ca-app-pub-3940256099942544/5224354917";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[AdManager] Inicializado");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
#if ADMOB_ENABLED
        // Inicializar SDK de AdMob
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("[AdManager] SDK de AdMob inicializado");
            LoadBanner();
            LoadRewardedAd();
        });
    #endif
    }

    /// <summary>
    /// Carga un banner en la parte inferior de la pantalla.
    /// </summary>
    public void LoadBanner()
    {
#if ADMOB_ENABLED
#if UNITY_ANDROID
        bannerView = new BannerView(bannerUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);

        bannerView.OnAdLoaded += () =>
        {
            Debug.Log("[AdManager] Banner cargado exitosamente");
        };

        bannerView.OnAdFailedToLoad += (error) =>
        {
            Debug.LogWarning($"[AdManager] Error cargando banner: {error.LoadAdError.GetMessage()}");
        };

        bannerView.OnAdOpenedEvent += () =>
        {
            Debug.Log("[AdManager] Banner abierto");
        };

        bannerView.OnAdClosedEvent += () =>
        {
            Debug.Log("[AdManager] Banner cerrado, recargando...");
            LoadBanner();
        };
#endif
#endif
    }

    /// <summary>
    /// Carga un anuncio recompensado.
    /// </summary>
    public void LoadRewardedAd()
    {
#if ADMOB_ENABLED
#if UNITY_ANDROID
        rewardedAd = new RewardedAd(rewardedUnitId);
        
        // Eventos del anuncio recompensado
        rewardedAd.OnAdLoaded += () =>
        {
            Debug.Log("[AdManager] Anuncio recompensado cargado");
        };

        rewardedAd.OnAdFailedToLoad += (error) =>
        {
            Debug.LogWarning($"[AdManager] Error cargando anuncio: {error.LoadAdError.GetMessage()}");
        };

        rewardedAd.OnAdOpenedEvent += () =>
        {
            Debug.Log("[AdManager] Anuncio recompensado abierto");
        };

        rewardedAd.OnAdClosedEvent += () =>
        {
            Debug.Log("[AdManager] Anuncio recompensado cerrado");
            LoadRewardedAd(); // Recargar para siguiente uso
        };

        rewardedAd.OnUserEarnedReward += (sender, args) =>
        {
            Debug.Log($"[AdManager] Recompensa ganada: {args.GetReward()}");
            isRewardPending = true;
            // Notificar al GameManager
            GameManager.Instance.OnRewardEarned();
        };

        rewardedAd.OnRewardedAdCompleted += () =>
        {
            Debug.Log("[AdManager] Anuncio recompensado completado");
        };

        rewardedAd.OnRewardedAdSkipped += () =>
        {
            Debug.Log("[AdManager] Anuncio recompensado saltado");
            isRewardPending = false;
        };

        // Cargar el anuncio
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
#endif
#endif
    }

    /// <summary>
    /// Muestra el anuncio recompensado y llama al callback con el resultado.
    /// </summary>
    /// <param name="onReward">Callback que recibe true si el anuncio se mostró y completó.</param>
    public void ShowRewardedAd(Action<bool> onReward)
    {
#if ADMOB_ENABLED
#if UNITY_ANDROID
        if (rewardedAd != null && rewardedAd.IsLoaded())
        {
            Debug.Log("[AdManager] Mostrando anuncio recompensado");
            rewardedAd.Show();
            isRewardPending = false;
            onReward?.Invoke(true);
        }
        else
        {
            Debug.LogWarning("[AdManager] Anuncio recompensado no disponible, intentando recargar...");
            LoadRewardedAd();
            onReward?.Invoke(false);
        }
#endif
#else
    Debug.LogWarning("[AdManager] AdMob está desactivado. Instala el SDK y define ADMOB_ENABLED para activarlo.");
    onReward?.Invoke(false);
#endif
    }

    /// <summary>
    /// Verifica si la recompensa fue recibida.
    /// </summary>
    public bool ConsumeReward()
    {
        bool hasReward = isRewardPending;
        isRewardPending = false;
        return hasReward;
    }

    private void OnDestroy()
    {
#if ADMOB_ENABLED
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
#endif
    }
}