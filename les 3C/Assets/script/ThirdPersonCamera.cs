using UnityEngine;
using UnityEngine.SceneManagement;

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
                InitializeCameraPosition();
            }
        }
        else { InitializeCameraPosition(); }
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

        if (inputEnabled)
        {
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");

            if (Mathf.Abs(mouseX) > 0.1f || Mathf.Abs(mouseY) > 0.1f)
            {
                rotX += mouseX * sensitivity;
                rotY -= mouseY * sensitivity;
                rotY = Mathf.Clamp(rotY, minY, maxY);
                lastInputTime = Time.time; // On reset le timer car le joueur bouge la souris
            }
            else
            {
                // AUTO-ALIGNEMENT : Si le joueur avance et ne touche pas à la souris
                HandleAutoAlign();
            }
        }

        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
        Vector3 desiredPos = target.position + rotation * new Vector3(0, 0, -distance);
        transform.position = desiredPos + Vector3.up * offset.y;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    void HandleAutoAlign()
    {
        // On vérifie si le joueur est en train de se déplacer (via sa vélocité ou son input)
        // Ici, on regarde si le temps écoulé depuis le dernier mouvement de souris est suffisant
        if (Time.time - lastInputTime > alignDelay)
        {
            // On récupère l'angle actuel du joueur
            float targetRotationY = target.eulerAngles.y;
            
            // On lisse la rotation de la caméra vers celle du joueur (Lerp)
            rotX = Mathf.LerpAngle(rotX, targetRotationY, Time.deltaTime * alignSpeed);
        }
    }

    public void SetInputEnabled(bool enabled) { inputEnabled = enabled; }
}