using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnManager.instance.SetCheckpoint(transform.position, this);
        }
    }

    public void ActivateCheckpoint()
    {
        gameObject.SetActive(false); // hide when collected
    }

}