using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    private Vector3 currentCheckpoint;
    private Vector3 defaultSpawnPoint;

    private CheckPoint currentCheckpointObj;

    private bool canRespawnFromCheckpoint = false;
    private bool checkpointAlreadyUsed = false;

    public bool CanUseCheckpoint => canRespawnFromCheckpoint && !checkpointAlreadyUsed;

    private void Awake()
    {
        instance = this;
        Debug.Log("[RespawnManager] Instance Created");
    }

    private void Start()
    {
        defaultSpawnPoint = GameObject.FindGameObjectWithTag("Player").transform.position;
        currentCheckpoint = defaultSpawnPoint;

        Debug.Log("[RespawnManager] Default Spawn Set To: " + defaultSpawnPoint);
    }

    public void SetCheckpoint(Vector3 pos, CheckPoint checkpoint)
    {
        currentCheckpoint = pos;
        currentCheckpointObj = checkpoint;

        canRespawnFromCheckpoint = true;
        checkpointAlreadyUsed = false;

        Debug.Log("[RespawnManager] NEW CHECKPOINT SET");
        Debug.Log("[RespawnManager] Checkpoint Position: " + currentCheckpoint);
        Debug.Log("[RespawnManager] Checkpoint Reset / Available Again");

        checkpoint.ActivateCheckpoint();
    }

    public bool TryGetCheckpoint(out Vector3 checkpointPos)
    {
        checkpointPos = Vector3.zero;

        if (CanUseCheckpoint)
        {
            checkpointAlreadyUsed = true;
            checkpointPos = currentCheckpoint;

            Debug.Log("[RespawnManager] USING CHECKPOINT");
            return true;
        }

        Debug.Log("[RespawnManager] No Checkpoint Available");
        return false;
    }


    public void HandleCheckpointOnRespawn()
    {
        Debug.Log("[RespawnManager] Clearing Current Checkpoint Reference");

        currentCheckpointObj = null;
    }
}