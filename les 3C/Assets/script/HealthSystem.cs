using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance { get; private set; }

    public delegate void OnDeathDelegate();
    public event OnDeathDelegate OnDeath;

    [Header("Entity Type")]
    public bool isPlayer = true;
    public PlayerProfileSO profile;

    [Header("Values (Player & Enemy)")]
    private float maxHealth = 100;

    public float currentHealth;

    [Header("Mana (Player Only)")]
    public float maxMana = 50;
    public float currentMana;
    public float manaRegenPerSec = 2f;
    public bool regenerate = true;
    public float healthRegenPerSec = 1f;

    [Header("UI References (Player Only)")]
    public Image healthBar;
    public Image manaBar;
    public Text healthText;
    public Text manaText;

    [Header("Death")]
    public GameObject deathCanvas;
    
    [Header("Events")]
    public UnityEvent onDamaged;
    public UnityEvent onDeathUnityEvent; 

    public bool godMode;
    private bool isDead;

    void Awake()
    {
        if (isPlayer)
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
    }

    void Start()
    {
        if (profile != null)
        {
            maxHealth = profile.maxHealth;
            maxMana = profile.maxMana;
            manaRegenPerSec = profile.manaRegenPerSec;
            healthRegenPerSec = profile.healthRegenPerSec;
        }

        currentHealth = maxHealth;
        currentMana = maxMana;
        
        if (isPlayer && deathCanvas != null)
            deathCanvas.SetActive(false);

        UpdateAllUI();
    }

    void Update()
    {
        if (isDead) return;

        if (regenerate)
        {
            if (godMode && isPlayer)
            {
                currentHealth = maxHealth;
                currentMana = maxMana;
            }
            else
            {
                if (currentHealth < maxHealth)
                {
                   currentHealth = Mathf.Min(maxHealth, currentHealth + healthRegenPerSec * Time.deltaTime);
                }

                if (isPlayer && currentMana < maxMana)
                {
                    currentMana = Mathf.Min(maxMana, currentMana + manaRegenPerSec * Time.deltaTime);
                }
            }
            if (isPlayer) UpdateAllUI();
        }
    }

    public void TakeDamage(float damage)
    {
        if (godMode && isPlayer) return;
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        
        onDamaged?.Invoke();
        
        if (isPlayer) UpdateAllUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void HealDamage(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        if (isPlayer) UpdateAllUI();
    }

    public void UseMana(float amount)
    {
        if (!isPlayer || isDead) return;
        currentMana = Mathf.Max(0, currentMana - amount);
        UpdateAllUI();
    }

    public void RestoreMana(float amount)
    {
        if (!isPlayer || isDead) return;
        currentMana = Mathf.Min(maxMana, currentMana + amount);
        UpdateAllUI();
    }

    public bool HasMana(float amount)
    {
        return isPlayer && currentMana >= amount;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();
        onDeathUnityEvent?.Invoke();

        if (isPlayer)
        {
            if (deathCanvas != null)
            {
                deathCanvas.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
            }
        }
    }

    public void Revive()
    {
        isDead = false;
        currentHealth = maxHealth;
        currentMana = maxMana;
        if (isPlayer && deathCanvas != null)
        {
            deathCanvas.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        UpdateAllUI();
    }

    private void UpdateAllUI()
    {
        if (!isPlayer) return;

        if (healthBar != null)
            healthBar.fillAmount = maxHealth > 0 ? currentHealth / maxHealth : 0f;
        
        if (healthText != null)
            healthText.text = $"{Mathf.Round(currentHealth)}/{maxHealth}";

        if (manaBar != null)
            manaBar.fillAmount = maxMana > 0 ? currentMana / maxMana : 0f;

        if (manaText != null)
            manaText.text = $"{Mathf.Round(currentMana)}/{maxMana}";
    }

    // Compatibility methods if other scripts used them
    public float GetHealthPercentage()
    {
        return maxHealth > 0 ? currentHealth / maxHealth : 0f;
    }
}
