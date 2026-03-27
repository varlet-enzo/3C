using UnityEngine;

[CreateAssetMenu(fileName = "NewUIPreset", menuName = "3C Feel/UI Animation Preset")]
public class UIAnimationPreset : ScriptableObject
{
    public enum AnimType { ScaleBounce, FadeIn, SlideIn }
    
    [Header("Type d'animation")]
    public AnimType animationType = AnimType.ScaleBounce;

    [Header("Paramètres")]
    public float duration = 0.3f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Pour le Slide")]
    public Vector2 slideOffset = new Vector2(0, -500);
}