using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "player_progress.json");

    public static void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // 'true' makes it readable for debugging
        File.WriteAllText(SavePath, json);
        Debug.Log($"Game Saved to: {SavePath}");
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            return new GameSaveData(); // Return default data if no save exists
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }
}