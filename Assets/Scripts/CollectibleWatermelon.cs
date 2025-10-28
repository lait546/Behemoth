using UnityEngine;

public class CollectibleWatermelon : MonoBehaviour
{
    [Header("Collection Settings")]
    public float lifetime = 15f;
    public float fadeDuration = 2f;

    private float currentLifetime;
    private bool isFading = false;
    private Renderer watermelonRenderer;
    private Material originalMaterial;
    private Color originalColor;

    private void Start()
    {
        currentLifetime = lifetime;
        watermelonRenderer = GetComponent<Renderer>();
        originalMaterial = watermelonRenderer.material;
        originalColor = originalMaterial.color;

        Invoke(nameof(StartFade), lifetime - fadeDuration);
        Invoke(nameof(DestroyWatermelon), lifetime);
    }

    private void Update()
    {
        if (isFading)
        {
            float fadeProgress = (Time.time - (currentLifetime - fadeDuration)) / fadeDuration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, fadeProgress);
            watermelonRenderer.material.color = newColor;
        }
    }

    private void StartFade()
    {
        isFading = true;
    }

    private void DestroyWatermelon()
    {
        WatermelonSpawner.Instance.OnWatermelonPickedUp();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null && player.CanCarryMore())
            {
                player.PickUpWatermelon();
                WatermelonSpawner.Instance.OnWatermelonPickedUp();
                Destroy(gameObject);
            }
        }
    }
}