using System;
using UnityEngine;

public static class GameBroadcast
{
    // Progression
    public static Action<bool> OnAllFragmentsCollected; // true = all collected, false = reset

    // HUD Updates
    public static Action<int, int> OnProgressionCountUpdateHUD; // current count, total required
    public static Action<string> OnStageNameUpdateHUD; // stage name
    public static Action<string> OnObjectiveUpdateHUD; // objective text

    // Readable Item
    public static Action<string, bool> OnShowReadableContent;

    // Item Status
    public static Action<string, float> OnItemStatusUpdate; // 0 to 1 representing cooldown progress
    public static Action<string, Sprite> OnItemEquipped;

    // Camera & State
    public static Action<bool> OnCameraToggle; // true = Camera State, false = Normal State

    // Lighter
    public static Action<bool> OnLighterToggle; //  true = Lighter is ON, false = Lighter is OFF

    // Flashlight
    public static Action<bool> OnFlashlightToggle; //  true = Flashlight is ON, false = Flashlight is OFF

    // UI & Inventory
    public static Action<float, float> OnStaminaUpdate;
    public static Action<int, Sprite> OnInventorySlotUpdate;
    public static Action<int> OnSlotSelected;
    public static Action<string, bool> OnInteractionPromptReq;

    // Level Transition
    public static Action<string> OnLevelTransitionStarted;

    // Monster interactions
    public static Action<bool> OnMonsterIsHit;
    public static Action<string> OnPlayerMovementState;
    // Player State
    public static Action<bool> isPlayerFreezed;
}