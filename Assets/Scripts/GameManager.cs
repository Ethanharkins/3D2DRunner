using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Camera camera3D;
    public Camera camera2D;
    public GameObject player;
    public GameObject movingBlock;  // Reference to the moving block
    public AudioClip jumpSound;
    public AudioClip moveSound;
    public AudioClip landSound;
    public AudioClip checkpointSound;
    public AudioClip cameraSwitchSound;
    public AudioClip tickSound;
    public GameObject pauseMenu;
    public GameObject gameOverUI;
    public TextMeshProUGUI timerText;
    public int timerDuration = 60;

    private PlayerMovement3D playerMovement3D;
    private PlayerMovement2D playerMovement2D;
    private Rigidbody playerRigidbody;
    private AudioSource audioSource;
    private float timer;
    private float tickSoundCooldown = 1f;
    private float tickSoundTimer = 0f;
    private bool isGameOver = false;
    private MovingBlock movingBlockScript;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            InitializeLevel();
        }
    }

    private void Start()
    {
        playerMovement3D = player.GetComponent<PlayerMovement3D>();
        playerMovement2D = player.GetComponent<PlayerMovement2D>();
        playerRigidbody = player.GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        movingBlockScript = movingBlock.GetComponent<MovingBlock>();
        timer = timerDuration;
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        SwitchTo3D();
        UpdateTimerText();
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

        timer -= Time.deltaTime;
        tickSoundTimer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            GameOver();
        }
        UpdateTimerText();
    }

    private void SwitchTo3D()
    {
        camera3D.gameObject.SetActive(true);
        camera2D.gameObject.SetActive(false);

        playerMovement3D.enabled = true;
        playerMovement2D.enabled = false;

        playerRigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;

        SetGhostBlocksActive(false);

        // Notify the moving block to use the first set of movement points
        movingBlockScript.SetMovementPoints(movingBlockScript.pointA1, movingBlockScript.pointB1);
    }

    private void SwitchTo2D()
    {
        camera3D.gameObject.SetActive(false);
        camera2D.gameObject.SetActive(true);

        playerMovement3D.enabled = false;
        playerMovement2D.enabled = true;

        playerRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        SetGhostBlocksActive(true);

        // Notify the moving block to use the second set of movement points
        movingBlockScript.SetMovementPoints(movingBlockScript.pointA2, movingBlockScript.pointB2, true);
    }

    private void SetGhostBlocksActive(bool isActive)
    {
        GameObject[] jumpableBlocks = GameObject.FindGameObjectsWithTag("Jumpable");
        foreach (GameObject block in jumpableBlocks)
        {
            Transform ghostBlock = block.transform.Find("GhostBlock");
            if (ghostBlock != null)
            {
                ghostBlock.gameObject.SetActive(isActive);
            }
        }
    }

    private void TogglePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0;
        gameOverUI.SetActive(true);
        playerMovement3D.enabled = false;
        playerMovement2D.enabled = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateTimerText()
    {
        timerText.text = Mathf.Ceil(timer).ToString();
        if (tickSoundTimer <= 0 && timer > 0)
        {
            audioSource.PlayOneShot(tickSound);
            tickSoundTimer = tickSoundCooldown;
        }
    }
}
