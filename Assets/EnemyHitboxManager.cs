using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitboxManager : MonoBehaviour
{
    public EnemyManager enemy;
    public Collider hitboxCollider;
    void Awake()
    {
        enemy = GetComponentInParent<EnemyManager>();
        hitboxCollider = GetComponent<Collider>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("Enemy hit player");
            var player = other.gameObject.GetComponentInParent<PlayerManager>();
            player.isHitStunned = true;
            enemy.enemyCombatManager.TakeDamage(10f);
        }
    }
}
