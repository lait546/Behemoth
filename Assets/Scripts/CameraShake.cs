using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Shake Settings")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;

    private Vector3 originalPosition;
    private float shakeTimer = 0f;

    private void Awake()
    {
        Instance = this;
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            transform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeTimer = 0f;
            transform.localPosition = originalPosition;
        }
    }

    public void Shake()
    {
        shakeTimer = shakeDuration;
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTimer = duration;
    }
}