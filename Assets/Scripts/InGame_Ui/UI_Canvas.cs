using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Canvas : MonoBehaviour
{
    public static UI_Canvas instance;

    [Header("Stamina UI")]
    [SerializeField] Image fillImage;

    [Header("Colors")]
    [SerializeField] Color fullColor = Color.green;
    [SerializeField] Color midColor = new Color(1f, 0.5f, 0f);
    [SerializeField] Color lowColor = Color.red;

    [Header("Level Complete")]
    [SerializeField] GameObject levelPass;
    [SerializeField] GameObject celebrationEffect;

    [Header("Level Fail")]
    [SerializeField] GameObject levelFail;
    [SerializeField] GameObject pausePanel;

    PlayerController player;

    bool isGameOver;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();

        if (fillImage == null)
            Debug.LogError("Fill Image not assigned!");
    }

    private void Update()
    {
        PauseButtonHandler();

        if (!isGameOver)
        {
            UpdateStaminaUI();
        }
    }

    void UpdateStaminaUI()
    {
        if (player == null || fillImage == null) return;

        float staminaNormalized = player.StaminaNormalized;

        // Fill amount
        fillImage.fillAmount = staminaNormalized;

        // Color change
        if (staminaNormalized > 0.5f)
            fillImage.color = fullColor;
        else if (staminaNormalized > 0.2f)
            fillImage.color = midColor;
        else
            fillImage.color = lowColor;
    }

    // ================= LEVEL PASS =================
    public void ShowLevelComplete()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (levelPass != null)
            levelPass.SetActive(true);

        if (celebrationEffect != null)
            celebrationEffect.SetActive(true);

        if (player != null)
            player.SetControl(false);

        Debug.Log("LEVEL PASSED 🎉");
    }

    // ================= LEVEL FAIL =================
    public void ShowLevelFail()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (levelFail != null)
            levelFail.SetActive(true);

        if (player != null)
            player.SetControl(false);

        Debug.Log("LEVEL FAILED 💀");
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    //======================================== UI Buttons ======================================
   
    public void LevelPassHomeButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void PauseButtonHandler()
    {
        if (isGameOver) return;
        if (levelPass.activeInHierarchy) return;
        if (levelFail.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
}