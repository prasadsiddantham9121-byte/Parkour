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
        Vector3 checkpoint = RespawnManager.instance.GetCheckpoint();

        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        transform.position = checkpoint;

        if (controller != null)
            controller.enabled = true;

        // THIS IS THE IMPORTANT PART
        RespawnManager.instance.HandleCheckpointOnRespawn();
    }
}