using UnityEngine;

public class FallAnimationTrigger : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string freeFallStateName = "FreeFall";
    [SerializeField] private float crossFadeDuration = 0.15f;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("FallTrigger"))
            return;

        if (animator != null)
        {
            animator.CrossFade(freeFallStateName, crossFadeDuration);
        }
    }
}