using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Camera camera3D;
    public Camera camera2D;
    public GameObject player;
    public Transform[] checkpoints;
    public Transform[] respawnPoints;
    public AudioClip jumpSound;
    public AudioClip moveSound;
    public AudioClip landSound;
    public AudioClip checkpointSound;
    public AudioClip cameraSwitchSound;
    public AudioClip tickSound;
    public ParticleSystem respawnEffect;
    public ParticleSystem deathEffect;
    public GameObject pauseMenu;
    public TextMeshProUGUI timerText;
    public int timerDuration = 60;
    public GameObject gameOverUI;  // Add reference to the Game Over UI

    private PlayerMovement3D playerMovement3D;
    private PlayerMovement2D playerMovement2D;
    private Rigidbody playerRigidbody;
    private int currentCheckpointIndex = -1;
    private AudioSource audioSource;
    private float timer;
    private float tickSoundCooldown = 1f;
    private float tickSoundTimer = 0f;
    private bool isGameOver = false;  // Add flag for game over state

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
        if (isGameOver) return;  // Stop updating if the game is over

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

            // Play the camera switch sound
            audioSource.PlayOneShot(cameraSwitchSound);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        // Update the countdown timer
        timer -= Time.deltaTime;
        tickSoundTimer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            GameOver();  // Trigger game over when the timer reaches 0
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
    }

    private void SwitchTo2D()
    {
        camera3D.gameObject.SetActive(false);
        camera2D.gameObject.SetActive(true);

        playerMovement3D.enabled = false;
        playerMovement2D.enabled = true;

        playerRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        SetGhostBlocksActive(true);
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

    public void Respawn()
    {
        if (currentCheckpointIndex >= 0 && currentCheckpointIndex < respawnPoints.Length)
        {
            player.transform.position = respawnPoints[currentCheckpointIndex].position;
        }
        else
        {
            // Default respawn position (could be the initial player position)
            player.transform.position = checkpoints[0].position;
        }
        Instantiate(respawnEffect, player.transform.position, Quaternion.identity);
        playerRigidbody.velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            // Play death particle effect
            Instantiate(deathEffect, player.transform.position, Quaternion.identity);
            Respawn();
        }

        if (other.CompareTag("Checkpoint"))
        {
            for (int i = 0; i < checkpoints.Length; i++)
            {
                if (checkpoints[i] == other.transform)
                {
                    currentCheckpointIndex = i;
                    break;
                }
            }
            audioSource.PlayOneShot(checkpointSound);
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0;  // Stop game time
        gameOverUI.SetActive(true);  // Show Game Over UI
        playerMovement3D.enabled = false;  // Lock player movement
        playerMovement2D.enabled = false;  // Lock player movement
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
