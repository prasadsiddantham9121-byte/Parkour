using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;  //
    [SerializeField] float rotationSpeed = 500f; //

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f; //
    [SerializeField] Vector3 groundCheckOffset; //
    [SerializeField] LayerMask groundLayer; //

    bool isGrounded; //
    bool hasControl = true;
    public bool inAction { get; private set; } //
    public bool playerHanging { get; set; } //

    float ySpeed; //
    Quaternion targetRotation; //

    public EnvironmentScanner environmentScanner; //
    public bool playerOnLedge { get; set; } //
    public LedgeInfo LedgeInfo { get; set; } //
    Vector3 moveDir; //
    [SerializeField] Vector3 requiredMoveDir;  //  // this is for when player on ledge of an object not allowing the player to get down until press jump button so for that making an angle
    Vector3 velocity; //


    [Header("References")]
    CameraController cameraController; //
    Animator animator; //
    CharacterController characterController; //

    bool isRespawning;

    [Header("Stamina System")]
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float staminaDecreaseRate = 10f;

    float currentStamina;
    bool isOutOfStamina;

    Vector3 externalMovement; // this movement is coming from moving Obsticals

    // ===================== Respawning =========================
    public bool IsRespawning => isRespawning;

    private void Awake()
    {
        currentStamina = maxStamina;

        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        environmentScanner = GetComponent<EnvironmentScanner>();
    }

    private void Update()
    {
        if (!hasControl)
            return;

        if (playerHanging)
            return;

        // here making player rotation to 0 when player wants to move without performing the jump .
        velocity = Vector3.zero;

        if (isGrounded)
        {
            //ySpeed = -0.5f;

            ySpeed = 0f;


            // here when player on the obstacle he can move left/right, forward/backward directions freely until we press space button.
            velocity = moveDir * moveSpeed;

            // ✅ Now moveDir is accessible here
            playerOnLedge = environmentScanner.CheckLedge(moveDir, out LedgeInfo ledgeInfo);

            if (playerOnLedge)
            {
                LedgeInfo = ledgeInfo;
                PlayerLedgeMovement();
                Debug.Log("Player is on Ledge");
            }

            animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, 0.2f, Time.deltaTime);

        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;

            //here making player to not move while is in jump action
            velocity = transform.forward * moveSpeed / 2;
        }
        velocity.y = ySpeed;

        //characterController.Move(velocity * Time.deltaTime);


        PlayerMovement();
        HandleStamina();
        GroundCheck();

        animator.SetBool("isGrounded", isGrounded);

    }

    public void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        var moveInput = (new Vector3(horizontal, 0, vertical)).normalized;

        requiredMoveDir = cameraController.PlanarRotation * moveInput;

        //characterController.Move(velocity * Time.deltaTime);

        //====================================== Newly Added for moving Obsticals=======================
        // Combine player movement + platform movement
        Vector3 finalMove = velocity + externalMovement;

        // Move player once (VERY IMPORTANT)
        characterController.Move(finalMove * Time.deltaTime);

        // Reset after applying (so it doesn't stack infinitely)
        externalMovement = Vector3.zero;

        if (moveAmount > 0 && moveDir.magnitude > 0.1f)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        moveDir = requiredMoveDir;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
            rotationSpeed * Time.deltaTime);

    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void PlayerLedgeMovement()
    {
        float angle = Vector3.Angle(LedgeInfo.surfacehit.normal, requiredMoveDir);

        if(angle < 90)
        {
            velocity = Vector3.zero;
            moveDir = Vector3.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    public IEnumerator PerformAction(string AnimationName, CompareTargetParameter ctp = null, Quaternion RequiredRotation = new Quaternion(), 
        bool LookAtObstacle = false, float ParkourActionDelay = 0f)
    {
        inAction = true;
       

        // Using CrossFade to create smooth transition between two animations instead of snappy switch
        animator.CrossFadeInFixedTime(AnimationName, 0.2f);
        //animator.Play(action.AnimName);
        yield return null; // This is kept to finish the parkour action until the last frame


        // This is using to get next animation state from animator layers
        var animState = animator.GetNextAnimatorStateInfo(0);

        //checking the anim state and animation name both are same or not in below
        if (!animState.IsName(AnimationName))
            Debug.LogError("The Parkour Animation Name is Wrong!");

        //yield return new WaitForSeconds(animState.length);

        float rotateStartTime = (ctp != null) ? ctp.startTime : 0f; 

        // Making player Smoothly rotate towards obstacal while wait for the animation to be complete
        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;

            float normalizedTimerCounter = timer / animState.length;

            // Rotate the player towards the obstacle 
            if (LookAtObstacle && normalizedTimerCounter > rotateStartTime)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, RequiredRotation, rotationSpeed * Time.deltaTime);

            if (ctp != null)
                CompareTarget(ctp);

            // here we are stopping loop to stop the transition from vaultFence to locomotion transition
            // in below line the zero (0) it mean the layer index number in animator.
            if (animator.IsInTransition(0) && timer > 0.65f)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(ParkourActionDelay);

       
        inAction = false;
    }

    // this is helpful to match the target position on the hit object at the top point of an object
    void CompareTarget(CompareTargetParameter compareTargetParameter)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(compareTargetParameter.position, transform.rotation, compareTargetParameter.bodyPart,
            new MatchTargetWeightMask(compareTargetParameter.positionWeight, 0), compareTargetParameter.startTime, compareTargetParameter.endTime);
    }


    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if (!hasControl)
        {
            // To reset the Animator Parameters
            animator.SetFloat("moveAmount", 0f);
            targetRotation = transform.rotation;
        }
    }

    // here enabling the character controller when the player climbs to top 
    public void EnableCC(bool enabled)
    {
        characterController.enabled = enabled;
    }

    public void ResetRequiredRotation()
    {
        targetRotation = transform.rotation;
    }

    // creating property to use this variable in parkourController
    public float RotationSpeed => rotationSpeed;
    
    public bool HasPlayerControl
    {
        get => hasControl;
        set => hasControl = value;
    }

    // ========================================= Player Respawn Mechanism ==============================
    public IEnumerator RespawnRoutine()
    {
        if (isRespawning) yield break;
        isRespawning = true;

        // Disable control
        SetControl(false);

        playerHanging = false;
        playerOnLedge = false;
        inAction = false;

        // Optional: stop movement completely
        ySpeed = 0f;
        velocity = Vector3.zero;
        moveDir = Vector3.zero;

        yield return new WaitForSeconds(0.2f); // small delay (feels better)

        // Disable CharacterController before teleport (VERY IMPORTANT)
        characterController.enabled = false;

        // Move player to checkpoint
        transform.position = RespawnManager.instance.GetCheckpoint() + Vector3.up * 1.5f;

        // RESETING CAMERA PROPERLY AFTER RESPAWN
        if (cameraController != null)
        {
            cameraController.ResetToDefaultInstant();
        }

        // Enable CharacterController back
        characterController.enabled = true;

        // Optional: reset rotation
        ResetRequiredRotation();

        yield return new WaitForSeconds(0.1f);

        // Enable control again
        SetControl(true);

        // If you're using checkpoint visuals
        RespawnManager.instance.HandleCheckpointOnRespawn();

        isRespawning = false;
    }

    //===================================== Stamina Logic =============================================
    void HandleStamina()
    {
        // Player moving condition
        bool isMoving = moveDir.magnitude > 0.1f && isGrounded && !inAction && !playerHanging;

        if (isMoving)
        {
            currentStamina -= staminaDecreaseRate * Time.deltaTime;
        }

        // Clamp value
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // When stamina is empty
        if (currentStamina <= 0 && !isOutOfStamina)
        {
            isOutOfStamina = true;

            UI_Canvas.instance.ShowLevelFail();

            Debug.Log("Level Failed");
        }

        // Reset flag if stamina is above 0
        if (currentStamina > 0)
        {
            isOutOfStamina = false;
        }
    }

    //================================ To check Healing system =======================
    public void HealStamina(float amount)
    {
        currentStamina += amount * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    //===================================Moving Obtical helps to move Player===========================
    public void AddExternalMovement(Vector3 movement)
    {
        externalMovement += movement;
    }

    public float StaminaNormalized => currentStamina / maxStamina;
}

public class CompareTargetParameter
{
    public Vector3 position;
    public AvatarTarget bodyPart;
    public Vector3 positionWeight;
    public float startTime;
    public float endTime;
}
