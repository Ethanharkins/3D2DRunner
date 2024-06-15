using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public Vector3 pointA;  // Starting point
    public Vector3 pointB;  // Ending point
    public float speed = 1.0f;  // Speed of movement

    private Vector3 target;  // Current target point

    private void Start()
    {
        target = pointB;  // Start by moving towards pointB
    }

    private void Update()
    {
        // Move towards the target point
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Check if the block has reached the target point
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // Switch target point
            target = target == pointA ? pointB : pointA;
        }
    }

    // Draw the path in the editor for visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointA, pointB);
    }
}
