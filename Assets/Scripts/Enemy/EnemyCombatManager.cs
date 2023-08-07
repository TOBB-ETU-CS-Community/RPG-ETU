using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatManager : CharacterCombatManager
{
    public EnemyManager enemy;
    void Awake()
    {
        enemy = GetComponent<EnemyManager>();
    }

  
    public override void TakeDamage(float damage)
    {
        enemy.enemyStatManager.UpdateHealth(-damage);
        enemy.enemyUIManager.HandleUI();
    }

    private void OnTriggerEnter(Collider other)
    {
       
            if (other.gameObject.CompareTag("Weapon"))
            {
                Debug.Log("Enemy hit player");
                TakeDamage(10f);
            }
    }
}
