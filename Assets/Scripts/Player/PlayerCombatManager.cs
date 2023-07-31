using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    
    PlayerAnimationManager playerAnimationManager;
    PlayerManager player;
    public bool isLightAttacking;
    
    private void Awake()
    {
        playerAnimationManager = GetComponent<PlayerAnimationManager>();
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
    
    public void TakeDamage(float damage)
    {
        print("DAMAGE BABYYYY");
        player.playerStatManager.UpdateHealth(-damage,2f);
        player.playerHUDManager.HandleHUD();
    }
    
}
