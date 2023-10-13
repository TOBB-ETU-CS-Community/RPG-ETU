using System.Collections.Generic;
using UnityEngine;

public class EnemyStatManager : CharacterStatManager
{
    private EnemyManager enemy;
    
    
    [Header("Health")]
    [SerializeField] public float maxHealth = 100;
    [SerializeField] public float currentHealth = 100;

    [Header("Health Constraints")]
    [SerializeField] public bool isDead;

    [Header("UI")]
    [SerializeField] public bool isInArea;
    

    public void Awake()
    {
        enemy = GetComponent<EnemyManager>();
    }
    
    protected override void HandleAllStatChanges()
    {
        HandleHealth();
    }

    protected override void HandleStatConstraints()
    { 
       HandleHealthConstraints();
    }

    private void HandleHealth()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
            return;
        }
        
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    }

    private void HandleHealthConstraints()
    {
        if (isDead)
        {
            Destroy(gameObject);
        }
    }
    
    public void UpdateHealth(float health)
    {
        currentHealth += health;
    }
    
    

}