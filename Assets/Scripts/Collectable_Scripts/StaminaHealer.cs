using UnityEngine;

public class StaminaHealer : MonoBehaviour
{
    [Header("Healing Settings")]
    [SerializeField] float healRate = 20f; // YOU control this

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.HealStamina(healRate);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        this.gameObject.SetActive(false);
    }
}