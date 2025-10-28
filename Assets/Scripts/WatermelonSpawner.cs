using UnityEngine;

public class WatermelonSpawner : MonoBehaviour
{
    public static WatermelonSpawner Instance;

    [Header("Spawn Area")]
    public Collider spawnArea;
    public Vector3 customSpawnSize = new Vector3(10f, 0f, 10f);

    [Header("Spawn Settings")]
    public GameObject collectibleWatermelonPrefab;
    public float respawnDelay = 2f;
    public int maxWatermelonsOnMap = 5;
    public LayerMask spawnCheckMask = 1;

    private int currentWatermelonsOnMap = 0;
    private Bounds spawnBounds;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CalculateSpawnBounds();

        for (int i = 0; i < maxWatermelonsOnMap; i++)
        {
            SpawnWatermelon();
        }
    }

    private void CalculateSpawnBounds()
    {
        if (spawnArea != null)
        {
            spawnBounds = spawnArea.bounds;
        }
        else
        {
            Vector3 center = transform.position;
            Vector3 size = customSpawnSize;
            spawnBounds = new Bounds(center, size);
        }
    }

    public void SpawnWatermelon()
    {
        if (currentWatermelonsOnMap >= maxWatermelonsOnMap) return;

        Vector3 spawnPosition = FindSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            Instantiate(collectibleWatermelonPrefab, spawnPosition, Quaternion.identity);
            currentWatermelonsOnMap++;
        }
    }

    private Vector3 FindSpawnPosition()
    {
        int attempts = 0;
        int maxAttempts = 20;

        while (attempts < maxAttempts)
        {
            Vector3 spawnPos = GetRandomPointInBounds();

            if (!Physics.CheckSphere(spawnPos, 0.5f, spawnCheckMask))
            {
                return spawnPos;
            }

            attempts++;
        }

        return GetRandomPointInBounds();
    }

    private Vector3 GetRandomPointInBounds()
    {
        float x = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
        float y = spawnBounds.center.y;
        float z = Random.Range(spawnBounds.min.z, spawnBounds.max.z);

        return new Vector3(x, y, z);
    }

    public void OnWatermelonPickedUp()
    {
        currentWatermelonsOnMap--;
        Invoke(nameof(SpawnWatermelon), respawnDelay);
    }

    public Bounds GetSpawnBounds()
    {
        return spawnBounds;
    }
}