using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "NewPlayerProfile", menuName = "Game/Player Profile")]
public class PlayerProfileSO : ScriptableObject
{
    [Header("Santé & Mana")]
    public float maxHealth = 100f;
    public float maxMana = 50f;
    public float manaRegenPerSec = 2f;
    public float healthRegenPerSec = 1f;

    [Header("Mouvement")]
    public float moveSpeed = 6f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 3f;
    public float walkSpeed = 6f;
    public float rollSpeed = 15f;
    public float jumpHeight = 1.5f;
    public float Jumpcount = 1;
    public float gravity = -9.81f;

    [Header("Roulade")]
    public float rollDuration = 0.5f;
    public float rollDistance = 5f;
    public float rollCooldown = 1f;
    public bool canMoveWhileRolling = false;

    [Header("Combat & Interaction")]
    public int baseDamage = 20;
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    
    public InputActionAsset inputActions; 
}