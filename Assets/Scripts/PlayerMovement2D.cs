using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private bool isGrounded;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Debug.Log("PlayerMovement2D script started");
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
            Debug.Log("Moving left");
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveX = moveSpeed;
            Debug.Log("Moving right");
        }

        Vector3 move = new Vector3(moveX, rb.velocity.y, 0);
        rb.velocity = move;
    }

    void Jump()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f) || IsGroundedOnJumpable();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Jumping");
        }
    }

    private bool IsGroundedOnJumpable()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
        {
            if (hit.collider.gameObject.CompareTag("Jumpable"))
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
