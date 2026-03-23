using UnityEngine;
using UnityEngine.Events;

public class HospitalTutorialPopup : MonoBehaviour
{
    [Header("Root UI")]
    [SerializeField] private GameObject _root;

    [Header("Optional freeze while showing")]
    [SerializeField] private bool _freezePlayer = true;

    [Header("Events")]
    [SerializeField] private UnityEvent _onOpened;
    [SerializeField] private UnityEvent _onClosed;

    public bool IsShowing => _root != null && _root.activeSelf;

    public event System.Action Closed;

    private void Awake()
    {
        if (_root == null)
        {
            _root = gameObject;
        }

        _root.SetActive(false);
    }

    public void Show()
    {
        if (_root == null)
        {
            _root = gameObject;
        }

        if (_root.activeSelf)
        {
            return;
        }

        _root.SetActive(true);

        if (_freezePlayer)
        {
            GameBroadcast.OnInputStateChange?.Invoke("UI");
            GameBroadcast.isPlayerFreezed?.Invoke(true);
        }

        _onOpened?.Invoke();
    }

    public void Close()
    {
        if (_root == null)
        {
            _root = gameObject;
        }

        if (!_root.activeSelf)
        {
            return;
        }

        _root.SetActive(false);

        if (_freezePlayer)
        {
            GameBroadcast.isPlayerFreezed?.Invoke(false);
            GameBroadcast.OnInputStateChange?.Invoke("Player");
        }

        _onClosed?.Invoke();
        Closed?.Invoke();
    }
}