using UnityEngine;

[RequireComponent(typeof(Collider))]
public class IntroRequirementBlocker : MonoBehaviour
{
    [Header("Required State")]
    [SerializeField] private bool _disableAfterRequirementCompleted = true;

    [Header("Hint Message")]
    [TextArea(2, 4)]
    [SerializeField] private string _blockedMessage = "Talk to Shadow first.";

    [SerializeField] private float _messageCooldown = 1f;

    private float _nextAllowedTime;
    private bool _requirementCompleted;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            // Giữ collider thường để nó chặn thật
            col.isTrigger = false;
        }
    }

    public void MarkRequirementCompleted()
    {
        _requirementCompleted = true;

        if (_disableAfterRequirementCompleted)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_requirementCompleted) return;
        if (Time.time < _nextAllowedTime) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        _nextAllowedTime = Time.time + _messageCooldown;

        GameBroadcast.OnObjectiveUpdateHUD?.Invoke(_blockedMessage);
    }
}