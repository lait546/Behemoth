using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Throwing")]
    public Transform throwPoint;
    public GameObject throwableWatermelonPrefab;
    public float maxThrowDistance = 15f;
    public int maxWatermelons = 999;

    private PlayerMovement playerMovement;
    private int currentWatermelonCount = 0;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        UIManager.Instance.UpdateWatermelonCount(currentWatermelonCount);
        CheckAndSetEnableThrow();
    }

    private void Update()
    {
        HandleThrowing();
    }

    private void HandleThrowing()
    {
        if (Input.GetMouseButtonDown(0) && currentWatermelonCount > 0)
        {
            ThrowWatermelon();
        }
    }

    private void ThrowWatermelon()
    {
        if (currentWatermelonCount > 0)
        {
            GameObject watermelonObj = Instantiate(throwableWatermelonPrefab, throwPoint.position, Quaternion.identity);
            Rigidbody rb = watermelonObj.GetComponent<Rigidbody>();
            ThrowableWatermelon watermelon = watermelonObj.GetComponent<ThrowableWatermelon>();

            if (rb != null)
            {
                Vector3 throwDirection = GetSmartThrowDirection();
                float throwPower = GetAdaptiveThrowPower();

                rb.velocity = throwDirection * throwPower;

                watermelon.MarkAsThrown();

                currentWatermelonCount--;
                CheckAndSetEnableThrow();
                UIManager.Instance.UpdateWatermelonCount(currentWatermelonCount);
            }
        }
    }

    private Vector3 GetSmartThrowDirection()
    {
        Camera playerCamera = playerMovement.GetPlayerCamera();
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Vector3 cameraDirection = playerCamera.transform.forward;

        if (Physics.Raycast(ray, out hit, maxThrowDistance))
        {
            float hitDistance = Vector3.Distance(throwPoint.position, hit.point);

            if (hitDistance < 3f) 
            {
                Vector3 horizontalDirection = new Vector3(cameraDirection.x, 0, cameraDirection.z).normalized;
                return (horizontalDirection + Vector3.up * 0.3f).normalized;
            }
            else
            {
                Vector3 directionToHit = (hit.point - throwPoint.position).normalized;
                return directionToHit;
            }
        }
        else
        {
            return cameraDirection;
        }
    }

    private float GetAdaptiveThrowPower()
    {
        Camera playerCamera = playerMovement.GetPlayerCamera();
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        float minPower = 10f;
        float maxPower = 18f;

        if (Physics.Raycast(ray, out hit, maxThrowDistance))
        {
            float hitDistance = Vector3.Distance(throwPoint.position, hit.point);

            float normalizedDistance = Mathf.Clamp01(hitDistance / maxThrowDistance);

            float powerCurve = Mathf.Pow(normalizedDistance, 0.7f);
            float power = Mathf.Lerp(minPower, maxPower, powerCurve);

            return Mathf.Max(power, minPower); 
        }
        else
        {
            return maxPower;
        }
    }

    public void PickUpWatermelon()
    {
        if (currentWatermelonCount < maxWatermelons)
        {
            currentWatermelonCount++;
            CheckAndSetEnableThrow();
            UIManager.Instance.UpdateWatermelonCount(currentWatermelonCount);
        }
    }

    private void CheckAndSetEnableThrow()
    {
        if (throwPoint != null)
        {
            if (currentWatermelonCount > 0)
                throwPoint.gameObject.SetActive(true);
            else
                throwPoint.gameObject.SetActive(false);
        }
    }

    public bool HasWatermelon()
    {
        return currentWatermelonCount > 0;
    }

    public bool CanCarryMore()
    {
        return currentWatermelonCount < maxWatermelons;
    }

    public int GetWatermelonCount()
    {
        return currentWatermelonCount;
    }
}