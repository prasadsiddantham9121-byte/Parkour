using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {

        if (hasTriggered)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {

            PlayerController player = other.GetComponent<PlayerController>();

            if (player == null)
            {
                return;
            }

            if (player.IsRespawning)
            {
                return;
            }

            hasTriggered = true;

            if (RespawnManager.instance.CanUseCheckpoint)
            {
                //player.StartCoroutine(HandleRespawn(player));
            }
            else
            {
                UI_Canvas.instance.ShowLevelFail();
            }
        }
    }

    //private IEnumerator HandleRespawn(PlayerController player)
    //{

    //    yield return player.StartCoroutine(player.RespawnRoutine());

    //    hasTriggered = false;
    //}
}