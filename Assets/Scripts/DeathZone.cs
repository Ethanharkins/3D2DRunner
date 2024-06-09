using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour
{
    public ParticleSystem deathEffect;
    public ParticleSystem respawnEffect;
    public float respawnDelay = 0.5f;  // Shorter delay for faster respawn

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleDeath(other.gameObject));
        }
    }

    private IEnumerator HandleDeath(GameObject player)
    {
        Camera activeCamera = Camera.main;

        // Play the death effect immediately
        Instantiate(deathEffect, player.transform.position, Quaternion.identity);

        // Wait for a short duration to ensure the death effect is visible
        yield return new WaitForSeconds(0.1f);

        player.SetActive(false);  // Hide the player

        yield return new WaitForSeconds(respawnDelay);  // Short delay before respawn

        player.SetActive(true);  // Show the player

        RespawnManager.Instance.RespawnPlayer(player, respawnEffect);

        // Ensure the camera is active during the entire process
        activeCamera.gameObject.SetActive(true);
    }
}
