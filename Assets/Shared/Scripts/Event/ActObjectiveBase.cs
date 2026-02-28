using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class ActObjectiveBase
{
    [Header("Objective Settings")]
    [SerializeField] protected string _objectiveName; // internal name
    [SerializeField, TextArea] protected string _objectiveDescription; // What the player sees

    // --- State Properties ---
    public abstract bool IsReached { get; }
    public abstract int CurrentProgress { get; }
    public abstract int GoalAmount { get; }
    public string Description => _objectiveDescription;

    // --- Lifecycle Methods ---

    // Called when the Act begins
    public virtual void OnObjectiveStarted() { Reset(); }

    // Logic for frame-based checks (Distance, Timers, HP)
    public virtual void OnUpdate(float deltaTime) { }

    // Logic for event-based triggers (Signals, Item Pickups)
    public virtual void OnSignalRaised(GameSignal signal) { }

    // Called if the player fails or the act restarts
    public abstract void Reset();

    // Helper for the Director's listener setup
    public abstract List<GameSignal> GetAllRequiredSignals();
}