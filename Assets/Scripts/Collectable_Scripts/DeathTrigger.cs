using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player == null) return;

            // 🚫 Prevent triggering while already respawning
            if (player.IsRespawning) return;

            hasTriggered = true;

            if (RespawnManager.instance.CanUseCheckpoint)
            {
                // ✅ First death → respawn
                player.StartCoroutine(HandleRespawn(player));
            }
            else
            {
                // ❌ Second death → FAIL
                Debug.Log("LEVEL FAIL");
                UI_Canvas.instance.ShowLevelFail();
            }
        }
    }

    private IEnumerator HandleRespawn(PlayerController player)
    {
        yield return player.StartCoroutine(player.RespawnRoutine());

        // ✅ Reset trigger AFTER respawn completes
        hasTriggered = false;
    }
}