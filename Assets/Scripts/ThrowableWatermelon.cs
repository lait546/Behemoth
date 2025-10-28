using UnityEngine;

public class ThrowableWatermelon : MonoBehaviour
{
    [Header("Throw Settings")]
    public float destroyDelay = 5f;
    public float maxLifetime = 10f;

    private Rigidbody rb;
    private bool isThrown = false;
    private float lifetime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isThrown)
        {
            lifetime += Time.deltaTime;
            if (lifetime >= maxLifetime)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Throw(Vector3 velocity)
    {
        isThrown = true;
        rb.isKinematic = false;
        rb.velocity = velocity;

        Invoke(nameof(DestroyIfMissed), destroyDelay);
    }

    private void DestroyIfMissed()
    {
        if (isThrown)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown)
        {
            if (collision.collider.CompareTag("HippoMouth"))
            {
                Hippo.Instance.FeedWatermelon();
                Destroy(gameObject);
            }
            else
            {
                isThrown = false;
            }
        }
    }

    public void MarkAsThrown()
    {
        isThrown = true;
        Invoke(nameof(DestroyIfMissed), destroyDelay);
    }
}