using System;
using System.Collections;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    [Header("Player Specific")]
    private PlayerManager player;
    private Rig[] rigs;
    
    [Header("Transforms")]
    private Transform cameraTransform;
    public Transform groundCheckTransform;
    public Transform wallCheckTransformLeft;
    public Transform wallCheckTransformRight;
    
    [Header("Input")]
    public float verticalInput;
    public float horizontalInput;
    
    [Header("Movement Amounts")]
    public float moveAmount;
    public float leanAmount;
    public float jumpAmount;
 
    
    [Header("Movement Directions")]
    public Vector3 moveDirection;
    public Vector3 leanDirection;
    public Vector3 targetMoveDirection;
    public Vector3 targetRotateDirection;
    
    [Header("Flags")]
    public bool isGrounded;
    public bool isJumping;
    public bool isFalling;
    public bool isLockedOn;
    public bool isSliding;
    public bool wallRunLeftAllowed;
    public bool wallRunRightAllowed;
    public bool isWallRunning;


    [Header("Vectors")] 
    [SerializeField] private Vector3 groundVelocity;
    [SerializeField] private Vector3 airVelocity;
    public Vector3 realVelocity;
    public Vector3 movementAnimParams;
    public Vector3 OldVector;
    
    [Header("Movement Settings")]
    public float speed;
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] public float jumpHeight = 15f;
    [SerializeField] public float dodgeRange = 4f;
    [SerializeField] public AnimationCurve dodgeCurve;

    [Header("Movement Multipliers")]
    [SerializeField] public float moveDirectionTimer = 7f;
    [SerializeField] public float rotateMultiplier = 10f;
    [SerializeField] public float gravityMultiplier = -10f;
    [SerializeField] public float jumpMultiplier = 2f;
    [SerializeField] public float airMultiplier = 10f;

   
    public event Action onJump;
    public event Action setAnimParams;
    public event Action setRotateDirection;
    public event Action onWallJump;

    protected override void Awake()
    {
        
        base.Awake();
        player = GetComponent<PlayerManager>();
        rigs = GetComponentsInChildren<Rig>();
        OldVector = player.transform.position;
        
        onJump = delegate { GroundJump(); };
        setAnimParams = delegate { FreeCamParams(); };
        setRotateDirection = delegate { FreeRotation(); };
    }

    public void HandleAllMovement()
    {
        GetMovementValues();
        HandleMovement();
        HandleRotation();
        HandleVerticalMovement();
        UpdateMultipliers();
        
        realVelocity = airVelocity + groundVelocity;
        
        if(!isGrounded)
            WallRun();
        
        Debug.DrawLine(transform.position, transform.position + transform.forward*3, Color.red);
        Debug.DrawLine(transform.position, transform.position + targetMoveDirection*3, Color.blue);
        Debug.DrawLine(transform.position, transform.position + realVelocity, Color.yellow);
    }
    
    private void GetMovementValues()
    {
        var inputInstance = PlayerInputManager.instance;
        var cameraInstance = PlayerCamera.instance;
        
        verticalInput = inputInstance.verticalInput;
        horizontalInput = inputInstance.horizontalInput;
        moveAmount = inputInstance.moveAmount;
        
        isLockedOn = cameraInstance.isLockedOn;
        cameraTransform = cameraInstance.cameraObject.transform;
        
        positionalVelocity = (player.transform.position - OldVector)*10;
        onGroundSpeed = Mathf.Sqrt(positionalVelocity.x * positionalVelocity.x + positionalVelocity.z * positionalVelocity.z);
        OldVector = player.transform.position;
    }
    
    
    void LockOnParams()
    {
        movementAnimParams = Vector3.Lerp(movementAnimParams, Vector3.up*verticalInput + Vector3.right*horizontalInput, Time.deltaTime * 10f);
    }

    void FreeCamParams()
    {
        leanDirection = targetMoveDirection - moveDirection;
        leanDirection.Normalize();
            
        var dotProduct = Vector3.Dot(leanDirection, transform.right) / 2;
        leanAmount = Mathf.Lerp(leanAmount, dotProduct, 0.05f);
        movementAnimParams.x = leanAmount;
        movementAnimParams.y = moveAmount;
    }

    private void HandleMovement()
    {
        if (!player.canMove) 
            return;
        
        targetMoveDirection = cameraTransform.forward * verticalInput + cameraTransform.right * horizontalInput;
        targetMoveDirection.y = 0;
        targetMoveDirection.Normalize();
        
        speed = moveAmount > 0.5f ? runSpeed : walkSpeed;
        groundVelocity = Vector3.Lerp(groundVelocity, targetMoveDirection*speed, moveDirectionTimer*Time.deltaTime);
        player.controller.Move(Time.deltaTime * groundVelocity);
        setAnimParams?.Invoke();
        
        if (moveAmount < 0.1f)
            movementAnimParams = Vector3.zero;
        
        player.playerAnimationManager.UpdateAnimatorMovementParameters(movementAnimParams.x ,movementAnimParams.y);
    }

    private void FreeRotation()
    {
        targetRotateDirection = targetMoveDirection;
    }

    private void LockRotation()
    {
        targetRotateDirection = PlayerCamera.instance.lockOnTarget.transform.position - transform.position;
        targetRotateDirection.y = 0;
        targetRotateDirection.Normalize();
    }
    
    private void HandleRotation()
    {
        if (!player.canRotate) 
            return;
        
        setRotateDirection?.Invoke();

        if (targetRotateDirection == Vector3.zero)
            targetRotateDirection = transform.forward;
        
        var playerRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetRotateDirection), rotateMultiplier * Time.deltaTime);
        transform.rotation = playerRotation;
    }
    
    void UpdateMultipliers()
    {
        if (isGrounded)
        {
            if (isSliding)
            {
                moveDirectionTimer = 0.01f;
                runSpeed = 5f;
                rotateMultiplier = 1f;
            }
            else if (player.isSprinting)
            {
                moveDirectionTimer = 7f;
                runSpeed = 6f;
                rotateMultiplier = 10f;
            }
            else
            {
                gravityMultiplier = 0f;
                moveDirectionTimer = 7f;
                runSpeed = 4f;
                rotateMultiplier = 10f;
                onJump = delegate { GroundJump(); };
            }
        }
        else
        {
            moveDirectionTimer = 0.0005f;
            gravityMultiplier = -10f;
            onJump = delegate { WallRunJump(); };   
            
        }
    }
    
    private Vector3 WallPointGizmos;
    private Vector3 targetPositionGizmos;
    private void WallRun()
    {
        var hitLeft = Physics.OverlapSphere(wallCheckTransformLeft.position, 0.1f, LayerMask.GetMask("Wall"));
        var hitRight = Physics.OverlapSphere(wallCheckTransformRight.position, 0.1f, LayerMask.GetMask("Wall"));
        var isHit = false;

        Collider hit = null;
        Vector3 wallCheckPosition = Vector3.zero;

        if (hitLeft.Length > 0 && wallRunLeftAllowed) {
                hit = hitLeft[0];
                isHit = true;
                wallCheckPosition = wallCheckTransformLeft.position;
            
                onWallJump = delegate {
                    GroundVelocityOverride(Vector3.RotateTowards(player.transform.right, targetMoveDirection, 1.5f, 0f) * 6f);
                    wallRunRightAllowed = true; 
                    player.playerAnimationManager.PlayTargetActionAnimation("Wall Jump Left", true, 0.1f, false, true, true);
                };
                
                player.playerAnimationManager.PlayTargetActionAnimation("Wall Run Left", true, 0.4f);
        }
            
        else if (hitRight.Length > 0 && wallRunRightAllowed) {   
                hit = hitRight[0];
                isHit = true;
                wallCheckPosition = wallCheckTransformRight.position;
            
                onWallJump = delegate {
                    GroundVelocityOverride(Vector3.RotateTowards(-player.transform.right, targetMoveDirection, 1.5f, 0f) * 6f);
                    wallRunLeftAllowed = true; 
                    player.playerAnimationManager.PlayTargetActionAnimation("Wall Jump Right", true, 0.1f, false, true, true);
                };
                
                player.playerAnimationManager.PlayTargetActionAnimation("Wall Run Right", true, 0.4f);
        }

        if (isHit)
        {
            wallRunLeftAllowed = wallRunRightAllowed = false;
            
            targetMoveDirection = Vector3.Project(groundVelocity, hit.transform.forward);
            RotationOverride(targetMoveDirection);

          
            var tempDirection = hit.ClosestPoint(wallCheckPosition) - player.transform.position;
            tempDirection.y = 0;
            player.controller.Move(tempDirection*3f);
            
            WallPointGizmos = hit.ClosestPoint(wallCheckPosition);
            targetPositionGizmos = player.transform.position + tempDirection*3f;
        }
        
        isWallRunning = (!wallRunLeftAllowed && !wallRunRightAllowed);
    }

    private void HandleLand()
    {
        
        RotationOverride(groundVelocity);
        
        if (onGroundSpeed > 0.3f)
            player.playerAnimationManager.PlayTargetActionAnimation("Land Roll", true, 0.1f);
        else
            player.playerAnimationManager.PlayTargetActionAnimation("Landing", true, 0.1f);
    }
    
    
    
    private void HandleVerticalMovement()
    {
        
        var hit = Physics.OverlapSphere(groundCheckTransform.position, 0.2f, LayerMask.GetMask("Ground"));
        
        if(hit.Length > 0)
        {
            if (!isGrounded)
                HandleLand();

            isGrounded = wallRunLeftAllowed = wallRunRightAllowed = true;
            isFalling = isJumping = isWallRunning = false;
        }
        else
        {
            isGrounded = false;
            isFalling = true;
        }
        
        jumpAmount = Mathf.Lerp(jumpAmount, gravityMultiplier, Time.deltaTime * jumpMultiplier);
        airVelocity = Vector3.Lerp(airVelocity, Vector3.up * jumpAmount, Time.deltaTime * airMultiplier);
        player.controller.Move( airVelocity * Time.deltaTime);
        
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isGrounded",isGrounded);
        player.playerAnimationManager.UpdateAnimatorFloatParameters("groundedVelocity", onGroundSpeed);
        
    }

   
    
    public void HandleLockOn()
    {
        if (PlayerCamera.instance.isLockedOn)
        {
            setAnimParams = delegate { FreeCamParams(); };
            setRotateDirection = delegate { FreeRotation(); };
            
            PlayerCamera.instance.isLockedOn = false;
            PlayerCamera.instance.lockOnTarget = null;
        }
        else
        {
            setAnimParams = delegate { LockOnParams(); };
            setRotateDirection = delegate { LockRotation(); };
            
            PlayerCamera.instance.isLockedOn = true;
            PlayerCamera.instance.lockOnTarget = player.playerCombatManager.GetClosestTarget();
            
        }
    }
    
    public void PerformBackstep()
    {
        if (player.isPerformingAction)
            return;
        
        if(player.playerStatManager.canBackstep)
        {
            player.playerAnimationManager.PlayTargetActionAnimation("Backstep", true);
            player.playerStatManager.UpdateStamina(-20f,1);
        }
    }

    public void PerformDodge()
    {
        if (player.isPerformingAction)
            return;
    
        if (player.playerStatManager.canDodge)
        {
            player.transform.rotation = Quaternion.LookRotation(groundVelocity);
            player.playerAnimationManager.PlayTargetActionAnimation("Dodge", true, 0.1f, false);
            player.playerStatManager.UpdateStamina(-30f, 1);
            StartCoroutine(DodgeCoroutine(groundVelocity.normalized, 0.7f));
        }
    }

    private IEnumerator DodgeCoroutine(Vector3 targetDir, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            player.controller.Move(dodgeCurve.Evaluate(timer) *  dodgeRange * targetDir * Time.deltaTime );
            yield return null;
        }
    }
    
    public void HandleJump()
    {
        onJump.Invoke();
    }

    public void WallRunJump()
    {
        if (isWallRunning)
        {
            jumpAmount = 8f;
            onWallJump.Invoke();
        }
       
    }

    public void GroundJump()
    {
        if(player.isPerformingAction)
            return;
        
        isJumping = true;
        player.playerAnimationManager.PlayTargetActionAnimation("SlideJump", true, 0.05f, false, true, true);
    }

    
    public void StartSprint()
    {
        if (player.isPerformingAction) return;

        if (player.playerStatManager.canSprint)
        {
            player.isSprinting = true;
            player.playerAnimationManager.UpdateAnimatorBoolParameters("isSprinting", true);
        }
    }

    public void EndSprint()
    {
        player.isSprinting = false;
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isSprinting", false);
    }
    
    public void StartSlide()
    {
        if (player.isPerformingAction)
            return;
        
        isSliding = true;
        RotationOverride(targetMoveDirection);
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isSliding", true);
        player.playerAnimationManager.PlayTargetActionAnimation("Slide Start", true, 0.1f, false, true, true);
    }
    
    public void StopSlide()
    {
        isSliding = false;
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isSliding", false);
    }
    
    private void RotationOverride(Vector3 direction)
    {
        player.transform.rotation = Quaternion.LookRotation(direction);
    }

    private void GroundVelocityOverride(Vector3 velocity)
    {
        groundVelocity = velocity;
        groundVelocity.y = 0f;

    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckTransform.position, 0.2f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallCheckTransformLeft.position, 0.1f);
        Gizmos.DrawWireSphere(wallCheckTransformRight.position, 0.1f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(WallPointGizmos, 0.1f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPositionGizmos, 0.1f);
    }
    

    public void UpdateJumpAmount()
    {
        jumpAmount = jumpHeight;
    }

    public void KeepVelocity()
    {
        print("KEPT VELOCITY");
        groundVelocity = airVelocity = player.animator.velocity;

        groundVelocity.y = 0f;
        airVelocity.x = airVelocity.z = 0f;
    }

    
}