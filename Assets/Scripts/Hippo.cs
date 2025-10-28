using UnityEngine;
using System.Collections;
using TMPro;

public class Hippo : MonoBehaviour
{
    public static Hippo Instance;

    [Header("Hippo Settings")]
    public float satisfactionPerWatermelon = 0.1f;
    public float detectionDistance = 5f;
    public float eatingDuration = 1.5f;
    public float sleepDuration = 5f;
    public float mouthOpenDuration = 3f;
    public float levelTime = 30f;
    public int maxWatermelonsPerSession = 5;
    public int baseWatermelonsPerLevel = 3;

    [Header("References")]
    public Transform mouthPosition;
    public Animator animator;
    public Canvas sleepCanvas;
    public TextMeshProUGUI statusText;

    public enum HippoState { Sleeping, Awake, MouthOpen, Eating, GameOver }
    private HippoState currentState = HippoState.Awake;

    private float currentSatisfaction = 0f;
    private int currentLevel = 1;
    private int currentLevelProgress = 0;
    private int watermelonsToNextLevel = 3;
    private Player player;
    private bool isSleepingCycle = false;
    private float sleepTimer = 0f;
    private float mouthOpenTimer = 0f;
    private float levelTimer = 0f;
    private int watermelonsInCurrentSession = 0;
    private int comboCount = 0;
    private Coroutine zzzCoroutine;

    // Анимационные параметры
    private readonly int animSleep = Animator.StringToHash("Sleep");
    private readonly int animMouthOpen = Animator.StringToHash("MouthOpen");
    private readonly int animEating = Animator.StringToHash("Eating");

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();

        if (sleepCanvas != null)
            sleepCanvas.gameObject.SetActive(true);

        // Инициализируем первый уровень
        watermelonsToNextLevel = GetWatermelonsForLevel(currentLevel);
        levelTimer = levelTime;

        UIManager.Instance.UpdateLevel(currentLevel, currentLevelProgress, watermelonsToNextLevel);
        UIManager.Instance.UpdateLevelTimer(levelTimer);
        UIManager.Instance.UpdateSatisfaction(currentSatisfaction);

        Debug.Log($"Старт игры: Уровень {currentLevel}, требуется арбузов: {watermelonsToNextLevel}");

        UpdateStatusText();
    }

    private void Update()
    {
        if (currentState == HippoState.GameOver) return;

        if (!isSleepingCycle)
        {
            UpdateState();
            UpdateLevelTimer();
        }
        else
        {
            UpdateSleepTimer();
        }

        UpdateMouthTimer();
        UpdateAnimations();
        CheckSleepCycle();
        UpdateStatusText();
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case HippoState.Sleeping:
                break;

            case HippoState.Awake:
                if (IsPlayerNearby() && player.HasWatermelon())
                {
                    StartMouthOpenSession();
                }
                break;

            case HippoState.MouthOpen:
                if (!IsPlayerNearby() || !player.HasWatermelon())
                {
                    currentState = HippoState.Awake;
                }
                break;

            case HippoState.Eating:
                break;
        }
    }

    private void UpdateLevelTimer()
    {
        if (currentState != HippoState.GameOver && !isSleepingCycle)
        {
            levelTimer -= Time.deltaTime;
            UIManager.Instance.UpdateLevelTimer(levelTimer);

            if (levelTimer <= 0f)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        currentState = HippoState.GameOver;
        UIManager.Instance.ShowGameOver();

        if (player != null)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = false;
        }
    }

    private void StartMouthOpenSession()
    {
        currentState = HippoState.MouthOpen;
        mouthOpenTimer = mouthOpenDuration;
        watermelonsInCurrentSession = 0;
        comboCount = 0;
    }

    private void UpdateMouthTimer()
    {
        if (currentState == HippoState.MouthOpen)
        {
            mouthOpenTimer -= Time.deltaTime;

            if (mouthOpenTimer <= 0f)
            {
                FinishMouthOpenSession();
            }
        }
    }

    private void FinishMouthOpenSession()
    {
        if (watermelonsInCurrentSession > 0)
        {
            currentState = HippoState.Eating;

            // Проверяем, не достигли ли мы уровня
            CheckLevelCompletion();

            Invoke(nameof(FinishEating), eatingDuration);
        }
        else
        {
            currentState = HippoState.Awake;
        }
    }

    private void CheckLevelCompletion()
    {
        // Если набрали достаточно арбузов для уровня
        if (currentLevelProgress >= watermelonsToNextLevel)
        {
            currentSatisfaction = 1f;
            UIManager.Instance.UpdateSatisfaction(currentSatisfaction);
            Debug.Log($"Уровень {currentLevel} завершен! Съедено {currentLevelProgress}/{watermelonsToNextLevel} арбузов");
        }
    }

    private void UpdateSleepTimer()
    {
        sleepTimer -= Time.deltaTime;

        if (sleepTimer <= 0f)
        {
            WakeUpFromSleep();
        }
    }

    private void CheckSleepCycle()
    {
        // Переходим в сон только если сытость полная И прогресс уровня достигнут
        if (currentSatisfaction >= 1f && currentLevelProgress >= watermelonsToNextLevel &&
            !isSleepingCycle && currentState != HippoState.Eating)
        {
            StartSleepCycle();
        }
    }

    private void StartSleepCycle()
    {
        isSleepingCycle = true;
        sleepTimer = sleepDuration;
        currentState = HippoState.Sleeping;

        levelTimer = levelTime;
        UIManager.Instance.UpdateLevelTimer(levelTimer);

        zzzCoroutine = StartCoroutine(AnimateZZZ());
        LevelUp();
    }

    private IEnumerator AnimateZZZ()
    {
        string[] zzzStates = { "z..", "zz..", "zzz.." };
        int currentState = 0;

        while (isSleepingCycle)
        {
            currentState = (currentState + 1) % zzzStates.Length;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void WakeUpFromSleep()
    {
        isSleepingCycle = false;
        currentSatisfaction = 0f;
        currentLevelProgress = 0; // Сбрасываем прогресс для нового уровня
        UIManager.Instance.UpdateSatisfaction(currentSatisfaction);
        currentState = HippoState.Awake;

        if (zzzCoroutine != null)
            StopCoroutine(zzzCoroutine);

        Debug.Log($"Бегемот проснулся! Уровень {currentLevel}, требуется арбузов: {watermelonsToNextLevel}");
    }

    private void UpdateAnimations()
    {
        animator.SetBool(animSleep, currentState == HippoState.Sleeping);
        animator.SetBool(animMouthOpen, currentState == HippoState.MouthOpen);
        animator.SetBool(animEating, currentState == HippoState.Eating);
    }

    private void UpdateStatusText()
    {
        if (statusText == null) return;

        string statusMessage = "";

        if (currentState == HippoState.GameOver)
        {
            statusMessage = "GAME OVER";
        }
        else if (isSleepingCycle)
        {
            int zzzState = Mathf.FloorToInt(Time.time * 2f) % 3;
            string[] zzzStates = { "z..", "zz..", "zzz.." };
            statusMessage = zzzStates[zzzState];
        }
        else if (currentState == HippoState.MouthOpen)
        {
            statusMessage = $"Throw watermelons!\nTime: {mouthOpenTimer:F1}s\nCombo: {comboCount}";
        }
        else if (currentState == HippoState.Eating)
        {
            statusMessage = "Yum-yum...";
        }
        else if (currentState == HippoState.Awake)
        {
            if (IsPlayerNearby() && player.HasWatermelon())
            {
                statusMessage = $"Come closer to feed\nLevel: {currentLevel}\nProgress: {currentLevelProgress}/{watermelonsToNextLevel}";
            }
            else
            {
                statusMessage = "";
            }
        }

        statusText.text = statusMessage;
    }

    private bool IsPlayerNearby()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance <= detectionDistance;
    }

    public void FeedWatermelon()
    {
        if (currentState == HippoState.MouthOpen && !isSleepingCycle)
        {
            watermelonsInCurrentSession++;
            comboCount++;
            currentLevelProgress++;

            // Обновляем сытость (но она теперь только для визуала)
            currentSatisfaction = Mathf.Clamp01((float)currentLevelProgress / watermelonsToNextLevel);

            UIManager.Instance.UpdateSatisfaction(currentSatisfaction);
            UIManager.Instance.UpdateLevel(currentLevel, currentLevelProgress, watermelonsToNextLevel);

            Debug.Log($"Арбуз съеден! Уровень: {currentLevel}, Прогресс: {currentLevelProgress}/{watermelonsToNextLevel}");

            // Если достигли максимума за сессию
            if (watermelonsInCurrentSession >= maxWatermelonsPerSession)
            {
                FinishMouthOpenSession();
            }
        }
        else
        {
            CameraShake.Instance.Shake();
            comboCount = 0;
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        watermelonsToNextLevel = GetWatermelonsForLevel(currentLevel);

        UIManager.Instance.UpdateLevel(currentLevel, 0, watermelonsToNextLevel);

        Debug.Log($"?? ПОВЫШЕНИЕ УРОВНЯ! Новый уровень: {currentLevel}, требуется арбузов: {watermelonsToNextLevel}");
    }

    private int GetWatermelonsForLevel(int level)
    {
        //// Явная прогрессия по уровням
        //switch (level)
        //{
        //    case 1: return 3;
        //    case 2: return 5;
        //    case 3: return 8;
        //    case 4: return 12;
        //    case 5: return 17;
        //    case 6: return 23;
        //    case 7: return 30;
        //    case 8: return 38;
        //    case 9: return 47;
        //    case 10: return 57;
        //    default: return 57 + (level - 10) * 10; // После 10 уровня +10 за уровень
        //}
        return baseWatermelonsPerLevel + (level - 1) * 2;
    }

    private void FinishEating()
    {
        if (isSleepingCycle) return;

        currentState = HippoState.Awake;
    }

    public bool CanEat()
    {
        return currentState == HippoState.MouthOpen && !isSleepingCycle;
    }

    public Vector3 GetMouthPosition()
    {
        return mouthPosition.position;
    }

    public HippoState GetCurrentState()
    {
        return currentState;
    }

    public float GetSatisfaction()
    {
        return currentSatisfaction;
    }

    public bool IsInSleepCycle()
    {
        return isSleepingCycle;
    }

    public float GetSleepTimeLeft()
    {
        return sleepTimer;
    }

    public float GetMouthTimeLeft()
    {
        return mouthOpenTimer;
    }

    public float GetLevelTimeLeft()
    {
        return levelTimer;
    }

    public int GetComboCount()
    {
        return comboCount;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetCurrentLevelProgress()
    {
        return currentLevelProgress;
    }

    public int GetWatermelonsToNextLevel()
    {
        return watermelonsToNextLevel;
    }
}