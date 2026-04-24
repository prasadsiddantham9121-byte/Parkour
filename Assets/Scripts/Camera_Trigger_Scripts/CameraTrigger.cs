using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public CameraController.CameraSide sideToSet;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController cam = Camera.main.GetComponent<CameraController>();
            if (cam != null)
            {
                cam.SetCameraSide(sideToSet);
            }
        }
    }
}