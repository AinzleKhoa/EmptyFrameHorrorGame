using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sig#_L#_NewEvent", menuName = "EmptyFrame/Game Signal Data")]
public class GameSignal : ScriptableObject
{
    private readonly List<StoryDirector> _listeners = new List<StoryDirector>();

    public void Raise()
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
            _listeners[i].OnEventRaised(this);
    }

    public void RegisterListener(StoryDirector listener) => _listeners.Add(listener);
    public void UnregisterListener(StoryDirector listener) => _listeners.Remove(listener);
}