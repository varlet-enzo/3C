using UnityEngine;
using UnityEngine.InputSystem; 

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // ==================== PROFILS ====================
    [Header("Profil Joueur")]
    public PlayerProfileSO profile;
    
    [Header("Configuration Layers")]
    public LayerMask enemyLayer;
    public LayerMask interactableLayer;

    [Header("Configuration Caméra")]
    public Transform playerCamera; 
    public float lookSensitivity = 50f;

    [Header("Auto-Reset Caméra (Manette/Souris)")]
    [Tooltip("Temps sans toucher au stick droit avant le reset")]
    public float idleResetTime = 4f; 
    [Tooltip("Vitesse de retour de la caméra (3 = doux, 10 = rapide)")]
    public float resetSmoothSpeed = 3f;
    private float lastLookTime; 

    // Variables de mouvement synchronisées
    private float moveSpeed = 6f;
    private float sprintSpeed = 10f;
    private float crouchSpeed = 3f;
    private float walkSpeed = 6f;
    private float gravity = -9.81f;
    private float jumpHeight = 1.5f;
    private float rollDuration = 0.5f;
    private float rollDistance = 5f;
    private float rollCooldown = 1f;
    private bool canMoveWhileRolling = false;
    private int baseDamage = 20;
    private float attackRange = 2f;
    private float attackCooldown = 0.5f;

    // Variables d'état
    private CharacterController controller;
    private Animator animator;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private Vector3 rollDirection;
    private float rollDistanceTraveled;
    private bool isGrounded;
    private bool isRolling;
    private bool isSprinting;
    private bool isCrouching;
    private float rollTimer;
    private float rollCooldownTimer;
    private float nextAttackTime;
    private float xRotation = 0f;
    private Transform cam;
    
    private Chest lastDetectedChest; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        lastLookTime = Time.time;

        if (profile != null) ApplyProfile();
        if (Camera.main != null) cam = Camera.main.transform;

        // On bloque le curseur pour le confort
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void ApplyProfile()
    {
        moveSpeed = profile.moveSpeed;
        sprintSpeed = profile.sprintSpeed;
        crouchSpeed = profile.crouchSpeed;
        walkSpeed = profile.walkSpeed;
        gravity = profile.gravity;
        jumpHeight = profile.jumpHeight;
        rollDuration = profile.rollDuration;
        rollDistance = profile.rollDistance;
        rollCooldown = profile.rollCooldown;
        canMoveWhileRolling = profile.canMoveWhileRolling;
        baseDamage = profile.baseDamage;
        attackRange = profile.attackRange;
        attackCooldown = profile.attackCooldown;
    }

    // ==================== INPUT CALLBACKS ====================

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();

        // Si le joueur bouge la vue, on reset le chrono d'inactivité
        if (lookInput.sqrMagnitude > 0.01f)
        {
            lastLookTime = Time.time;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded) Jump();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed) isSprinting = true;
        else if (context.canceled) isSprinting = false;

        if (context.started && isGrounded && rollCooldownTimer <= 0f) Roll();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started) ToggleCrouch();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started) Interact();
    }

    // ==================== LOGIQUE UPDATE ====================

    void Update()
    {
        if (Time.timeScale == 0f || controller == null || !controller.enabled) return;

        isGrounded = controller.isGrounded;

        if (rollCooldownTimer > 0f) rollCooldownTimer -= Time.deltaTime;

        if (isRolling)
        {
            UpdateRoll();
            if (!canMoveWhileRolling)
            {
                ApplyGravity();
                return;
            }
        }

        CheckForInteractable();
        ApplyLook(); 
        MoveWithInput();
        ApplyGravity();
    }

    void ApplyLook()
    {
        if (playerCamera == null) return;

        // Si on est inactif depuis plus de X secondes
        if (Time.time - lastLookTime > idleResetTime)
        {
            // Reset de la rotation verticale (regarder droit devant)
            xRotation = Mathf.Lerp(xRotation, 0f, Time.deltaTime * resetSmoothSpeed);
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            
            // Note: La rotation horizontale se fait via le corps (MoveWithInput)
        }
        else
        {
            // Rotation manuelle classique
            float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }
    }

    void MoveWithInput()
    {
        Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y);

        if (dir.sqrMagnitude > 0.01f)
        {
            // Calcul de l'angle basé sur la caméra
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + (cam != null ? cam.eulerAngles.y : 0f);
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            float currentSpeed = isSprinting ? sprintSpeed : (isCrouching ? crouchSpeed : walkSpeed);
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            // AUTO-ALIGNEMENT : On tourne le corps vers la direction de marche
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }
    }

    // ==================== ACTIONS (Interaction, Saut, etc.) ====================

    void CheckForInteractable()
    {
        float interactDistance = 5f; 
        float radius = 0.5f; 
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(rayOrigin, radius, transform.forward, out hit, interactDistance, interactableLayer))
        {
             Chest chest = hit.collider.GetComponent<Chest>();
             if (chest == null) chest = hit.collider.GetComponentInParent<Chest>();
             
             if (chest != null)
             {
                 if (lastDetectedChest != chest)
                 {
                     if (lastDetectedChest != null) lastDetectedChest.ShowPopup(false);
                     chest.ShowPopup(true);
                     lastDetectedChest = chest;
                 }
             }
        }
        else if (lastDetectedChest != null)
        {
            lastDetectedChest.ShowPopup(false);
            lastDetectedChest = null;
        }
    }

    void Interact()
    {
        float interactDistance = 5f;
        float radius = 1.0f;
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(rayOrigin, radius, transform.forward, out hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                interactable.OnInteract();
                Chest chest = hit.collider.GetComponent<Chest>();
                if (chest != null) chest.ShowPopup(false);
            }
        }
    }

    void Attack()
    {
        if (animator != null) animator.SetTrigger("Attack");
        Vector3 attackPosition = transform.position + transform.forward * (attackRange / 2);
        Collider[] hitEnemies = Physics.OverlapSphere(attackPosition, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            var health = enemy.GetComponent<HealthSystem>();
            if (health != null) health.TakeDamage(baseDamage);
        }
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void Roll()
    {
        if (isRolling) return;
        isRolling = true;
        rollTimer = rollDuration;
        rollCooldownTimer = rollCooldown;
        rollDistanceTraveled = 0f;

        Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y);
        if (dir.sqrMagnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + (cam != null ? cam.eulerAngles.y : 0f);
            rollDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }
        else rollDirection = transform.forward;

        rollDirection.y = 0;
        rollDirection.Normalize();
    }

    void UpdateRoll()
    {
        rollTimer -= Time.deltaTime;
        float speedNeeded = rollDistance / rollDuration;
        Vector3 movement = rollDirection * speedNeeded * Time.deltaTime;
        controller.Move(movement);
        rollDistanceTraveled += movement.magnitude;
        if (rollTimer <= 0f || rollDistanceTraveled >= rollDistance) isRolling = false;
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        if (animator != null) animator.SetBool("IsCrouching", isCrouching);
    }

    void ApplyGravity()
    {
        if (!isGrounded) velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -2f;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(origin + transform.forward * 5f, 1.0f);
    }
}