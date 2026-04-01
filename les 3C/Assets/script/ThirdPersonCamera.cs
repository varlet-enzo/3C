using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3f, -5f);
    public float sensitivity = 2f;
    public float distance = 5f;
    public float minY = -30f;
    public float maxY = 60f;

    [Header("Auto-Alignement (Obligatoire)")]
    public float alignSpeed = 2f; // Vitesse de retour de la caméra
    public float alignDelay = 1.5f; // Temps avant que l'alignement commence
    private float lastInputTime;

    private float rotX;
    private float rotY;
    private bool isInitialized = false;
    private bool inputEnabled = true;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;

    public Vector3 PlanarForward => Quaternion.Euler(0f, rotX, 0f) * Vector3.forward;
    public Vector3 PlanarRight => Quaternion.Euler(0f, rotX, 0f) * Vector3.right;

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isInitialized = false;
        FindAndSetTarget();
    }

    void Start() { FindAndSetTarget(); }

    void FindAndSetTarget()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                CacheInputActions();
                InitializeCameraPosition();
            }
        }
        else
        {
            CacheInputActions();
            InitializeCameraPosition();
        }
    }

    void CacheInputActions()
    {
        if (target == null) return;

        playerInput = target.GetComponent<PlayerInput>();
        if (playerInput == null || playerInput.actions == null)
        {
            moveAction = null;
            lookAction = null;
            return;
        }

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
    }

    void InitializeCameraPosition()
    {
        if (target != null)
        {
            rotX = target.eulerAngles.y;
            rotY = 20f;
            isInitialized = true;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        if (!isInitialized) { InitializeCameraPosition(); return; }

        if (playerInput == null || moveAction == null || lookAction == null)
        {
            CacheInputActions();
        }

        Vector2 moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        Vector2 lookInput = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;

        float moveX = moveInput.x;
        float moveZ = moveInput.y;

        if (inputEnabled)
        {
            float mouseX = lookInput.x;
            float mouseY = lookInput.y;

            if (Mathf.Abs(mouseX) > 0.1f || Mathf.Abs(mouseY) > 0.1f)
            {
                rotX += mouseX * sensitivity;
                rotY -= mouseY * sensitivity;
                rotY = Mathf.Clamp(rotY, minY, maxY);
                lastInputTime = Time.time; // On reset le timer car le joueur bouge la souris
            }
            else
            {
                // Auto-alignement seulement si le joueur avance vers l'avant.
                HandleAutoAlign(moveX, moveZ);
            }
        }

        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
        Vector3 desiredPos = target.position + rotation * new Vector3(0, 0, -distance);
        transform.position = desiredPos + Vector3.up * offset.y;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    void HandleAutoAlign(float moveX, float moveZ)
    {
        bool isMovingForward = moveZ > 0.1f;
        bool isStrafing = Mathf.Abs(moveX) > 0.1f;

        if (isMovingForward && !isStrafing && Time.time - lastInputTime > alignDelay)
        {
            // On récupère l'angle actuel du joueur
            float targetRotationY = target.eulerAngles.y;
            
            // On lisse la rotation de la caméra vers celle du joueur (Lerp)
            rotX = Mathf.LerpAngle(rotX, targetRotationY, Time.deltaTime * alignSpeed);
        }
    }

    public void SetInputEnabled(bool enabled) { inputEnabled = enabled; }
}