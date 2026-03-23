using UnityEngine;

public class HospitalMonsterTutorialManager : MonoBehaviour
{
    public static HospitalMonsterTutorialManager Instance { get; private set; }

    private bool _hasShownFirstMonsterTutorial;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _hasShownFirstMonsterTutorial = false;
    }

    public bool HasShownTutorial()
    {
        return _hasShownFirstMonsterTutorial;
    }

    public void MarkTutorialShown()
    {
        _hasShownFirstMonsterTutorial = true;
    }

    public void ResetTutorialState()
    {
        _hasShownFirstMonsterTutorial = false;
    }
}