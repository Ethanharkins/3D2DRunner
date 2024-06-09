using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPoint;  // The respawn point associated with this checkpoint

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnManager.Instance.SetCurrentCheckpoint(this);
        }
    }
}
