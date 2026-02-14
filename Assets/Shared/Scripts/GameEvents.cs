using System;
using UnityEngine;

public static class GameEvents
{
    // Progression
    public static Action<int, int> OnFragmentUpdate;
    public static Action<string> OnStageNameUpdate;

    // Camera & State
    public static Action<bool> OnCameraToggle; // true = Camera State, false = Normal State

    // Lighter
    public static Action<bool> OnLighterToggle; //  true = Lighter is ON, false = Lighter is OFF

    // UI & Inventory
    public static Action<float, float> OnStaminaUpdate;
    public static Action<int, Sprite> OnInventorySlotUpdate;
    public static Action<int> OnSlotSelected;
    public static Action<string, bool> OnInteractionPromptReq;

    // Level Transition
    public static Action<string> OnLevelTransitionStarted;
}