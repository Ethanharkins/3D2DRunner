using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private Checkpoint currentCheckpoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentCheckpoint(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    public void RespawnPlayer(GameObject player, ParticleSystem respawnEffect)
    {
        if (currentCheckpoint != null)
        {
            player.transform.position = currentCheckpoint.respawnPoint.position;
        }
        else
        {
            player.transform.position = Vector3.zero;  // Default respawn position
        }

        Instantiate(respawnEffect, player.transform.position, Quaternion.identity);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
