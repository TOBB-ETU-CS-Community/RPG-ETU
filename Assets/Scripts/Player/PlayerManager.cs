using UnityEngine;

public class PlayerManager : CharacterManager
{
    
    
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimationManager playerAnimationManager;
    [HideInInspector] public PlayerStatManager playerStatManager;
    [HideInInspector] public PlayerHUDManager playerHUDManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerInventory playerInventory;
    
    private PlayerCamera cameraIns => PlayerCamera.instance;

    [SerializeField] private float timeScale = 1f;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
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
        Time.timeScale = timeScale;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (cameraIns.isLockedOn)
        {
            cameraIns.HandleLockOnCameraMovement();
        }
        else
        {
            cameraIns.HandleAllCameraMovement();
        }
        
        playerHUDManager.HandleHUD();
    }

    public override void ResetActionFlags()
    {
        isPerformingAction = false;
        applyRootMotion = false;
        canRotate = true;
        canMove = true;
        isLightAttacking = false;
        isHitStunned = false;
    }
}