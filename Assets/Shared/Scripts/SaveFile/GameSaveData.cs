[System.Serializable]
public class GameSaveData
{
    public string lastLevelName = "L1_Home";

    // Level Select: Stores which levels are unlocked
    public System.Collections.Generic.List<string> unlockedLevels = new System.Collections.Generic.List<string> { "L1_Home" };
}