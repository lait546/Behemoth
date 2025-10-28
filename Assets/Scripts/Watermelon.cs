using UnityEngine;

public class Watermelon : MonoBehaviour
{
    [Header("Watermelon Settings")]
    public float destroyDelay = 2f;

    private Rigidbody rb;
    private bool isThrown = false;
    private bool canBePickedUp = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Throw(Vector3 velocity)
    {
        isThrown = true;
        canBePickedUp = false;
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

        if (collision.gameObject.CompareTag("Player") && canBePickedUp && !isThrown)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null && player.CanCarryMore())
            {
                player.PickUpWatermelon();
                canBePickedUp = false;
                WatermelonSpawner.Instance.OnWatermelonPickedUp();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canBePickedUp && !isThrown)
        {
            Player player = other.GetComponent<Player>();
            if (player != null && player.CanCarryMore())
            {
                player.PickUpWatermelon();
                canBePickedUp = false;
                WatermelonSpawner.Instance.OnWatermelonPickedUp();
                Destroy(gameObject);
            }
        }
    }
}