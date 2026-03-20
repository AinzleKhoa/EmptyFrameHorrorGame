using System.Collections;
using UnityEngine;

public class L1H_LightFlickerController : MonoBehaviour
{
    [Header("Light Targets")]
    [SerializeField] private Light[] _lights;
    [SerializeField] private GameObject[] _lightVisuals;

    [Header("Initial Flicker")]
    [SerializeField] private int _initialFlickerCount = 3;
    [SerializeField] private float _initialOffTime = 0.08f;
    [SerializeField] private float _initialOnTime = 0.12f;

    [Header("Loop Flicker")]
    [SerializeField] private float _loopInterval = 15f;
    [SerializeField] private int _loopFlickerCount = 2;
    [SerializeField] private float _loopOffTime = 0.06f;
    [SerializeField] private float _loopOnTime = 0.1f;

    [Header("Exit Flicker")]
    [SerializeField] private int _exitFlickerCount = 4;
    [SerializeField] private float _exitOffTime = 0.05f;
    [SerializeField] private float _exitOnTime = 0.08f;

    private bool _isRunning;
    private Coroutine _loopCoroutine;

    private void Awake()
    {
        SetLights(false);
    }

    public void TurnOnWithFlicker()
    {
        if (_isRunning) return;
        _isRunning = true;

        if (_loopCoroutine != null)
            StopCoroutine(_loopCoroutine);

        StopAllCoroutines();
        StartCoroutine(InitialTurnOnRoutine());
    }

    public void FlickerAndTurnOff()
    {
        _isRunning = false;

        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
            _loopCoroutine = null;
        }

        StopAllCoroutines();
        StartCoroutine(FlickerAndTurnOffRoutine());
    }

    public void TurnOffCompletely()
    {
        _isRunning = false;

        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
            _loopCoroutine = null;
        }

        StopAllCoroutines();
        SetLights(false);
    }

    private IEnumerator InitialTurnOnRoutine()
    {
        for (int i = 0; i < _initialFlickerCount; i++)
        {
            SetLights(true);
            yield return new WaitForSeconds(_initialOnTime);

            SetLights(false);
            yield return new WaitForSeconds(_initialOffTime);
        }

        SetLights(true);
        _loopCoroutine = StartCoroutine(LoopFlickerRoutine());
    }

    private IEnumerator LoopFlickerRoutine()
    {
        while (_isRunning)
        {
            yield return new WaitForSeconds(_loopInterval);

            for (int i = 0; i < _loopFlickerCount; i++)
            {
                SetLights(false);
                yield return new WaitForSeconds(_loopOffTime);

                SetLights(true);
                yield return new WaitForSeconds(_loopOnTime);
            }

            SetLights(true);
        }
    }

    private IEnumerator FlickerAndTurnOffRoutine()
    {
        for (int i = 0; i < _exitFlickerCount; i++)
        {
            SetLights(false);
            yield return new WaitForSeconds(_exitOffTime);

            SetLights(true);
            yield return new WaitForSeconds(_exitOnTime);
        }

        SetLights(false);
    }

    private void SetLights(bool isOn)
    {
        if (_lights != null)
        {
            for (int i = 0; i < _lights.Length; i++)
            {
                if (_lights[i] != null)
                    _lights[i].enabled = isOn;
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
}