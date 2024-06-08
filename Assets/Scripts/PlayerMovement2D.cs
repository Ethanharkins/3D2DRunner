using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public AudioClip jumpSound;
    public AudioClip moveSound;
    private Rigidbody rb;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        if (moveHorizontal != 0 && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(moveSound);
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, 0.0f) * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, rb.velocity.z);

        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.001f)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            audioSource.PlayOneShot(jumpSound);
        }
    }
}
