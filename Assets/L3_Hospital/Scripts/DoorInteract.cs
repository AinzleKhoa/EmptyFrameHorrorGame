using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 3f;
    bool open;
    Quaternion closedRot;
    Quaternion openRot;

    void Start()
    {
        closedRot = transform.rotation;
        openRot = transform.rotation * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        if (Vector3.Distance(Camera.main.transform.position, transform.position) < 2f)
        {
            if (Input.GetKeyDown(KeyCode.E))
                open = !open;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            open ? openRot : closedRot,
            Time.deltaTime * speed);
    }
}
