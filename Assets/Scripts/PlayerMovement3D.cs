using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
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
        float moveVertical = 0;

        // Switch W/S to control left/right, and A/D to control forward/backward
        if (Input.GetKey(KeyCode.W))
        {
            moveHorizontal = 1; // Move right
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveHorizontal = -1; // Move left
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveVertical = 1; // Move backward
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveVertical = -1; // Move forward
        }

        // Combine movement in both directions
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * speed;

        // Maintain current vertical velocity while applying movement
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // Play movement sound at intervals while moving
        if ((moveHorizontal != 0 || moveVertical != 0) && isGrounded)
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
        if ((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Jumpable")) && !isGrounded)
        {
            audioSource.PlayOneShot(landSound);
            isGrounded = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Jumpable"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Jumpable"))
        {
            isGrounded = false;
        }
    }
}
