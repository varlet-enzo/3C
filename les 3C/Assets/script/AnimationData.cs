using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimMapping", menuName = "3C Feel/Animation Mapping")]
public class AnimationMapping : ScriptableObject
{
    [Header("Paramètres de déplacement")]
    public string moveSpeedParam = "Speed";
    public string isGroundedParam = "IsGrounded";
    public string isCrouchingParam = "IsCrouching";

    [Header("Actions")]
    public string jumpTrigger = "Jump";
    public string attackTrigger = "Attack";
    public string rollTrigger = "Roll";
}