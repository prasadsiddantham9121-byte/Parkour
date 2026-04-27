using UnityEngine;

public class MovingObstacles : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] Transform pointA; // Start position
    [SerializeField] Transform pointB; // End position

    [Header("Movement Settings")]
    [SerializeField] float speed = 2f; // Movement speed
    [SerializeField] bool loop = true; // Ping-pong movement

    [Header("Wait Settings")]
    [SerializeField] bool waitAtPointA = false;
    [SerializeField] float waitTimeAtA = 1f;

    [SerializeField] bool waitAtPointB = false;
    [SerializeField] float waitTimeAtB = 1f;

    float t = 0f;              // Lerp value (0 → 1)
    int direction = 1;         // 1 = A→B, -1 = B→A

    bool isWaiting = false;
    float waitTimer = 0f;

    // 🔥 Platform movement tracking
    Vector3 lastPosition;

    // 🔥 Reference to player script (NOT transform)
    PlayerController player;

    private void Start()
    {
        // Store initial position to calculate movement delta
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (pointA == null || pointB == null) return;

        // ================= WAIT LOGIC =================
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;

                // Reverse direction after waiting
                direction *= -1;
            }

            ApplyPlatformMovement();
            return;
        }

        // ================= MOVEMENT =================
        t += Time.deltaTime * speed * direction;

        // Move between A and B
        transform.position = Vector3.Lerp(pointA.position, pointB.position, t);

        // ===== Reached Point B =====
        if (t >= 1f)
        {
            t = 1f;

            if (waitAtPointB)
            {
                isWaiting = true;
                waitTimer = waitTimeAtB;
            }
            else if (loop)
            {
                direction = -1;
            }
        }
        // ===== Reached Point A =====
        else if (t <= 0f)
        {
            t = 0f;

            if (waitAtPointA)
            {
                isWaiting = true;
                waitTimer = waitTimeAtA;
            }
            else if (loop)
            {
                direction = 1;
            }
        }

        // Apply movement to player
        ApplyPlatformMovement();
    }

    // ================= PLAYER CARRY LOGIC =================
    void ApplyPlatformMovement()
    {
        // Calculate how much platform moved this frame
        Vector3 delta = transform.position - lastPosition;

        // If player is on platform → move player
        if (player != null)
        {
            // IMPORTANT: divide by deltaTime because PlayerController multiplies later
            //player.AddExternalMovement(delta / Time.deltaTime);
        }

        // Update last position
        lastPosition = transform.position;
    }

    // Detect CharacterController contact
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("Player"))
        {
            // Only detect if player is standing ON TOP
            if (hit.moveDirection.y < -0.3f)
            {
                player = hit.transform.GetComponent<PlayerController>();
            }
        }
    }

    // Safety check (player left platform)
    private void LateUpdate()
    {
        if (player != null)
        {
            float dist = Vector3.Distance(player.transform.position, transform.position);

            if (dist > 4f) // tweak if needed
            {
                player = null;
            }
        }
    }
}