using System.Collections.Generic;
using Character;
using UnityEngine;

public class PlayerStatManager : CharacterStatManager
{
    private PlayerManager player;
    
    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth = 100;
    public float healthRegenValue = 15;
    public float healthRegenTimer = 2;
    
    [Header("Health Constraints")]
    public bool isDead;

    [Header("Stamina")]
    public float maxStamina = 100;
    public float currentStamina = 100;
    public float staminaRegenValue = 15;
    public float staminaRegenTimer = 2;
    
    [Header("Stamina Constraints")]
    public bool canSprint;
    public bool canDodge;
    public bool canBackstep;
    public bool canJump;
    public bool isFatigue;

    private readonly Dictionary<float, bool[]> staminaThresholds = new Dictionary<float, bool[]>
    {
        { 10f, new[]{false,false,false} },
        { 20f, new[]{false,false,true} },  
        { 30f, new[]{false,true,true} },  
        { 9999f, new[]{true,true,true} },
    };

    public bool statChanged;
    

    public void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
        statChanged = true;
    }
    
    public void HandleAllStatChanges()
    {
        HandleHealth();
        HandleStamina();
        HandleStatConstraints();
    }

    private void HandleStatConstraints()
    { 
       HandleHealthConstraints();
       HandleStaminaConstraints();
    }
    

    public void HandleStamina()
    {
        if (staminaRegenTimer > 0)
        {
            staminaRegenTimer -= Time.deltaTime;
        }
        else
        {
           UpdateStamina(staminaRegenValue * Time.deltaTime, 0f);
        }
        
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        
        if (currentStamina <= 2 && !isFatigue)
        {
            staminaRegenValue = 10;
            UpdateStamina(0f,2f);
            isFatigue = true;
            return;
        }

        if (!(currentStamina >= 30)) return;
        isFatigue = false;
        staminaRegenValue = 15;
    }
    
    private void HandleStaminaConstraints()
    {
        if (isFatigue)
        {
            canSprint = false;
            return;
        }
        
        canSprint = true;
        foreach (var kvp in staminaThresholds)
        {
            if (currentStamina < kvp.Key)
            {
                canDodge = kvp.Value[0];
                canBackstep = kvp.Value[1];
                canJump = kvp.Value[2];
                break;
            }
        }
    }

    public void UpdateStamina(float value, float timer)
    {
        currentStamina += value;
        staminaRegenTimer = staminaRegenTimer > timer ? staminaRegenTimer : timer;
    }
  

    public void HandleHealth()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
            return;
        }
        
        if (healthRegenTimer > 0)
        {
            healthRegenTimer -= Time.deltaTime;
        }
        else
        {
            UpdateHealth(healthRegenValue * Time.deltaTime, 0f);
        }
        
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
      
    }
    
    
    
    public void HandleHealthConstraints()
    {
        if (isDead)
        {
            //game over
        }
        
        
    }
    
    public void UpdateHealth(float value, float timer)
    {
        currentHealth += value;
        healthRegenTimer = healthRegenTimer > timer ? healthRegenTimer : timer;
    }
    
}