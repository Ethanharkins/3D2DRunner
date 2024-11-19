using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public AudioClip jumpSound;
    public AudioClip moveSound;
    public AudioClip landSound;

    private Rigidbody rb;
    private AudioSource audioSource;
    private bool isGrounded;
    private float moveSoundCooldown = 0.2f; // Time between movement sounds
    private float moveSoundTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        float moveHorizontal = 0;

        // Allow movement in both directions simultaneously
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1;
        }

        // Apply horizontal movement without overriding the vertical velocity
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, 0.0f) * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, rb.velocity.z);

        // Play movement sound at intervals while moving on the ground
        if (moveHorizontal != 0 && isGrounded)
        {
            moveSoundTimer -= Time.deltaTime;
            if (moveSoundTimer <= 0)
            {
                audioSource.PlayOneShot(moveSound);
                moveSoundTimer = moveSoundCooldown;
            }
        }

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            audioSource.PlayOneShot(jumpSound);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if colliding with "Ground" or "Jumpable" objects
        if ((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Jumpable")) && !isGrounded)
        {
            audioSource.PlayOneShot(landSound);
            isGrounded = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Stay grounded if still in contact with "Ground" or "Jumpable" objects
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Jumpable"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Set as not grounded if leaving "Ground" or "Jumpable" objects
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Jumpable"))
        {
            isGrounded = false;
        }
    }
}
