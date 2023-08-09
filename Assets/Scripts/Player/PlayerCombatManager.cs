using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    
    PlayerAnimationManager playerAnimationManager;
    WeaponSlotManager weaponSlotManager;
    PlayerManager player;
    
    public bool isLightAttacking;
    private void Awake()
    {
        playerAnimationManager = GetComponent<PlayerAnimationManager>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        player = GetComponent<PlayerManager>();
    }
    
    public void HandleLightAttack(WeaponItem weapon)
    {
       
        if (player.isPerformingAction)
        {
            if (isLightAttacking)
                playerAnimationManager.UpdateAnimatorTriggerParameters("Light_Trigger");
            return;
        }

        playerAnimationManager.PlayTargetActionAnimation(weapon.Light_Attack_1, true, 0.2f, true);
        isLightAttacking = true;
    }
    
    public override void TakeDamage(float damage)
    {
        player.playerStatManager.UpdateHealth(-damage,2f);
        player.playerHUDManager.HandleHUD();
    }
    
    public void HitStart()
    {
        weaponSlotManager.weaponHitboxManager.EnableCollider();
        player.canRotate = false;
    }
    
    public void HitEnd()
    {
        weaponSlotManager.weaponHitboxManager.DisableCollider();
    }

    public void AnimStart()
    {
        player.canRotate = true;
    }

    public GameObject GetClosestTarget()
    {
        GameObject closestTarget = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (var enemy in EnemyParentalControl.instance.enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, currentPosition);
            if (distance < minDistance)
            {
                closestTarget = enemy;
                minDistance = distance;
            }
        }

        return closestTarget;
    }
}
