using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class MenuButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private TMP_Text _buttonText;

    [Header("Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _hoverColor = new Color(1f, 0.85f, 0.4f); // Warm Lantern Yellow
    [SerializeField] private Color _pressedColor = Color.gray;

    [Header("Animations")]
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _transitionSpeed = 10f;

    private Vector3 _originalScale;
    private Vector3 _targetScale;
    private Color _targetColor;

    void Awake()
    {
        if (_buttonText == null) _buttonText = GetComponentInChildren<TMP_Text>();
        _originalScale = transform.localScale;
        _targetScale = _originalScale;
        _targetColor = _normalColor;
    }

    void Update()
    {
        // Smoothly fade the color and scale every frame
        _buttonText.color = Color.Lerp(_buttonText.color, _targetColor, _transitionSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, _transitionSpeed * Time.deltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _targetColor = _hoverColor;
        _targetScale = _originalScale * _hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _targetColor = _normalColor;
        _targetScale = _originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _targetColor = _pressedColor;
        _targetScale = _originalScale * 0.95f; // Slight "push" effect
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _targetColor = _hoverColor;
        _targetScale = _originalScale * _hoverScale;
    }
}