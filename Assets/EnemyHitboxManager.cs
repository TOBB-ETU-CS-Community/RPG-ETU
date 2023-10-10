using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitboxManager : MonoBehaviour
{
    public EnemyManager enemy;
    public CapsuleCollider hitboxCollider;
    void Awake()
    {
        enemy = GetComponentInParent<EnemyManager>();
        hitboxCollider = GetComponentInChildren<CapsuleCollider>();
    }
    
    void FixedUpdate()
    {
        MyCollisions();
    }

    void MyCollisions()
    {
        //check if there is any collisions in hitbox collider
        Collider[] hitColliders = Physics.OverlapCapsule(hitboxCollider.bounds.center, hitboxCollider.bounds.center, hitboxCollider.radius);
        
        //if there is a collision
        if (hitColliders.Length > 0)
        {
            //for each collision
            foreach (var hitCollider in hitColliders)
            {
                //if the collision is a player
                if (hitCollider.gameObject.CompareTag("Weapon"))
                {
                    Debug.Log("Enemy hit player");
                    var player = hitCollider.gameObject.GetComponentInParent<PlayerManager>();
                    player.isHitStunned = true;
                    enemy.enemyCombatManager.TakeDamage(10f);
                }
            }
        }
    }

}
