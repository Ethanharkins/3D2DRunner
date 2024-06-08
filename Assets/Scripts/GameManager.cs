using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Camera camera3D;
    public Camera camera2D;
    public GameObject player;
    public Transform[] checkpoints;
    public AudioClip jumpSound;
    public AudioClip moveSound;
    public AudioClip checkpointSound;
    public ParticleSystem respawnEffect;
    public GameObject pauseMenu;

    private PlayerMovement3D playerMovement3D;
    private PlayerMovement2D playerMovement2D;
    private Rigidbody playerRigidbody;
    private int currentCheckpointIndex = -1; // Initialize with -1 to indicate no checkpoint reached yet

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
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        SwitchTo3D();
    }

    private void Update()
    {
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
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
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
        if (currentCheckpointIndex >= 0)
        {
            player.transform.position = checkpoints[currentCheckpointIndex].position;
        }
        else
        {
            // If no checkpoint reached, respawn at the initial position
            player.transform.position = player.transform.position;
        }
        Instantiate(respawnEffect, player.transform.position, Quaternion.identity);
        playerRigidbody.velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Respawn();
        }

        if (other.CompareTag("Checkpoint"))
        {
            // Find the index of the checkpoint that was triggered
            for (int i = 0; i < checkpoints.Length; i++)
            {
                if (checkpoints[i] == other.transform)
                {
                    currentCheckpointIndex = i;
                    break;
                }
            }
            AudioSource.PlayClipAtPoint(checkpointSound, other.transform.position);
        }
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
}
