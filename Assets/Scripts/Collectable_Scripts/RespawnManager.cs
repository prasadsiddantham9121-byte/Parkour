using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    private Vector3 currentCheckpoint;
    private Vector3 defaultSpawnPoint;

    private CheckPoint currentCheckpointObj;

    private bool canRespawnFromCheckpoint = false;
    private bool checkpointAlreadyUsed = false;

    // ✅ This is what DeathTrigger should check
    public bool CanUseCheckpoint => canRespawnFromCheckpoint && !checkpointAlreadyUsed;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        defaultSpawnPoint = GameObject.FindGameObjectWithTag("Player").transform.position;
        currentCheckpoint = defaultSpawnPoint;
    }

    public void SetCheckpoint(Vector3 pos, CheckPoint checkpoint)
    {
        currentCheckpoint = pos;
        currentCheckpointObj = checkpoint;

        canRespawnFromCheckpoint = true;
        checkpointAlreadyUsed = false; // ✅ reset when new checkpoint collected

        checkpoint.ActivateCheckpoint();
    }

    public Vector3 GetCheckpoint()
    {
        if (CanUseCheckpoint)
        {
            checkpointAlreadyUsed = true; // ✅ consume checkpoint
            return currentCheckpoint;
        }

        return defaultSpawnPoint;
    }

    public void HandleCheckpointOnRespawn()
    {
        currentCheckpointObj = null;
    }
}