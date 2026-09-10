using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Compone y gestiona la UI/UX estilo "Modern Fantasy Game UI Kit" de TapMon con layout reestructurado (21% altura paneles),
/// auto-sizing proporcional en todos los TextMeshPro y margen de seguridad del 4%.
/// </summary>
public class TapMonMainMenuUI : MonoBehaviour
{
    // Paleta de Colores "Modern Fantasy Game UI Kit"
    private static readonly Color BackgroundColor = new Color(0.055f, 0.086f, 0.106f, 1f);     // #0E161B - Pizarra Ébano
    private static readonly Color CardBackgroundColor = new Color(0.110f, 0.169f, 0.212f, 0.96f); // #1C2B36 - Pizarra Metálica
    private static readonly Color ActiveTabColor = new Color(1.000f, 0.482f, 0.000f, 1f);      // #FF7B00 - Ámbar Fuego
    private static readonly Color InactiveTabColor = new Color(0.078f, 0.122f, 0.153f, 1f);    // #141F27 - Metálico Oscuro
    private static readonly Color MagicCyanColor = new Color(0.000f, 0.898f, 1.000f, 1f);     // #00E5FF - Cian Mágico
    private static readonly Color BronzeGoldColor = new Color(0.831f, 0.686f, 0.216f, 1f);    // #D4AF37 - Bronce Místico / Oro
    private static readonly Color RewardGoldColor = new Color(1.000f, 0.667f, 0.000f, 1f);    // #FFAA00 - Oro Recompensa
    private static readonly Color TextSecondaryColor = new Color(0.627f, 0.698f, 0.776f, 1f);  // #A0B2C6 - Plata / Azul Místico

    [Header("Referencias UI")]
    [SerializeField] private Button missionsTab;
    [SerializeField] private Button shopTab;
    [SerializeField] private GameObject missionsPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Sprite coinIcon;
    [SerializeField] private GameObject mascotPrefab;
    [SerializeField] private TMP_Text evolutionLabel;
    [SerializeField] private Image mascotImage;
    [SerializeField] private Animator mascotAnimator;

    private GameManager _gameManager;
    private Image _missionsTabImage;
    private Image _shopTabImage;
    private RectTransform _mascotRectTransform;
    private RectTransform _coinDisplayRect;
    private Image _evolutionProgressBarFill;
    private CanvasGroup _missionsCanvasGroup;
    private CanvasGroup _shopCanvasGroup;

    private Vector3 _mascotBaseScale = Vector3.one;
    private Coroutine _mascotPunchCoroutine;
    private Coroutine _coinPulseCoroutine;
    private Coroutine _tabTransitionCoroutine;

    private void Start()
    {
        _gameManager = GameManager.Instance;

        if (UITouchFeedback.Instance == null)
        {
            gameObject.AddComponent<UITouchFeedback>();
        }

        ConfigureCameraBackground();
        BuildLayout();
        ConfigureDynamicPanels();
        RegisterEvents();

        missionsTab.onClick.AddListener(ShowMissions);
        shopTab.onClick.AddListener(ShowShop);

        ShowMissions();
        UpdateEvolutionProgress();
    }

    private void Update()
    {
        AnimateMascotBreathing();
    }

    private void ConfigureCameraBackground()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = BackgroundColor;
        }
    }

    private void BuildLayout()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.localScale = Vector3.one;

        CreateBackground(canvas.transform);

        // 1. Contador de Monedas (Top Bar: 24-44pt auto-sizing)
        CreateCoinBar(canvas.transform);

        // 2. Mascota (Centro Absoluto: 44% a 78% de la pantalla)
        CreateMascotCenter(canvas.transform);

        // 3. Nivel de Evolución y Barra de Progreso (39% a 43% y 36% a 38%)
        CreateEvolutionProgressBar(canvas.transform);

        // 4. Paneles de Contenido (Expandidos a 21% de la pantalla: 13% a 34%)
        CreateContentPanels(canvas.transform);

        // 5. Pestañas de Navegación (Base: 5% a 12%)
        CreateNavigation(canvas.transform);
    }

    private void CreateBackground(Transform canvasTransform)
    {
        GameObject bg = new GameObject("BackgroundOverlay", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasTransform, false);
        bg.transform.SetAsFirstSibling();

        RectTransform rect = (RectTransform)bg.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = bg.GetComponent<Image>();
        img.color = BackgroundColor;
    }

    private void CreateCoinBar(Transform canvasTransform)
    {
        GameObject bar = CreatePanel("CoinBar", canvasTransform, new Vector2(0.05f, 0.88f), new Vector2(0.95f, 0.98f), CardBackgroundColor);

        Shadow shadow = bar.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.55f);
        shadow.effectDistance = new Vector2(0f, -7f);

        Outline outline = bar.AddComponent<Outline>();
        outline.effectColor = BronzeGoldColor;
        outline.effectDistance = new Vector2(2.5f, 2.5f);

        GameObject iconObject = new GameObject("CoinIcon", typeof(RectTransform), typeof(Image));
        iconObject.transform.SetParent(bar.transform, false);
        RectTransform iconRect = (RectTransform)iconObject.transform;
        iconRect.anchorMin = new Vector2(0.04f, 0.12f);
        iconRect.anchorMax = new Vector2(0.18f, 0.88f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = coinIcon;
        icon.color = RewardGoldColor;
        icon.preserveAspect = true;

        GameObject displayObject = GameObject.Find("CoinDisplay");
        if (displayObject != null)
        {
            displayObject.transform.SetParent(bar.transform, false);
            _coinDisplayRect = displayObject.GetComponent<RectTransform>();
            _coinDisplayRect.anchorMin = new Vector2(0.20f, 0f);
            _coinDisplayRect.anchorMax = new Vector2(0.96f, 1f);
            _coinDisplayRect.offsetMin = Vector2.zero;
            _coinDisplayRect.offsetMax = Vector2.zero;

            TMP_Text display = displayObject.GetComponent<TMP_Text>();
            display.enableAutoSizing = true;
            display.fontSizeMin = 24f;
            display.fontSizeMax = 44f;
            display.fontStyle = FontStyles.Bold;
            display.color = RewardGoldColor;
            display.alignment = TextAlignmentOptions.Center;
        }
    }

    private void CreateMascotCenter(Transform canvasTransform)
    {
        GameObject tapButton = GameObject.Find("TapButton");
        if (tapButton == null) return;

        tapButton.transform.SetParent(canvasTransform, false);
        _mascotRectTransform = (RectTransform)tapButton.transform;

        _mascotRectTransform.anchorMin = new Vector2(0.15f, 0.44f);
        _mascotRectTransform.anchorMax = new Vector2(0.85f, 0.78f);
        _mascotRectTransform.offsetMin = Vector2.zero;
        _mascotRectTransform.offsetMax = Vector2.zero;

        if (mascotPrefab != null)
        {
            GameObject mascotObject = Instantiate(mascotPrefab, tapButton.transform, false);
            RectTransform mascotRect = mascotObject.GetComponent<RectTransform>();
            mascotRect.anchorMin = Vector2.zero;
            mascotRect.anchorMax = Vector2.one;
            mascotRect.offsetMin = new Vector2(16f, 16f);
            mascotRect.offsetMax = new Vector2(-16f, -16f);

            mascotImage = mascotObject.GetComponent<Image>();
            mascotAnimator = mascotObject.GetComponent<Animator>();
            GameManager.Instance?.SetTapButtonImage(mascotImage);

            Transform label = tapButton.transform.Find("TapButtonLabel");
            if (label != null) label.SetAsLastSibling();
        }
        else
        {
            mascotImage = tapButton.GetComponent<Image>();
            mascotAnimator = tapButton.GetComponent<Animator>();
        }

        Button button = tapButton.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnMascotTapped);
            UIButtonAnimator.Attach(tapButton);
        }
    }

    private void CreateEvolutionProgressBar(Transform canvasTransform)
    {
        GameObject labelObject = new GameObject("EvolutionLevel", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(canvasTransform, false);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.10f, 0.39f);
        labelRect.anchorMax = new Vector2(0.90f, 0.43f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        evolutionLabel = labelObject.GetComponent<TMP_Text>();
        evolutionLabel.text = "Nivel 1: Huevo Místico";
        evolutionLabel.enableAutoSizing = true;
        evolutionLabel.fontSizeMin = 22f;
        evolutionLabel.fontSizeMax = 32f;
        evolutionLabel.fontStyle = FontStyles.Bold;
        evolutionLabel.color = MagicCyanColor;
        evolutionLabel.alignment = TextAlignmentOptions.Center;

        Shadow textShadow = labelObject.AddComponent<Shadow>();
        textShadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
        textShadow.effectDistance = new Vector2(3f, -3f);

        // Barra de progreso de evolución mística (36% a 38%)
        GameObject barBg = CreatePanel("EvolutionBarBg", canvasTransform, new Vector2(0.15f, 0.36f), new Vector2(0.85f, 0.38f), new Color(0.04f, 0.07f, 0.10f, 0.95f));
        barBg.AddComponent<Outline>().effectColor = BronzeGoldColor;

        GameObject barFill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        barFill.transform.SetParent(barBg.transform, false);
        RectTransform fillRect = (RectTransform)barFill.transform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0.35f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        _evolutionProgressBarFill = barFill.GetComponent<Image>();
        _evolutionProgressBarFill.color = MagicCyanColor;
    }

    private void CreateNavigation(Transform canvasTransform)
    {
        GameObject navigation = CreatePanel("NavigationTabs", canvasTransform, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.12f), Color.clear);
        missionsTab = CreateTab("Misiones", navigation.transform, 0f);
        shopTab = CreateTab("Tienda", navigation.transform, 0.51f);

        _missionsTabImage = missionsTab.GetComponent<Image>();
        _shopTabImage = shopTab.GetComponent<Image>();

        UIButtonAnimator.Attach(missionsTab.gameObject);
        UIButtonAnimator.Attach(shopTab.gameObject);
    }

    private Button CreateTab(string labelText, Transform parent, float horizontalPosition)
    {
        GameObject tabObject = new GameObject(labelText, typeof(RectTransform), typeof(Image), typeof(Button));
        tabObject.transform.SetParent(parent, false);
        RectTransform rect = tabObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(horizontalPosition, 0f);
        rect.anchorMax = new Vector2(horizontalPosition + 0.49f, 1f);
        rect.offsetMin = new Vector2(4f, 4f);
        rect.offsetMax = new Vector2(-4f, -4f);

        Image img = tabObject.GetComponent<Image>();
        img.color = InactiveTabColor;

        Shadow tabShadow = tabObject.AddComponent<Shadow>();
        tabShadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
        tabShadow.effectDistance = new Vector2(0f, -5f);

        Outline outline = tabObject.AddComponent<Outline>();
        outline.effectColor = BronzeGoldColor;
        outline.effectDistance = new Vector2(1.5f, 1.5f);

        GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(tabObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.text = labelText;
        text.enableAutoSizing = true;
        text.fontSizeMin = 14f;
        text.fontSizeMax = 22f;
        text.fontStyle = FontStyles.Bold;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;

        return tabObject.GetComponent<Button>();
    }

    private void CreateContentPanels(Transform canvasTransform)
    {
        // Paneles expandidos a 21% de la altura total de pantalla (0.13f a 0.34f)
        missionsPanel = CreatePanel("MissionsPanel", canvasTransform, new Vector2(0.05f, 0.13f), new Vector2(0.95f, 0.34f), CardBackgroundColor);
        shopPanel = CreatePanel("ShopPanel", canvasTransform, new Vector2(0.05f, 0.13f), new Vector2(0.95f, 0.34f), CardBackgroundColor);

        ApplyFantasyPanelStyle(missionsPanel);
        ApplyFantasyPanelStyle(shopPanel);

        _missionsCanvasGroup = missionsPanel.AddComponent<CanvasGroup>();
        _shopCanvasGroup = shopPanel.AddComponent<CanvasGroup>();
    }

    private void ApplyFantasyPanelStyle(GameObject panelObj)
    {
        Shadow shadow = panelObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.55f);
        shadow.effectDistance = new Vector2(0f, -7f);

        Outline outline = panelObj.AddComponent<Outline>();
        outline.effectColor = BronzeGoldColor;
        outline.effectDistance = new Vector2(2f, 2f);
    }

    private void ConfigureDynamicPanels()
    {
        DailyMissionUI missionsUI = GetComponent<DailyMissionUI>();
        if (missionsUI != null) missionsUI.Configure(missionsPanel.transform);

        UpgradeStoreUI storeUI = GetComponent<UpgradeStoreUI>();
        if (storeUI != null) storeUI.Configure(shopPanel.transform);
    }

    private void RegisterEvents()
    {
        if (_gameManager != null)
        {
            _gameManager.OnCoinsChanged.AddListener(OnCoinsChangedHandler);
            _gameManager.OnUpgradePurchased.AddListener(_ => UpdateEvolutionProgress());
        }
    }

    private void OnMascotTapped()
    {
        if (_gameManager == null) return;

        _gameManager.AddTapCoins();
        int tapValue = _gameManager.GetCoinsPerTap();
        Vector2 tapPos = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;

        if (UITouchFeedback.Instance != null)
        {
            UITouchFeedback.Instance.SpawnTapParticles(tapPos);
            UITouchFeedback.Instance.SpawnFloatingText(tapPos, $"+{tapValue}", MagicCyanColor);
        }

        TriggerMascotPunchScale();
    }

    private void TriggerMascotPunchScale()
    {
        if (_mascotRectTransform == null) return;
        if (_mascotPunchCoroutine != null) StopCoroutine(_mascotPunchCoroutine);
        _mascotPunchCoroutine = StartCoroutine(AnimatePunchScale());
    }

    private IEnumerator AnimatePunchScale()
    {
        float elapsed = 0f;
        float duration = 0.18f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float scaleMultiplier = (t < 0.4f)
                ? Mathf.Lerp(0.92f, 1.08f, t / 0.4f)
                : Mathf.Lerp(1.08f, 1.0f, (t - 0.4f) / 0.6f);

            _mascotRectTransform.localScale = _mascotBaseScale * scaleMultiplier;
            yield return null;
        }

        _mascotRectTransform.localScale = _mascotBaseScale;
    }

    private void AnimateMascotBreathing()
    {
        if (_mascotRectTransform == null) return;

        float breathSpeed = 2.5f;
        float breathScaleY = 1f + Mathf.Sin(Time.time * breathSpeed) * 0.025f;
        float breathScaleX = 1f - Mathf.Sin(Time.time * breathSpeed) * 0.015f;

        _mascotBaseScale = new Vector3(breathScaleX, breathScaleY, 1f);

        if (_mascotPunchCoroutine == null)
        {
            _mascotRectTransform.localScale = _mascotBaseScale;
        }
    }

    private void OnCoinsChangedHandler(int currentCoins)
    {
        if (_coinDisplayRect == null) return;
        if (_coinPulseCoroutine != null) StopCoroutine(_coinPulseCoroutine);
        _coinPulseCoroutine = StartCoroutine(AnimateCoinPulse());
    }

    private IEnumerator AnimateCoinPulse()
    {
        float elapsed = 0f;
        float duration = 0.25f;
        Vector3 baseScale = Vector3.one;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float scale = (t < 0.4f) ? Mathf.Lerp(1.0f, 1.35f, t / 0.4f) : Mathf.Lerp(1.35f, 1.0f, (t - 0.4f) / 0.6f);
            _coinDisplayRect.localScale = baseScale * scale;
            yield return null;
        }

        _coinDisplayRect.localScale = baseScale;
    }

    public void ShowMissions()
    {
        if (_tabTransitionCoroutine != null) StopCoroutine(_tabTransitionCoroutine);
        _tabTransitionCoroutine = StartCoroutine(AnimateTabSwitch(true));

        if (_missionsTabImage != null) _missionsTabImage.color = ActiveTabColor;
        if (_shopTabImage != null) _shopTabImage.color = InactiveTabColor;
    }

    public void ShowShop()
    {
        if (_tabTransitionCoroutine != null) StopCoroutine(_tabTransitionCoroutine);
        _tabTransitionCoroutine = StartCoroutine(AnimateTabSwitch(false));

        if (_missionsTabImage != null) _missionsTabImage.color = InactiveTabColor;
        if (_shopTabImage != null) _shopTabImage.color = ActiveTabColor;
    }

    private IEnumerator AnimateTabSwitch(bool showMissions)
    {
        float duration = 0.30f;
        float elapsed = 0f;

        CanvasGroup showGroup = showMissions ? _missionsCanvasGroup : _shopCanvasGroup;
        CanvasGroup hideGroup = showMissions ? _shopCanvasGroup : _missionsCanvasGroup;

        GameObject showObj = showMissions ? missionsPanel : shopPanel;
        GameObject hideObj = showMissions ? shopPanel : missionsPanel;

        RectTransform showRect = showObj != null ? (RectTransform)showObj.transform : null;
        Vector2 startPos = showMissions ? new Vector2(-40f, 0f) : new Vector2(40f, 0f);

        if (showObj != null) showObj.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (hideGroup != null) hideGroup.alpha = 1f - t;
            if (showGroup != null) showGroup.alpha = t;

            if (showRect != null)
            {
                showRect.anchoredPosition = Vector2.Lerp(startPos, Vector2.zero, t);
            }

            yield return null;
        }

        if (hideGroup != null) hideGroup.alpha = 0f;
        if (showGroup != null) showGroup.alpha = 1f;
        if (showRect != null) showRect.anchoredPosition = Vector2.zero;
        if (hideObj != null) hideObj.SetActive(false);
    }

    private void UpdateEvolutionProgress()
    {
        if (_gameManager == null) return;
        int totalTaps = _gameManager.TotalTaps;
        int level = Mathf.Max(1, (totalTaps / 100) + 1);
        float progress = (totalTaps % 100) / 100f;

        if (evolutionLabel != null)
        {
            evolutionLabel.text = $"Nivel {level} | {totalTaps % 100}/100";
        }

        if (_evolutionProgressBarFill != null)
        {
            RectTransform fillRect = (RectTransform)_evolutionProgressBarFill.transform;
            fillRect.anchorMax = new Vector2(Mathf.Clamp01(progress), 1f);
        }
    }

    private GameObject CreatePanel(string objectName, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject panel = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = color;
        return panel;
    }
}
