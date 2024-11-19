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
        InitializeReferences(); // Reinitialize scene-specific references
    }

    private void InitializeReferences()
    {
        // Ensure all UI is hidden at the start
        if (winCanvas != null) winCanvas.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);

        // Hide the cursor during gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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

    private void Update()
    {
        // Show cursor when PauseMenu or WinCanvas is active
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

            // Unlock and show the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("Player reached the Win Zone!");
        }
    }
}
