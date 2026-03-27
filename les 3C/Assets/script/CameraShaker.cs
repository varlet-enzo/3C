using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }

    [Header("Configuration")]
    [Tooltip("Le preset qui sera joué par défaut si on n'en précise pas d'autre.")]
    public CameraShakePreset defaultPreset; 

    private Transform camTransform;
    private Vector3 originalLocalPos;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        camTransform = transform;
    }

    // =================================================================
    // NOUVELLE FONCTION ULTRA SIMPLE POUR LE GD
    // =================================================================
    public void PlayDefaultShake()
    {
        if (defaultPreset != null)
        {
            PlayShake(defaultPreset);
        }
        else
        {
            Debug.LogWarning("Aucun preset de Camera Shake n'est assigné dans l'inspecteur !");
        }
    }

    // On garde celle-ci au cas où un autre script voudrait forcer un preset différent (ex: une grosse explosion vs un petit coup)
    public void PlayShake(CameraShakePreset preset)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(preset));
    }

    private IEnumerator ShakeRoutine(CameraShakePreset preset)
    {
        originalLocalPos = camTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < preset.duration)
        {
            float percentComplete = elapsed / preset.duration;
            float damper = preset.intensityCurve.Evaluate(percentComplete); 

            Vector3 randomOffset = Random.insideUnitSphere * preset.amplitude * damper;
            camTransform.localPosition = originalLocalPos + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        camTransform.localPosition = originalLocalPos;
    }
}