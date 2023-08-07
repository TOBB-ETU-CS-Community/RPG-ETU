using UnityEngine;

public class PlayerManager : CharacterManager
{
    
    
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimationManager playerAnimationManager;
    [HideInInspector] public PlayerStatManager playerStatManager;
    [HideInInspector] public PlayerHUDManager playerHUDManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerInventory playerInventory;


    protected override void Awake()
    {
        base.Awake();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimationManager = GetComponent<PlayerAnimationManager>();
        playerStatManager = GetComponent<PlayerStatManager>();
        playerHUDManager = GetComponent<PlayerHUDManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    protected override void Update()
    {
        base.Update();
        playerLocomotionManager.HandleAllMovement();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        PlayerCamera.instance.HandleAllCameraMovement();
        playerHUDManager.HandleHUD();
    }

    public override void ResetActionFlags()
    {
        isPerformingAction = false;
        applyRootMotion = false;
        canRotate = true;
        canMove = true;
        isLightAttacking = false;
    }
}