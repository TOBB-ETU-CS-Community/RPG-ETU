using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    
    PlayerAnimationManager playerAnimationManager;
    PlayerManager player;
    
    private void Awake()
    {
        playerAnimationManager = GetComponent<PlayerAnimationManager>();
        player = GetComponent<PlayerManager>();
    }
    
    public void HandleLightAttack(WeaponItem weapon)
    {
        
        if (weapon == null)
            return;

        if (player.isPerformingAction)
            return;

        playerAnimationManager.PlayTargetActionAnimation(weapon.Light_Attack_1, true);
        
    }
    
    public void TakeDamage(float damage)
    {
        print("DAMAGE BABYYYY");
        player.playerStatManager.UpdateHealth(-damage,2f);
        player.playerHUDManager.HandleHUD();
    }
    
}
