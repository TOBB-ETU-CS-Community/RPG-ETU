using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : CharacterStatManager
{
    private PlayerManager player;
    
    [Header("Health")]
    [SerializeField] public float maxHealth = 100;
    [SerializeField] public float currentHealth = 100;
    [SerializeField] public float healthRegenValue = 15;
    [SerializeField] public float healthRegenTimer = 2;
    
    [Header("Health Constraints")]
    [SerializeField] public bool isDead;

    [Header("Stamina")]
    [SerializeField] public float maxStamina = 100;
    [SerializeField] public float currentStamina = 100;
    [SerializeField] public float staminaRegenValue = 15;
    [SerializeField] public float staminaRegenTimer = 2;
    
    [Header("Stamina Constraints")]
    [SerializeField] public bool canSprint;
    [SerializeField] public bool canDodge;
    [SerializeField] public bool canBackstep;
    [SerializeField] public bool canJump;
    [SerializeField] public bool isFatigue;

    private readonly Dictionary<float, bool[]> staminaThresholds = new Dictionary<float, bool[]>
    {
        { 10f, new[]{false,false,false} },
        { 20f, new[]{false,false,true} },  
        { 30f, new[]{false,true,true} },  
        { 9999f, new[]{true,true,true} },
    };

    [SerializeField] public bool statChanged;
    

    public void Awake()
    {
        player = GetComponent<PlayerManager>();
        statChanged = true;
    }
    
    protected override void HandleAllStatChanges()
    {
        HandleHealth();
        HandleStamina();
        HandleStatConstraints();
    }

    protected override void HandleStatConstraints()
    { 
       HandleHealthConstraints();
       HandleStaminaConstraints();
    }


    private void HandleStamina()
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


    private void HandleHealth()
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


    private void HandleHealthConstraints()
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