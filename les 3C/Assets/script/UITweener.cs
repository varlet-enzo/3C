using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UITweener : MonoBehaviour
{
    public UIAnimationPreset preset;
    
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector2 originalAnchoredPos;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalAnchoredPos = rectTransform.anchoredPosition;
    }

    // Le GD peut appeler cette fonction avec un UnityEvent (ex: OnInteract du coffre)
    // Ou on peut la jouer automatiquement quand l'objet s'active
    void OnEnable()
    {
        if (preset != null)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        float elapsed = 0f;

        // Préparation selon le type
        if (preset.animationType == UIAnimationPreset.AnimType.ScaleBounce) rectTransform.localScale = Vector3.zero;
        if (preset.animationType == UIAnimationPreset.AnimType.FadeIn) canvasGroup.alpha = 0f;
        if (preset.animationType == UIAnimationPreset.AnimType.SlideIn) rectTransform.anchoredPosition = originalAnchoredPos + preset.slideOffset;

        while (elapsed < preset.duration)
        {
            float t = elapsed / preset.duration;
            float curveValue = preset.easeCurve.Evaluate(t);

            // Animation
            if (preset.animationType == UIAnimationPreset.AnimType.ScaleBounce)
                rectTransform.localScale = originalScale * curveValue;
            else if (preset.animationType == UIAnimationPreset.AnimType.FadeIn)
                canvasGroup.alpha = curveValue;
            else if (preset.animationType == UIAnimationPreset.AnimType.SlideIn)
                rectTransform.anchoredPosition = Vector2.Lerp(originalAnchoredPos + preset.slideOffset, originalAnchoredPos, curveValue);

            elapsed += Time.unscaledDeltaTime; // unscaled pour marcher même si le jeu est en pause (Time.timeScale = 0)
            yield return null;
        }

        // Fin propre
        rectTransform.localScale = originalScale;
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = originalAnchoredPos;
    }
}