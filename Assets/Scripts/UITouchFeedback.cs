using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sistema de feedback visual estilo "Modern Fantasy Game UI Kit":
/// Partículas de magia cian/turquesa (#00E5FF) y polvo estelar dorado (#FFAA00),
/// textos flotantes épicos de 44pt, ráfagas de monedas metálicas y vibración háptica.
/// </summary>
public class UITouchFeedback : MonoBehaviour
{
    public static UITouchFeedback Instance { get; private set; }

    private Canvas _canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _canvas = FindObjectOfType<Canvas>();
    }

    /// <summary>
    /// Genera ráfagas de partículas místicas en cian fosforescente (#00E5FF), ámbar fuego y polvo estelar.
    /// </summary>
    public void SpawnTapParticles(Vector2 screenPos)
    {
        if (_canvas == null) _canvas = FindObjectOfType<Canvas>();
        if (_canvas == null) return;

        GameObject container = new GameObject("ParticleBurst", typeof(RectTransform));
        container.transform.SetParent(_canvas.transform, false);
        RectTransform containerRect = (RectTransform)container.transform;
        containerRect.position = screenPos;

        int particleCount = 16;
        Color[] fantasyColors = new Color[]
        {
            new Color(0.000f, 0.898f, 1.000f, 1f), // Cian Mágico (#00E5FF)
            new Color(1.000f, 0.667f, 0.000f, 1f), // Oro Místico (#FFAA00)
            new Color(1.000f, 0.482f, 0.000f, 1f), // Ámbar Fuego (#FF7B00)
            new Color(0.086f, 0.878f, 0.741f, 1f), // Turquesa Cristal (#16E0BD)
            new Color(1.000f, 1.000f, 1.000f, 1f)  // Destello Blanco
        };

        for (int i = 0; i < particleCount; i++)
        {
            GameObject p = new GameObject("MagicSpark", typeof(RectTransform), typeof(Image));
            p.transform.SetParent(container.transform, false);
            Image img = p.GetComponent<Image>();
            img.color = fantasyColors[Random.Range(0, fantasyColors.Length)];

            RectTransform r = (RectTransform)p.transform;
            float size = Random.Range(22f, 36f);
            r.sizeDelta = new Vector2(size, size);

            float angle = (i / (float)particleCount) * 360f + Random.Range(-25f, 25f);
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            float speed = Random.Range(160f, 320f);
            float lifetime = Random.Range(0.45f, 0.70f);

            StartCoroutine(AnimateParticle(r, img, dir * speed, lifetime));
        }

        Destroy(container, 0.8f);
    }

    private IEnumerator AnimateParticle(RectTransform rect, Image img, Vector2 velocity, float lifetime)
    {
        float elapsed = 0f;
        Vector3 startScale = rect.localScale;
        Color startColor = img.color;
        float rotSpeed = Random.Range(-450f, 450f);

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;

            rect.anchoredPosition += velocity * Time.deltaTime;
            rect.Rotate(0f, 0f, rotSpeed * Time.deltaTime);

            float scale = (t < 0.2f) ? Mathf.Lerp(0.6f, 1.5f, t / 0.2f) : Mathf.Lerp(1.5f, 0f, (t - 0.2f) / 0.8f);
            rect.localScale = startScale * scale;

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, Mathf.Pow(t, 2f));
            img.color = c;

            yield return null;
        }
    }

    /// <summary>
    /// Texto flotante `+X` estilo fantasía (44pt) con resplandor cian/dorado.
    /// </summary>
    public void SpawnFloatingText(Vector2 screenPos, string textContent, Color textColor)
    {
        if (_canvas == null) _canvas = FindObjectOfType<Canvas>();
        if (_canvas == null) return;

        GameObject obj = new GameObject("FloatingText", typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(_canvas.transform, false);

        RectTransform rect = (RectTransform)obj.transform;
        rect.position = screenPos + new Vector2(Random.Range(-20f, 20f), Random.Range(0f, 20f));
        rect.sizeDelta = new Vector2(320f, 85f);

        TMP_Text text = obj.GetComponent<TMP_Text>();
        text.text = textContent;
        text.fontSize = 44f;
        text.fontStyle = FontStyles.Bold;
        text.color = textColor;
        text.alignment = TextAlignmentOptions.Center;

        Outline outline = obj.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0.4f, 0.5f, 0.95f);
        outline.effectDistance = new Vector2(3f, -3f);

        Shadow shadow = obj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
        shadow.effectDistance = new Vector2(4f, -4f);

        StartCoroutine(AnimateFloatingText(rect, text, 0.75f));
    }

    private IEnumerator AnimateFloatingText(RectTransform rect, TMP_Text text, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = rect.position;
        Vector3 endPos = startPos + new Vector3(0f, 120f, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rect.position = Vector3.Lerp(startPos, endPos, t);

            float scale = (t < 0.25f) ? Mathf.Lerp(0.7f, 1.4f, t / 0.25f) : Mathf.Lerp(1.4f, 1.0f, (t - 0.25f) / 0.75f);
            rect.localScale = Vector3.one * scale;

            Color c = text.color;
            c.a = Mathf.Lerp(1f, 0f, Mathf.Pow(t, 2f));
            text.color = c;

            yield return null;
        }

        Destroy(rect.gameObject);
    }

    /// <summary>
    /// Destello dorado/cian místico en la tarjeta al comprar.
    /// </summary>
    public void FlashGoldCard(RectTransform cardRect)
    {
        if (cardRect == null) return;

        GameObject flashObj = new GameObject("MagicFlash", typeof(RectTransform), typeof(Image));
        flashObj.transform.SetParent(cardRect, false);

        RectTransform r = (RectTransform)flashObj.transform;
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;

        Image img = flashObj.GetComponent<Image>();
        img.color = new Color(0f, 0.898f, 1f, 0.8f);

        Outline outline = flashObj.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 0.843f, 0f, 1f);
        outline.effectDistance = new Vector2(4f, 4f);

        StartCoroutine(AnimateCardFlash(img, 0.4f));
    }

    private IEnumerator AnimateCardFlash(Image img, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Color c = img.color;
            c.a = Mathf.Lerp(0.8f, 0f, t);
            img.color = c;
            yield return null;
        }
        Destroy(img.gameObject);
    }

    /// <summary>
    /// Ráfaga de monedas místicas voladoras hacia el contador de monedas.
    /// </summary>
    public void SpawnCoinBurst(Vector2 startPos, Vector2 targetPos, int count = 18)
    {
        if (_canvas == null) _canvas = FindObjectOfType<Canvas>();
        if (_canvas == null) return;

        for (int i = 0; i < count; i++)
        {
            GameObject coin = new GameObject("CoinParticle", typeof(RectTransform), typeof(Image));
            coin.transform.SetParent(_canvas.transform, false);

            RectTransform r = (RectTransform)coin.transform;
            r.position = startPos + new Vector2(Random.Range(-45f, 45f), Random.Range(-45f, 45f));
            r.sizeDelta = new Vector2(38f, 38f);

            Image img = coin.GetComponent<Image>();
            img.color = new Color(1f, 0.667f, 0f, 1f);

            Outline outline = coin.AddComponent<Outline>();
            outline.effectColor = new Color(0.831f, 0.686f, 0.216f, 1f);

            float delay = i * 0.03f;
            StartCoroutine(AnimateCoinToTarget(r, targetPos, delay, 0.6f));
        }
    }

    private IEnumerator AnimateCoinToTarget(RectTransform rect, Vector2 targetPos, float delay, float duration)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        Vector3 startPos = rect.position;
        Vector3 midControl = (startPos + (Vector3)targetPos) / 2f + new Vector3(Random.Range(-130f, 130f), Random.Range(70f, 160f), 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 pos = Mathf.Pow(1 - t, 2) * startPos + 2 * (1 - t) * t * midControl + Mathf.Pow(t, 2) * (Vector3)targetPos;
            rect.position = pos;
            rect.localScale = Vector3.one * Mathf.Lerp(1.35f, 0.75f, t);

            yield return null;
        }

        Destroy(rect.gameObject);
    }

    public void TriggerVibration()
    {
#if UNITY_ANDROID || UNITY_IOS
        try
        {
            Handheld.Vibrate();
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[UITouchFeedback] Vibración no disponible: {ex.Message}");
        }
#endif
    }

    public void PlayCashSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.pitch = Random.Range(1.18f, 1.38f);
        audioSource.PlayOneShot(CreateChimeClip());
    }

    private AudioClip CreateChimeClip()
    {
        int sampleRate = 44100;
        float duration = 0.15f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];
        float frequency = 1046.50f; // C6 note

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = 1f - (i / (float)sampleCount);
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.32f;
        }

        AudioClip clip = AudioClip.Create("CashChime", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
