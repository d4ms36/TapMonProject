using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Crea y actualiza las tarjetas de mejoras en la tienda estilo "Modern Fantasy Game UI Kit" con enableAutoSizing habilitado
/// en todos los textos dinámicos (nombres 14-22pt, insignias 12-18pt, costos 14-22pt, botones 14-22pt).
/// </summary>
public class UpgradeStoreUI : MonoBehaviour
{
    private static readonly Color CardColor = new Color(0.110f, 0.169f, 0.212f, 0.96f);     // #1C2B36 - Pizarra Metálica
    private static readonly Color ButtonActiveColor = new Color(1.000f, 0.482f, 0.000f, 1f); // #FF7B00 - Ámbar Fuego
    private static readonly Color ButtonDisabledColor = new Color(0.12f, 0.16f, 0.20f, 0.6f);
    private static readonly Color GoldColor = new Color(1.000f, 0.667f, 0.000f, 1f);          // #FFAA00 - Oro Recompensa
    private static readonly Color BronzeColor = new Color(0.831f, 0.686f, 0.216f, 1f);        // #D4AF37 - Bronce Místico
    private static readonly Color CyanGemColor = new Color(0.000f, 0.898f, 1.000f, 1f);       // #00E5FF - Cian Mágico
    private static readonly Color TextSecondaryColor = new Color(0.627f, 0.698f, 0.776f, 1f);  // #A0B2C6 - Plata / Azul Místico
    private static readonly Color BadgeColor = new Color(0.078f, 0.122f, 0.153f, 1f);        // #141F27

    [SerializeField] private UpgradeSO[] mejoras;
    [SerializeField] private Transform shopRoot;

    private GameManager _gameManager;
    private readonly Button[] _buttons = new Button[3];
    private readonly TMP_Text[] _nameLabels = new TMP_Text[3];
    private readonly TMP_Text[] _levelLabels = new TMP_Text[3];
    private readonly TMP_Text[] _costLabels = new TMP_Text[3];
    private readonly RectTransform[] _cardRects = new RectTransform[3];
    private bool _initialized;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        Initialize();
    }

    public void Configure(Transform root)
    {
        _gameManager = GameManager.Instance;
        if (_initialized)
        {
            MoveContentTo(root);
            return;
        }

        shopRoot = root;
        Initialize();
    }

    private void MoveContentTo(Transform root)
    {
        if (shopRoot == root) return;
        while (shopRoot != null && shopRoot.childCount > 0)
            shopRoot.GetChild(0).SetParent(root, false);
        if (shopRoot != null) Destroy(shopRoot.gameObject);
        shopRoot = root;
    }

    private void Initialize()
    {
        if (_initialized || _gameManager == null || mejoras == null || mejoras.Length < 3) return;

        CreateStoreLayout();
        _gameManager.OnCoinsChanged.AddListener(_ => RefreshStore());
        _gameManager.OnUpgradePurchased.AddListener(_ => RefreshStore());
        RefreshStore();
        _initialized = true;
    }

    private void CreateStoreLayout()
    {
        if (shopRoot == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null) return;
            GameObject root = new GameObject("Tienda", typeof(RectTransform));
            shopRoot = root.transform;
            shopRoot.SetParent(canvas.transform, false);
            RectTransform rootRect = (RectTransform)shopRoot;
            rootRect.anchorMin = new Vector2(0.05f, 0.13f);
            rootRect.anchorMax = new Vector2(0.95f, 0.34f);
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;
        }

        for (int index = 0; index < 3; index++)
        {
            GameObject buttonObject = new GameObject($"Mejora_{mejoras[index].nombre}", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(shopRoot, false);
            RectTransform rect = (RectTransform)buttonObject.transform;
            rect.anchorMin = new Vector2(0f, 0.26f + (1f - (index + 1) / 3f) * 0.74f);
            rect.anchorMax = new Vector2(1f, 0.26f + (1f - index / 3f) * 0.74f);
            rect.offsetMin = new Vector2(4f, 3f);
            rect.offsetMax = new Vector2(-4f, -3f);

            Image background = buttonObject.GetComponent<Image>();
            background.color = CardColor;

            Shadow shadow = buttonObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
            shadow.effectDistance = new Vector2(0f, -5f);

            buttonObject.AddComponent<Outline>().effectColor = BronzeColor;
            _cardRects[index] = rect;

            Button button = buttonObject.GetComponent<Button>();
            UIButtonAnimator.Attach(buttonObject);

            int capturedIndex = index;
            button.onClick.AddListener(() => OnUpgradeClicked(capturedIndex));

            // 1. Nombre de la Mejora (Auto-Sizing: 14pt - 22pt)
            GameObject nameObject = new GameObject("Nombre", typeof(RectTransform), typeof(TextMeshProUGUI));
            nameObject.transform.SetParent(buttonObject.transform, false);
            RectTransform nameRect = (RectTransform)nameObject.transform;
            nameRect.anchorMin = new Vector2(0.04f, 0.52f);
            nameRect.anchorMax = new Vector2(0.6f, 0.95f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;

            TMP_Text nameLabel = nameObject.GetComponent<TMP_Text>();
            nameLabel.alignment = TextAlignmentOptions.Left;
            nameLabel.enableAutoSizing = true;
            nameLabel.fontSizeMin = 14f;
            nameLabel.fontSizeMax = 22f;
            nameLabel.fontStyle = FontStyles.Bold;
            nameLabel.color = Color.white;
            _nameLabels[index] = nameLabel;

            // 2. Insignia de Nivel Estilo Gema Mística Cian (Auto-Sizing: 12pt - 18pt)
            GameObject badgeObject = new GameObject("NivelBadge", typeof(RectTransform), typeof(Image));
            badgeObject.transform.SetParent(buttonObject.transform, false);
            RectTransform badgeRect = (RectTransform)badgeObject.transform;
            badgeRect.anchorMin = new Vector2(0.04f, 0.1f);
            badgeRect.anchorMax = new Vector2(0.38f, 0.48f);
            badgeRect.offsetMin = Vector2.zero;
            badgeRect.offsetMax = Vector2.zero;
            badgeObject.GetComponent<Image>().color = BadgeColor;
            badgeObject.AddComponent<Outline>().effectColor = CyanGemColor;

            GameObject levelObject = new GameObject("NivelTexto", typeof(RectTransform), typeof(TextMeshProUGUI));
            levelObject.transform.SetParent(badgeObject.transform, false);
            RectTransform levelRect = (RectTransform)levelObject.transform;
            levelRect.anchorMin = Vector2.zero;
            levelRect.anchorMax = Vector2.one;
            levelRect.offsetMin = Vector2.zero;
            levelRect.offsetMax = Vector2.zero;

            TMP_Text levelLabel = levelObject.GetComponent<TMP_Text>();
            levelLabel.alignment = TextAlignmentOptions.Center;
            levelLabel.enableAutoSizing = true;
            levelLabel.fontSizeMin = 12f;
            levelLabel.fontSizeMax = 18f;
            levelLabel.fontStyle = FontStyles.Bold;
            levelLabel.color = CyanGemColor;
            _levelLabels[index] = levelLabel;

            // 3. Costo y Acción Comprar en Oro Místico (Auto-Sizing: 14pt - 22pt)
            GameObject costObject = new GameObject("CostoTexto", typeof(RectTransform), typeof(TextMeshProUGUI));
            costObject.transform.SetParent(buttonObject.transform, false);
            RectTransform costRect = (RectTransform)costObject.transform;
            costRect.anchorMin = new Vector2(0.6f, 0.1f);
            costRect.anchorMax = new Vector2(0.96f, 0.9f);
            costRect.offsetMin = Vector2.zero;
            costRect.offsetMax = Vector2.zero;

            TMP_Text costLabel = costObject.GetComponent<TMP_Text>();
            costLabel.alignment = TextAlignmentOptions.Right;
            costLabel.enableAutoSizing = true;
            costLabel.fontSizeMin = 14f;
            costLabel.fontSizeMax = 22f;
            costLabel.fontStyle = FontStyles.Bold;
            costLabel.color = GoldColor;
            _costLabels[index] = costLabel;

            _buttons[index] = button;
        }

        CreateRewardedAdButton();
    }

    private void CreateRewardedAdButton()
    {
        GameObject buttonObject = new GameObject("VerAnuncioRecompensado", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(shopRoot, false);
        RectTransform rect = (RectTransform)buttonObject.transform;
        rect.anchorMin = new Vector2(0f, 0.02f);
        rect.anchorMax = new Vector2(1f, 0.22f);
        rect.offsetMin = new Vector2(4f, 2f);
        rect.offsetMax = new Vector2(-4f, -2f);

        Image background = buttonObject.GetComponent<Image>();
        background.color = ButtonActiveColor;

        Shadow shadow = buttonObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
        shadow.effectDistance = new Vector2(0f, -5f);

        buttonObject.AddComponent<Outline>().effectColor = BronzeColor;

        Button button = buttonObject.GetComponent<Button>();
        UIButtonAnimator.Attach(buttonObject);
        button.onClick.AddListener(ShowRewardedAd);

        GameObject labelObject = new GameObject("Texto", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);
        RectTransform labelRect = (RectTransform)labelObject.transform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TMP_Text label = labelObject.GetComponent<TMP_Text>();
        label.text = "Ver Anuncio <color=#FFAA00>+500 🪙</color>";
        label.alignment = TextAlignmentOptions.Center;
        label.enableAutoSizing = true;
        label.fontSizeMin = 14f;
        label.fontSizeMax = 22f;
        label.fontStyle = FontStyles.Bold;
        label.color = Color.white;
    }

    private void OnUpgradeClicked(int index)
    {
        if (_gameManager == null || index < 0 || index >= mejoras.Length) return;
        UpgradeSO upgrade = mejoras[index];
        if (upgrade == null) return;

        bool purchased = _gameManager.PurchaseUpgrade(upgrade);
        if (purchased && UITouchFeedback.Instance != null)
        {
            UITouchFeedback.Instance.FlashGoldCard(_cardRects[index]);
            UITouchFeedback.Instance.PlayCashSound();
            UITouchFeedback.Instance.TriggerVibration();

            Vector2 cardPos = _cardRects[index] != null ? _cardRects[index].position : (Vector2)Input.mousePosition;
            UITouchFeedback.Instance.SpawnFloatingText(cardPos, "¡Mejora Comprada!", CyanGemColor);
        }
    }

    private void ShowRewardedAd()
    {
        if (AdManager.Instance == null)
        {
            Debug.LogWarning("[UpgradeStoreUI] AdManager no está disponible.");
            return;
        }

        AdManager.Instance.ShowRewardedAd(success =>
        {
            if (!success)
                Debug.Log("[UpgradeStoreUI] Anuncio recompensado no disponible; se intentará cargar otro.");
        });
    }

    private void RefreshStore()
    {
        if (_gameManager == null) return;

        for (int index = 0; index < _nameLabels.Length; index++)
        {
            if (_nameLabels[index] == null || mejoras[index] == null) continue;

            int level = _gameManager.GetUpgradeLevel(mejoras[index]);
            int cost = _gameManager.GetUpgradeCost(mejoras[index]);
            bool canAfford = _gameManager.CurrentCoins >= cost;

            _nameLabels[index].text = mejoras[index].nombre;
            _levelLabels[index].text = $"Nivel {level}";
            _costLabels[index].text = canAfford ? $"{cost} 🪙" : $"<color=#A0B2C6>{cost} 🪙</color>";

            _buttons[index].interactable = canAfford;
            _buttons[index].GetComponent<Image>().color = canAfford ? CardColor : ButtonDisabledColor;
        }
    }
}
