using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Micro-animaciones modernas de feedback táctil para botones UI:
/// - Hover: elevación y escala ligera (1.02)
/// - Press: compresión táctil (0.98) con respuesta elástica
/// </summary>
public class UIButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Vector3 hoverScale = new Vector3(1.02f, 1.02f, 1.02f);
    [SerializeField] private Vector3 pressedScale = new Vector3(0.98f, 0.98f, 0.98f);
    [SerializeField] private float animationSpeed = 16f;

    private Vector3 _originalScale;
    private Coroutine _animateCoroutine;
    private bool _isHovered;
    private bool _isPressed;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = _originalScale;
        _isHovered = false;
        _isPressed = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Selectable selectable = GetComponent<Selectable>();
        if (selectable != null && !selectable.interactable) return;

        _isHovered = true;
        if (!_isPressed)
        {
            StartScaleAnimation(hoverScale);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Selectable selectable = GetComponent<Selectable>();
        if (selectable != null && !selectable.interactable) return;

        _isHovered = false;
        if (!_isPressed)
        {
            StartScaleAnimation(_originalScale);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Selectable selectable = GetComponent<Selectable>();
        if (selectable != null && !selectable.interactable) return;

        _isPressed = true;
        StartScaleAnimation(pressedScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Selectable selectable = GetComponent<Selectable>();
        if (selectable != null && !selectable.interactable) return;

        _isPressed = false;
        StartScaleAnimation(_isHovered ? hoverScale : _originalScale);
    }

    private void StartScaleAnimation(Vector3 targetScale)
    {
        if (_animateCoroutine != null) StopCoroutine(_animateCoroutine);
        _animateCoroutine = StartCoroutine(AnimateScale(targetScale));
    }

    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.0005f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
            yield return null;
        }
        transform.localScale = targetScale;
    }

    /// <summary>
    /// Adjunta el animador de botones a cualquier GameObject si aún no lo tiene.
    /// </summary>
    public static UIButtonAnimator Attach(GameObject target)
    {
        if (target == null) return null;
        UIButtonAnimator animator = target.GetComponent<UIButtonAnimator>();
        if (animator == null) animator = target.AddComponent<UIButtonAnimator>();
        return animator;
    }
}
