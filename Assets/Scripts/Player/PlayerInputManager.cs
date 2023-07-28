using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    private PlayerManager player;
    
    [SerializeField] private Vector2 movementInput;
    [SerializeField] private Vector2 cameraInput;


    [Header("Movement Flags")] public float verticalInput;

    public float horizontalInput;
    public float moveInputAmount;
    public float moveAmount;

    [Header("Camera Flags")] public float cameraVerticalInput;

    public float cameraHorizontalInput;
    private PlayerControls controls;

    public bool lockOnInput;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            player = GetComponent<PlayerManager>();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        HandleMovementInput();
        HandleCameraInput();
    }

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new PlayerControls();
            controls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            controls.PlayerCamera.CameraControls.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
            controls.PlayerCamera.LockOn.performed += ctx => HandleLockOnInput();
            controls.PlayerMovement.Dodge.performed += ctx => HandleDodgeInput();

            controls.PlayerMovement.Sprint.performed += ctx => HandleSprintInput(true);
            controls.PlayerMovement.Sprint.canceled += ctx => HandleSprintInput(false);

            controls.PlayerMovement.Jump.performed += ctx => HandleJumpInput();

            controls.PlayerAttack.LightAttack.performed += ctx => HandleAttackInput();
            controls.PlayerItems.ToggleWeapon.performed += ctx => HandleToggleWeaponInput();
        }

        controls.Enable();
    }

    public void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveInputAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        moveAmount = Mathf.Lerp(moveAmount, moveInputAmount, 0.02f);

        if (moveAmount < 0.00001f) moveAmount = 0;
    }

    public void HandleCameraInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }

    public void HandleDodgeInput()
    {
        if (moveInputAmount > 0)
        {
            player.playerLocomotionManager.PerformDodge();
        }
        else
        {
            player.playerLocomotionManager.PerformBackstep();
        }
    }

    public void HandleLockOnInput()
    {
        player.playerLocomotionManager.HandleLockOn();
    }


    public void HandleSprintInput(bool sprint)
    {
        player.playerLocomotionManager.HandleSprint(sprint);
    }
    
    public void HandleJumpInput()
    {
        player.playerLocomotionManager.HandleJump();
    }
    
    public void HandleAttackInput()
    {
        player.playerCombatManager.HandleLightAttack(player.playerInventory.weapon);
    }
    
    public void HandleToggleWeaponInput()
    {
        player.playerInventory.ToggleWeapon();
    }
    
    
}