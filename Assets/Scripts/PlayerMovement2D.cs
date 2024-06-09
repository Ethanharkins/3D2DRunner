using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = FindObjectOfType<GameManager>().GetComponent<AudioSource>();
    }

    private void Update()
    {
        float moveHorizontal = 0;

        if (Input.GetKey(KeyCode.W))
        {
            moveHorizontal = -1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveHorizontal = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, 0.0f) * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, rb.velocity.z);

        if (moveHorizontal != 0 && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(FindObjectOfType<GameManager>().moveSound);
        }

        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.velocity.y) < 0.001f)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            audioSource.PlayOneShot(FindObjectOfType<GameManager>().jumpSound);
        }
    }
}
