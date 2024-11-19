using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Camera camera3D;
    public Camera camera2D;
    public GameObject player;
    public AudioClip jumpSound;
    public AudioClip moveSound;
    public AudioClip landSound;
    public AudioClip checkpointSound;
    public AudioClip cameraSwitchSound;
    public AudioClip tickSound;
    public GameObject gameOverUI;
    public TextMeshProUGUI timerText;
    public int timerDuration = 60;
    public GameObject pauseMenu;
    public GameObject winCanvas;

    private PlayerMovement3D playerMovement3D;
    private PlayerMovement2D playerMovement2D;
    private Rigidbody playerRigidbody;
    private AudioSource audioSource;
    private float timer;
    private float tickSoundCooldown = 1f;
    private float tickSoundTimer = 0f;
    private bool isGameOver = false;
    private bool isPaused = false;

    // List to cache all objects tagged "Jumpable"
    private List<GameObject> jumpableObjects = new List<GameObject>();

    // Singleton instance
    private static GameManager instance;

    private void Awake()
    {
        // Implement Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // Persist GameManager across scenes
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeLevel(); // Reinitialize references on scene load
    }

    private void Start()
    {
        playerMovement3D = player.GetComponent<PlayerMovement3D>();
        playerMovement2D = player.GetComponent<PlayerMovement2D>();
        playerRigidbody = player.GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        timer = timerDuration;

        pauseMenu.SetActive(false);
        winCanvas.SetActive(false);
        CacheJumpableObjects();
        InitializeLevel();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void CacheJumpableObjects()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Jumpable") && obj.scene.IsValid())
            {
                jumpableObjects.Add(obj);
            }
        }

        Debug.Log($"Cached {jumpableObjects.Count} objects tagged as 'Jumpable'.");
    }

    private void InitializeLevel()
    {
        // Reset game state
        isGameOver = false;
        isPaused = false;

        // Reset UI
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (winCanvas != null) winCanvas.SetActive(false);

        // Reset timer
        timer = timerDuration;
        UpdateTimerText();

        // Reset player movement and camera
        if (playerMovement3D != null) playerMovement3D.enabled = true;
        if (playerMovement2D != null) playerMovement2D.enabled = false;

        SwitchTo3D();

        Time.timeScale = 1;
    }

    private void Update()
    {
        if (isGameOver) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (camera3D.gameObject.activeInHierarchy)
            {
                SwitchTo2D();
            }
            else
            {
                SwitchTo3D();
            }

            audioSource.PlayOneShot(cameraSwitchSound);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        if (!isPaused)
        {
            timer -= Time.deltaTime;
            tickSoundTimer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                GameOver();
            }
            UpdateTimerText();
        }

        if ((pauseMenu != null && pauseMenu.activeSelf) || (winCanvas != null && winCanvas.activeSelf))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void SwitchTo3D()
    {
        camera3D.gameObject.SetActive(true);
        camera2D.gameObject.SetActive(false);

        if (playerMovement3D != null) playerMovement3D.enabled = true;
        if (playerMovement2D != null) playerMovement2D.enabled = false;

        playerRigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;

        ToggleJumpableObjects(false);
    }

    private void SwitchTo2D()
    {
        camera3D.gameObject.SetActive(false);
        camera2D.gameObject.SetActive(true);

        if (playerMovement3D != null) playerMovement3D.enabled = false;
        if (playerMovement2D != null) playerMovement2D.enabled = true;

        playerRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        ToggleJumpableObjects(true);
    }

    private void ToggleJumpableObjects(bool isActive)
    {
        foreach (GameObject jumpable in jumpableObjects)
        {
            if (jumpable != null)
            {
                jumpable.SetActive(isActive);
            }
        }
    }

    private void TogglePauseMenu()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            isPaused = false;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            isPaused = true;
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0;
        if (gameOverUI != null) gameOverUI.SetActive(true);
        if (playerMovement3D != null) playerMovement3D.enabled = false;
        if (playerMovement2D != null) playerMovement2D.enabled = false;
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(timer).ToString();
        }

        if (tickSoundTimer <= 0 && timer > 0)
        {
            audioSource.PlayOneShot(tickSound);
            tickSoundTimer = tickSoundCooldown;
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WinZone"))
        {
            if (winCanvas != null)
            {
                winCanvas.SetActive(true);
            }

            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("Player reached the Win Zone!");
        }
    }
}
