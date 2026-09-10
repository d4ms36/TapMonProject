using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Genera los tres paneles de misiones diarias estilo "Modern Fantasy Game UI Kit" con enableAutoSizing habilitado
/// en todos los textos dinámicos (títulos 14-22pt, descripciones 11-16pt, porcentajes 12-18pt, botones 14-22pt).
/// </summary>
public class DailyMissionUI : MonoBehaviour
{
    private static readonly Color CardColor = new Color(0.110f, 0.169f, 0.212f, 0.96f);     // #1C2B36 - Pizarra Metálica
    private static readonly Color ProgressFillColor = new Color(0.000f, 0.898f, 1.000f, 1f); // #00E5FF - Cian Mágico
    private static readonly Color ProgressBgColor = new Color(0.04f, 0.07f, 0.10f, 0.95f);
    private static readonly Color ButtonActiveColor = new Color(1.000f, 0.482f, 0.000f, 1f); // #FF7B00 - Ámbar Fuego
    private static readonly Color ButtonDisabledColor = new Color(0.12f, 0.16f, 0.20f, 0.6f);
    private static readonly Color GoldColor = new Color(1.000f, 0.667f, 0.000f, 1f);          // #FFAA00 - Oro Recompensa
    private static readonly Color BronzeColor = new Color(0.831f, 0.686f, 0.216f, 1f);        // #D4AF37 - Bronce Místico
    private static readonly Color TextSecondaryColor = new Color(0.627f, 0.698f, 0.776f, 1f);  // #A0B2C6 - Plata / Azul Místico

    [SerializeField] private DailyMission[] missions;
    [SerializeField] private Transform missionsRoot;

    private GameManager _gameManager;
    private readonly Button[] _claimButtons = new Button[3];
    private readonly TMP_Text[] _titleLabels = new TMP_Text[3];
    private readonly TMP_Text[] _descLabels = new TMP_Text[3];
    private readonly Image[] _progressFills = new Image[3];
    private readonly TMP_Text[] _percentLabels = new TMP_Text[3];
    private readonly RectTransform[] _panelRects = new RectTransform[3];
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

        missionsRoot = root;
        Initialize();
    }

    private void MoveContentTo(Transform root)
    {
        if (missionsRoot == root) return;
        while (missionsRoot != null && missionsRoot.childCount > 0)
            missionsRoot.GetChild(0).SetParent(root, false);
        if (missionsRoot != null) Destroy(missionsRoot.gameObject);
        missionsRoot = root;
    }

    private void Initialize()
    {
        if (_initialized || _gameManager == null || missions == null || missions.Length < 3) return;

        CreateMissionPanels();
        _gameManager.OnDailyMissionsChanged.AddListener(RefreshMissions);
        _gameManager.OnCoinsChanged.AddListener(_ => RefreshMissions());
        RefreshMissions();
        _initialized = true;
    }

    private void CreateMissionPanels()
    {
        if (missionsRoot == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null) return;
            GameObject root = new GameObject("MisionesDiarias", typeof(RectTransform));
            missionsRoot = root.transform;
            missionsRoot.SetParent(canvas.transform, false);
            RectTransform rootRect = (RectTransform)missionsRoot;
            rootRect.anchorMin = new Vector2(0.05f, 0.13f);
            rootRect.anchorMax = new Vector2(0.95f, 0.34f);
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;
        }

        for (int index = 0; index < 3; index++)
        {
            GameObject panel = new GameObject($"Mision_{missions[index].missionName}", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(missionsRoot, false);
            RectTransform panelRect = (RectTransform)panel.transform;
            panelRect.anchorMin = new Vector2(0f, 1f - (index + 1) / 3f);
            panelRect.anchorMax = new Vector2(1f, 1f - index / 3f);
            panelRect.offsetMin = new Vector2(4f, 3f);
            panelRect.offsetMax = new Vector2(-4f, -3f);

            panel.GetComponent<Image>().color = CardColor;

            Shadow shadow = panel.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
            shadow.effectDistance = new Vector2(0f, -5f);

            panel.AddComponent<Outline>().effectColor = BronzeColor;
            _panelRects[index] = panelRect;

            // 1. Título y Recompensa Mística (Auto-Sizing: 14pt - 22pt)
            GameObject titleObject = new GameObject("Titulo", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObject.transform.SetParent(panel.transform, false);
            RectTransform titleRect = (RectTransform)titleObject.transform;
            titleRect.anchorMin = new Vector2(0.03f, 0.58f);
            titleRect.anchorMax = new Vector2(0.68f, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            TMP_Text title = titleObject.GetComponent<TMP_Text>();
            title.alignment = TextAlignmentOptions.Left;
            title.enableAutoSizing = true;
            title.fontSizeMin = 14f;
            title.fontSizeMax = 22f;
            title.fontStyle = FontStyles.Bold;
            title.color = Color.white;
            _titleLabels[index] = title;

            // 2. Descripción (Auto-Sizing: 11pt - 16pt)
            GameObject descObject = new GameObject("Descripcion", typeof(RectTransform), typeof(TextMeshProUGUI));
            descObject.transform.SetParent(panel.transform, false);
            RectTransform descRect = (RectTransform)descObject.transform;
            descRect.anchorMin = new Vector2(0.03f, 0.32f);
            descRect.anchorMax = new Vector2(0.68f, 0.58f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;

            TMP_Text desc = descObject.GetComponent<TMP_Text>();
            desc.alignment = TextAlignmentOptions.Left;
            desc.enableAutoSizing = true;
            desc.fontSizeMin = 11f;
            desc.fontSizeMax = 16f;
            desc.color = TextSecondaryColor;
            _descLabels[index] = desc;

            // 3. Barra de Progreso Cian Mágico (#00E5FF) con Porcentaje (Auto-Sizing: 12pt - 18pt)
            GameObject barBg = new GameObject("ProgressBg", typeof(RectTransform), typeof(Image));
            barBg.transform.SetParent(panel.transform, false);
            RectTransform barBgRect = (RectTransform)barBg.transform;
            barBgRect.anchorMin = new Vector2(0.03f, 0.08f);
            barBgRect.anchorMax = new Vector2(0.68f, 0.28f);
            barBgRect.offsetMin = Vector2.zero;
            barBgRect.offsetMax = Vector2.zero;
            barBg.GetComponent<Image>().color = ProgressBgColor;
            barBg.AddComponent<Outline>().effectColor = BronzeColor;

            GameObject barFill = new GameObject("ProgressFill", typeof(RectTransform), typeof(Image));
            barFill.transform.SetParent(barBg.transform, false);
            RectTransform fillRect = (RectTransform)barFill.transform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fillImage = barFill.GetComponent<Image>();
            fillImage.color = ProgressFillColor;
            _progressFills[index] = fillImage;

            GameObject percentObject = new GameObject("PercentText", typeof(RectTransform), typeof(TextMeshProUGUI));
            percentObject.transform.SetParent(barBg.transform, false);
            RectTransform percentRect = (RectTransform)percentObject.transform;
            percentRect.anchorMin = Vector2.zero;
            percentRect.anchorMax = Vector2.one;
            percentRect.offsetMin = Vector2.zero;
            percentRect.offsetMax = Vector2.zero;

            TMP_Text percentText = percentObject.GetComponent<TMP_Text>();
            percentText.alignment = TextAlignmentOptions.Center;
            percentText.enableAutoSizing = true;
            percentText.fontSizeMin = 12f;
            percentText.fontSizeMax = 18f;
            percentText.fontStyle = FontStyles.Bold;
            percentText.color = Color.white;
            _percentLabels[index] = percentText;

            // 4. Botón de Reclamar Ámbar Fuego (Auto-Sizing: 14pt - 22pt)
            GameObject buttonObject = new GameObject("Reclamar", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(panel.transform, false);
            RectTransform buttonRect = (RectTransform)buttonObject.transform;
            buttonRect.anchorMin = new Vector2(0.71f, 0.12f);
            buttonRect.anchorMax = new Vector2(0.97f, 0.88f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            buttonObject.GetComponent<Image>().color = ButtonActiveColor;

            Shadow btnShadow = buttonObject.AddComponent<Shadow>();
            btnShadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
            btnShadow.effectDistance = new Vector2(0f, -4f);

            buttonObject.AddComponent<Outline>().effectColor = BronzeColor;

            Button claimButton = buttonObject.GetComponent<Button>();
            UIButtonAnimator.Attach(buttonObject);

            int capturedIndex = index;
            claimButton.onClick.AddListener(() => OnClaimClicked(capturedIndex));

            GameObject buttonLabelObject = new GameObject("Texto", typeof(RectTransform), typeof(TextMeshProUGUI));
            buttonLabelObject.transform.SetParent(buttonObject.transform, false);
            RectTransform buttonLabelRect = (RectTransform)buttonLabelObject.transform;
            buttonLabelRect.anchorMin = Vector2.zero;
            buttonLabelRect.anchorMax = Vector2.one;
            buttonLabelRect.offsetMin = Vector2.zero;
            buttonLabelRect.offsetMax = Vector2.zero;

            TMP_Text buttonLabel = buttonLabelObject.GetComponent<TMP_Text>();
            buttonLabel.text = "Reclamar";
            buttonLabel.alignment = TextAlignmentOptions.Center;
            buttonLabel.enableAutoSizing = true;
            buttonLabel.fontSizeMin = 14f;
            buttonLabel.fontSizeMax = 22f;
            buttonLabel.fontStyle = FontStyles.Bold;
            buttonLabel.color = Color.white;

            _claimButtons[index] = claimButton;
        }
    }

    private void OnClaimClicked(int index)
    {
        if (_gameManager == null || index < 0 || index >= missions.Length) return;
        DailyMission mission = missions[index];
        if (mission == null) return;

        bool claimed = _gameManager.ClaimMission(mission);
        if (claimed && UITouchFeedback.Instance != null)
        {
            Vector2 startPos = _panelRects[index] != null ? _panelRects[index].position : (Vector2)Input.mousePosition;
            Vector2 coinDisplayPos = GameObject.Find("CoinDisplay") != null ? (Vector2)GameObject.Find("CoinDisplay").transform.position : startPos + new Vector2(0, 300);

            UITouchFeedback.Instance.SpawnCoinBurst(startPos, coinDisplayPos, 20);
            UITouchFeedback.Instance.PlayCashSound();
            UITouchFeedback.Instance.TriggerVibration();
            UITouchFeedback.Instance.SpawnFloatingText(startPos, $"+{mission.reward} 🪙", GoldColor);
        }
    }

    private void RefreshMissions()
    {
        if (_gameManager == null) return;

        for (int index = 0; index < missions.Length; index++)
        {
            DailyMission mission = missions[index];
            if (_titleLabels[index] == null || mission == null) continue;

            int progress = _gameManager.GetMissionProgress(mission);
            bool claimed = _gameManager.IsMissionClaimed(mission);
            float percent = Mathf.Clamp01((float)progress / mission.target);

            _titleLabels[index].text = $"{mission.missionName} <color=#FFAA00>+{mission.reward}🪙</color>";
            _descLabels[index].text = $"{mission.description} ({progress}/{mission.target})";

            if (_progressFills[index] != null)
            {
                RectTransform fillRect = (RectTransform)_progressFills[index].transform;
                fillRect.anchorMax = new Vector2(percent, 1f);
            }

            if (_percentLabels[index] != null)
            {
                _percentLabels[index].text = claimed ? "¡Completada!" : $"{Mathf.RoundToInt(percent * 100)}%";
            }

            bool canClaim = !claimed && progress >= mission.target;
            _claimButtons[index].interactable = canClaim;
            _claimButtons[index].GetComponent<Image>().color = claimed ? ButtonDisabledColor : (canClaim ? ButtonActiveColor : ButtonDisabledColor);
            _claimButtons[index].GetComponentInChildren<TMP_Text>().text = claimed ? "Reclamada" : "Reclamar";
        }
    }
}
