using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(OutlineManager))]
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    private Color _defaultOutlineColor = Color.white;
    private float _defaultOutlineWidth = 5f;

    protected Outline _outline;
    protected OutlineManager _outlineManager;
    protected SignalTrigger _signal;

    public abstract string PromptMessage { get; }

    protected virtual void Awake()
    {
        _outline = GetComponent<Outline>();
        _outlineManager = GetComponent<OutlineManager>();
        _signal = GetComponent<SignalTrigger>();

        // We DO NOT force the color here anymore. 
        // We only ensure it's hidden so the Manager can take over.
        if (_outline != null)
        {
            _outline.enabled = false;
        }
    }

    // This ONLY runs when you first add the script or click "Reset"
    protected virtual void Reset()
    {
        _outline = GetComponent<Outline>();

        if (_outline != null)
        {
            _outline.OutlineColor = _defaultOutlineColor;
            _outline.OutlineWidth = _defaultOutlineWidth;
            _outline.OutlineMode = Outline.Mode.OutlineAll;
            _outline.enabled = false;
        }
    }

    public virtual void Interact(PlayerInteractor interactor)
    {
        if (_signal != null) _signal.RaiseSignal();
    }

    public void HideOutline()
    {
        if (_outlineManager != null) _outlineManager.enabled = false;
        if (_outline != null) _outline.enabled = false;
    }

    public void ShowOutline()
    {
        if (_outlineManager != null) _outlineManager.enabled = true;
        if (_outline != null) _outline.enabled = true;
    }
}