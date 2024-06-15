using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public Vector3 pointA1;  // First camera's starting point
    public Vector3 pointB1;  // First camera's ending point
    public Vector3 pointA2;  // Second camera's starting point
    public Vector3 pointB2;  // Second camera's ending point
    public float speed = 1.0f;  // Speed of movement

    private Vector3 target;  // Current target point
    private Vector3 currentPointA;  // Current starting point
    private Vector3 currentPointB;  // Current ending point
    private bool isUsingSecondCamera = false;  // Flag to check if second camera is being used

    private void Start()
    {
        SetMovementPoints(pointA1, pointB1);  // Start by moving according to the first camera
    }

    private void Update()
    {
        // Move towards the target point
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Check if the block has reached the target point
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // Switch target point
            target = target == currentPointA ? currentPointB : currentPointA;
        }
    }

    public void SetMovementPoints(Vector3 pointA, Vector3 pointB, bool useSecondCamera = false)
    {
        isUsingSecondCamera = useSecondCamera;
        currentPointA = pointA;
        currentPointB = pointB;
        target = currentPointB;  // Start by moving towards pointB
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointA1, pointB1);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pointA2, pointB2);
    }
}
