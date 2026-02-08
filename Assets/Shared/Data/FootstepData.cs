using UnityEngine;

[CreateAssetMenu(fileName = "NewFootstepData", menuName = "EmptyFrame/Footstep Data")]
public class FootstepData : ScriptableObject
{
    public string materialTag; // e.g., "Wood", "Metal", "Stone"
    public AudioClip[] clips;
}