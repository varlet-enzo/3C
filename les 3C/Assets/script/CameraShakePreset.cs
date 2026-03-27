using UnityEngine;

[CreateAssetMenu(fileName = "NewCameraShake", menuName = "3C Feel/Camera Shake Preset")]
public class CameraShakePreset : ScriptableObject
{
    [Header("Paramètres de la Secousse")]
    [Tooltip("Durée totale de l'effet en secondes")]
    public float duration = 0.5f;
    
    [Tooltip("Puissance de la secousse")]
    public float amplitude = 1f;
    
    [Tooltip("Vitesse de la vibration")]
    public float frequency = 10f;

    [Tooltip("Comment la force évolue dans le temps (ex: fort au début, puis s'estompe)")]
    public AnimationCurve intensityCurve = AnimationCurve.Linear(0, 1, 1, 0); 
}