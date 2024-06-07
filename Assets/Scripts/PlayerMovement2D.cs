using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private bool isGrounded;
    private Rigidbody rb;
    public float groundCheckRadius = 1.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float moveX = 0;

        if (Input.GetKey(KeyCode.A))
        {
            moveX = -moveSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveX = moveSpeed;
        }

        Vector3 move = new Vector3(moveX, rb.velocity.y, 0);
        rb.velocity = move;
    }

    void Jump()
    {
        isGrounded = IsGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        Vector3 playerPosition = transform.position;
        playerPosition.z = 0; // Ignore Z-axis for 2D ground check

        // Check for ground directly below the player
        if (Physics.Raycast(playerPosition, Vector3.down, groundCheckRadius, LayerMask.GetMask("Ground")))
        {
            return true;
        }

        // Check for nearby jumpable objects within the ground check radius
        Collider[] hitColliders = Physics.OverlapSphere(playerPosition, groundCheckRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Jumpable"))
            {
                return true;
            }
        }

        return false;
    }

    private void OnCollisionEnter(Collision collision)
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
