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

        playerAnimationManager.PlayTargetActionAnimation(weapon.Light_Attack_1, true);
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
    }
    
    public void HitEnd()
    {
        weaponSlotManager.weaponHitboxManager.DisableCollider();
    }
    
}
