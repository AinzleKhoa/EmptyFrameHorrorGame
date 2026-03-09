using UnityEngine;
using System.Collections.Generic;

public class ItemData : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string _itemNameDisplay = "New Item";
    public string ItemName = "NewItem";
    public Sprite ItemIcon; // Moved here to keep data together

    [Header("Description Settings")]
    [TextArea(3, 10)]
    [SerializeField] private string _baseDescription;

    private List<string> _activeUpgrades = new List<string>();

    public string FullDescription
    {
        get
        {
            // Combines the Name, Base Description, and all Upgrades
            string text = $"<b>{_itemNameDisplay}</b>\n{_baseDescription}";

            if (_activeUpgrades.Count > 0)
            {
                text += "\n<color=#FFD700><b>INSTALLED UPGRADES:</b></color>";
                foreach (var upgrade in _activeUpgrades)
                {
                    text += $"\n• {upgrade}";
                }
            }
            return text;
        }
    }

    public void AddUpgrade(string upgradeName)
    {
        if (!_activeUpgrades.Contains(upgradeName))
            _activeUpgrades.Add(upgradeName);
    }
}