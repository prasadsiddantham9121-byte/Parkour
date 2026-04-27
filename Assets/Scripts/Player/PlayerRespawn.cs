using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] float fallThreshold = -10f;

    private void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        if (!RespawnManager.instance.TryGetCheckpoint(out Vector3 checkpoint))
        {
            UI_Canvas.instance.ShowLevelFail();
            return;
        }

        CharacterController controller = GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        transform.position = checkpoint;

        if (controller != null)
            controller.enabled = true;

        RespawnManager.instance.HandleCheckpointOnRespawn();
    }
}