using UnityEngine;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour
{
    private string _playerName = "Player";
    private string _statDescription = "";


    private List<string> _permanentUpgrades = new List<string>();

    public string FullStatusDescription
    {
        get
        {
            string text = $"<b>{_playerName}</b>{_statDescription}";

            if (_permanentUpgrades.Count > 0)
            {
                text += "\n<color=#FFD700><b>PERMANENT UPGRADES:</b></color>";
                foreach (var upgrade in _permanentUpgrades)
                {
                    text += $"\n• {upgrade}";
                }
            }
            return text;
        }
    }

    public void AddPermanentUpgrade(string info)
    {
        if (!_permanentUpgrades.Contains(info))
            _permanentUpgrades.Add(info);
    }
}