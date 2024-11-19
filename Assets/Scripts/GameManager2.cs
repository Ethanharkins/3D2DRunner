using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager2 : MonoBehaviour
{
    public GameObject winCanvas; // Assign WinCanvas here
    public GameObject pauseMenu; // Assign PauseMenu here
    public GameManager gameManager; // Reference the original GameManager

    private static GameManager2 instance; // Singleton instance

    private void Awake()
    {
        // Implement Singleton pattern to ensure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // Persist this object across scenes
    }

    private void Start()
    {
        InitializeReferences();
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
        // Ensure references are re-established on scene load
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        // Hide UI at the start
        if (winCanvas != null) winCanvas.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);

        // Dynamically reassign GameManager if missing
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager is missing. Please assign it in the Inspector!");
            }
        }
    }

    public void RestartScene()
    {
        // Reload the current scene while preserving GameManager references
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadNextLevel()
    {
        // Load the next scene by incrementing the build index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadMainMenu()
    {
        // Load the Main Menu scene
        SceneManager.LoadScene("MainMenu");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the WinZone
        if (other.CompareTag("WinZone"))
        {
            if (winCanvas != null)
            {
                winCanvas.SetActive(true); // Show WinCanvas
            }

            // Stop the game timer by pausing the game
            Time.timeScale = 0;

            Debug.Log("Player reached the Win Zone!");
        }
    }
}
