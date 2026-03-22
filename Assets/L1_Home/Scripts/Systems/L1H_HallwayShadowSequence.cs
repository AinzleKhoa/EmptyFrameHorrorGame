using System.Collections;
using UnityEngine;

public class L1H_HallwayShadowSequence : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private string _playerTag = "Player";

    [Header("Hallway Lights")]
    [SerializeField] private Light[] _hallwayLights;
    [SerializeField] private GameObject[] _lightVisuals;

    [Header("Red Warning Light")]
    [SerializeField] private Light _redLight;
    [SerializeField] private GameObject _redLightVisual;

    [Header("Shadow Figure")]
    [SerializeField] private GameObject _shadowFigure;

    [Header("Timing")]
    [SerializeField] private float _delayBeforeShadowAppears = 0.1f;
    [SerializeField] private float _shadowVisibleTime = 0.8f;
    [SerializeField] private bool _triggerOnlyOnce = true;

    [Header("Flicker Timing")]
    [SerializeField] private float _flickerOffTime = 0.06f;
    [SerializeField] private float _flickerOnTime = 0.08f;

    private bool _triggered;

    private void Awake()
    {
        if (_shadowFigure != null)
            _shadowFigure.SetActive(false);

        SetRedLight(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggerOnlyOnce && _triggered) return;
        if (!other.transform.root.CompareTag(_playerTag)) return;

        _triggered = true;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        yield return StartCoroutine(FlickerOnceFast());

        yield return new WaitForSeconds(_delayBeforeShadowAppears);

        if (_shadowFigure != null)
            _shadowFigure.SetActive(true);

        SetRedLight(true);

        yield return new WaitForSeconds(_shadowVisibleTime);

        yield return StartCoroutine(FlickerOnceFast());

        if (_shadowFigure != null)
            _shadowFigure.SetActive(false);

        SetRedLight(false);
    }

    private IEnumerator FlickerOnceFast()
    {
        SetLights(false);
        yield return new WaitForSeconds(_flickerOffTime);

        SetLights(true);
        yield return new WaitForSeconds(_flickerOnTime);

        SetLights(false);
        yield return new WaitForSeconds(_flickerOffTime);

        SetLights(true);
    }

    private void SetLights(bool isOn)
    {
        if (_hallwayLights != null)
        {
            for (int i = 0; i < _hallwayLights.Length; i++)
            {
                if (_hallwayLights[i] != null)
                    _hallwayLights[i].enabled = isOn;
            }
        }

        if (_lightVisuals != null)
        {
            for (int i = 0; i < _lightVisuals.Length; i++)
            {
                if (_lightVisuals[i] != null)
                    _lightVisuals[i].SetActive(isOn);
            }
        }
    }

    private void SetRedLight(bool isOn)
    {
        if (_redLight != null)
            _redLight.enabled = isOn;

        if (_redLightVisual != null)
            _redLightVisual.SetActive(isOn);
    }
}