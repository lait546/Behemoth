using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    public Slider satisfactionSlider;
    public TextMeshProUGUI watermelonCountText;

    [Header("Level UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelProgressText;
    public Slider levelProgressSlider;
    public TextMeshProUGUI levelTimerText;

    [Header("Combo UI")]
    public TextMeshProUGUI comboText;

    [Header("Game Over UI")]
    public GameObject gameOverWindow;
    public Button restartButton;

    [Header("Start Game UI")]
    public GameObject startGameWindow;

    private void Awake()
    {
        Instance = this;

        Time.timeScale = 0f;
        startGameWindow.SetActive(true);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (gameOverWindow != null)
        {
            gameOverWindow.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateComboUI();
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }

    public void UpdateSatisfaction(float satisfaction)
    {
        if (satisfactionSlider != null)
            satisfactionSlider.value = satisfaction;
    }

    public void UpdateWatermelonCount(int count)
    {
        if (watermelonCountText != null)
            watermelonCountText.text = $"Watermelons: {count}";
    }

    public void UpdateLevel(int level, int progress, int required)
    {
        if (levelText != null)
            levelText.text = $"Level: {level}";

        if (levelProgressText != null)
            levelProgressText.text = $"{progress}/{required}";

        if (levelProgressSlider != null)
        {
            levelProgressSlider.maxValue = required;
            levelProgressSlider.value = progress;
        }
    }

    public void UpdateLevelTimer(float timeLeft)
    {
        if (levelTimerText != null)
        {
            levelTimerText.text = $"Time: {Mathf.CeilToInt(timeLeft)}s";

            if (timeLeft < 10f)
                levelTimerText.color = Color.red;
            else
                levelTimerText.color = Color.white;
        }
    }

    public void ShowGameOver()
    {
        if (gameOverWindow != null)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            Time.timeScale = 0f;
            gameOverWindow.gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void UpdateComboUI()
    {
        if (comboText != null)
        {
            int combo = Hippo.Instance.GetComboCount();
            if (combo > 0)
            {
                comboText.text = $"COMBO: x{combo}";
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}