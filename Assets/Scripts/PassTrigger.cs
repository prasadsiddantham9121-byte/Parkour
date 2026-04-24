using UnityEngine;

public class PassTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI_Canvas.instance.ShowLevelComplete();
        }
    }
}