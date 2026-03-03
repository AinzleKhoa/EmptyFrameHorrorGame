using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    Light l;
    float baseInt;

    public float flickerSpeed = 0.1f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    void Start()
    {
        l = GetComponent<Light>();
        baseInt = l.intensity;
    }

    void Update()
    {
        l.intensity = Random.Range(minIntensity, maxIntensity);
    }
}
