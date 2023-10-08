using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    
    PlayerAnimationManager playerAnimationManager;
    WeaponSlotManager weaponSlotManager;
    PlayerManager player;

    public bool isLightAttacking;
    public GameObject magicPrefab;
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

    public void HandleMagicAttack()
    {
        if(player.isPerformingAction) return;
        
        playerAnimationManager.PlayTargetActionAnimation("Magic", true, 0.2f, true);
        
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

    public void MagicStart()
    {
        print("Magic Start");
        var cameraTemp = PlayerCamera.instance;

        if (cameraTemp.isLockedOn)
        {
            GameObject target = cameraTemp.lockOnTarget;
            Quaternion targetRotation = target.transform.rotation * Quaternion.Euler(-90, 0, 0);
            Vector3 targetPosition = target.transform.position;
            
            RaycastHit hit;
            if (Physics.Raycast(targetPosition, Vector3.down, out hit, 10f))
            {
                targetPosition.y = hit.point.y;
            }
            var flame = Instantiate(magicPrefab, targetPosition, targetRotation);
            GameObject.Destroy(flame, 5f);
        }

      
           
            
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
