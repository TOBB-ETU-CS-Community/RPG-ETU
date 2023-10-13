using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    public float vertical;
    public float horizontal;
    public float moveAmount;
    public float leanAmount;
    public float jumpHeight = 0f;
    public float jumpAmount;
    public float dodgeAmount = 4f;
    public float rotateAmount = 10f;
    public float speedChangeMultiplier = 1f;
    public bool isLockedOn;
    public bool isSliding;

    public float speed;
    public float moveDirectionTimer = 0.5f;

    public bool isJumping;
    public bool isFalling;
    public bool isGrounded;
    
    public Vector3 moveDirection;
    public Vector3 leanDirection;
    public Vector3 targetDirection;

    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 4f;
    private Transform cameraTransform;

    private PlayerManager player;
    private Rig[] rigs;

    private Vector3 moveLockOn;
    public Transform groundCheckTransform;
    public Transform wallCheckTransformLeft;
    public Transform wallCheckTransformRight;

    public Vector3 OldVector;
    public Vector3 realVelocity;
    
    bool wallRunLeftAllowed = true;
    bool wallRunRightAllowed = true;
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
        rigs = GetComponentsInChildren<Rig>();
        OldVector = player.transform.position;
        
    }

    public void HandleAllMovement()
    {
        
        
        positionalVelocity = (OldVector - player.transform.position)*10;
        groundedVelocity = Mathf.Sqrt(positionalVelocity.x * positionalVelocity.x + positionalVelocity.z * positionalVelocity.z);
        OldVector = player.transform.position;
        
        GetMovementValues();
        HandleMovement();
        HandleRotation();
        HandleVerticalMovement();

        if (player.isSprinting)
        {
            player.playerStatManager.staminaRegenValue = -30;
        }
        else
        {
            player.playerStatManager.staminaRegenValue = 15;
        }
        
        if(!isGrounded)
            WallRun();
        
      
    }

    private void GetMovementValues()
    {
        var inputInstance = PlayerInputManager.instance;
        var cameraInstance = PlayerCamera.instance;
        
        
        vertical = inputInstance.verticalInput;
        horizontal = inputInstance.horizontalInput;
        moveAmount = inputInstance.moveAmount;
        
        isLockedOn = cameraInstance.isLockedOn;
        cameraTransform = cameraInstance.cameraObject.transform;
    }

    private void HandleMovement()
    {
        if (!player.canMove) 
            return;
        
        targetDirection = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
        targetDirection.y = 0;
        targetDirection.Normalize();
        
        speed = moveAmount > 0.5f ? runSpeed : walkSpeed;
        realVelocity = Vector3.Lerp(realVelocity, targetDirection*speed, moveDirectionTimer);
        player.controller.Move(Time.deltaTime * realVelocity);

        if(isLockedOn || isSliding)
        {
            moveLockOn = Vector3.Lerp(moveLockOn, Vector3.up*vertical + Vector3.right*horizontal, 0.05f);
            if (moveAmount < 0.1f)
                moveLockOn = Vector3.zero;
            player.playerAnimationManager.UpdateAnimatorMovementParameters(moveLockOn.x, moveLockOn.y);
        }
        else
        {
            leanDirection = targetDirection - moveDirection;
            leanDirection.Normalize();
            
            var dotProduct = Vector3.Dot(leanDirection, transform.right) / 2;
            
            if (moveAmount < 0.1f)
                leanAmount = 0;
            else
                leanAmount = Mathf.Lerp(leanAmount, dotProduct, 0.05f);
            
            player.playerAnimationManager.UpdateAnimatorMovementParameters(leanAmount, moveAmount);
        }

    }

    private void WallRun()
    {
        var hitLeft = Physics.OverlapSphere(wallCheckTransformLeft.position, 0.1f, LayerMask.GetMask("Wall"));
        var hitRight = Physics.OverlapSphere(wallCheckTransformRight.position, 0.1f, LayerMask.GetMask("Wall"));
        
        if (hitLeft.Length > 0 && wallRunLeftAllowed)
        {
            
            player.playerAnimationManager.PlayTargetActionAnimation("Wall Run", true, 0.1f);
            var wallNormal = hitLeft[0].transform.forward;
            wallNormal.y = 0;
            wallNormal.Normalize();
            var targetRotation = Quaternion.LookRotation(wallNormal);
            player.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.5f);

            var targetPosition = hitLeft[0].transform.position + wallNormal;
            targetPosition.y = player.transform.position.y; 
            player.transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);
            wallRunLeftAllowed = false;
            wallRunRightAllowed = true;

        }
        else if (hitRight.Length > 0)
        {
            player.playerAnimationManager.PlayTargetActionAnimation("Wall Run", true, 0.001f);
            wallRunRightAllowed = false;
            wallRunLeftAllowed = true;
            
        }
    }
    private void HandleVerticalMovement()
    {
        var hit = Physics.OverlapSphere(groundCheckTransform.position, 0.2f, LayerMask.GetMask("Ground"));
        
        if(hit.Length > 0)
        {
            if (!isGrounded){
                
                if(groundedVelocity > 0.3f)
                    player.playerAnimationManager.PlayTargetActionAnimation("Land Roll", true, 0.1f);
                else
                    player.playerAnimationManager.PlayTargetActionAnimation("Landing", true, 0.1f);
            }
                
            isGrounded = true;
            isFalling = false;
            isJumping = false;
            wallRunLeftAllowed = true;
            wallRunRightAllowed = true;
            if(!isSliding)
                moveDirectionTimer = 0.5f;
        }
        else
        {
            isGrounded = false;
            isFalling = true;
            moveDirectionTimer = 0.0005f;
                
        }
        
        jumpAmount = Mathf.Lerp(jumpAmount, 0, 0.01f);
        player.controller.Move(Time.deltaTime * (jumpAmount - 10f) * Vector3.up);
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isGrounded",isGrounded);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckTransform.position, 0.2f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallCheckTransformLeft.position, 0.1f);
        Gizmos.DrawWireSphere(wallCheckTransformRight.position, 0.1f);
    }


    private void HandleRotation()
    {
        if (!player.canRotate) return;
        
        if (PlayerCamera.instance.isLockedOn && !isSliding && !player.isSprinting)
        {
            targetDirection = PlayerCamera.instance.lockOnTarget.transform.position - transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
        }

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        if (isSliding || !isGrounded)
        {
            rotateAmount = Mathf.Lerp(rotateAmount, 1 / runSpeed, 0.1f);
        }
        else
        {
            rotateAmount = 10f;
        }
        
        var playerRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), rotateAmount * Time.deltaTime);
        
        playerRotation.x = playerRotation.z = 0;
        transform.rotation = playerRotation;
        
       
    }

    public void UpdateJumpAmount()
    {
        print("Jump updated");
        jumpAmount = 15f;
    }

    public void KeepVelocity()
    {
        realVelocity = player.animator.velocity;
        
        
    }
    

    public void HandleSprint(bool isSprinting)
    {
        if (player.isPerformingAction) return;

        if (player.playerStatManager.canSprint)
        {
            runSpeed = isSprinting ? Mathf.Lerp(runSpeed, 6f, 0.1f) : Mathf.Lerp(runSpeed, 4f, 0.2f);
            player.playerAnimationManager.UpdateAnimatorBoolParameters("isSprinting", isSprinting);
            player.isSprinting = isSprinting;
        }
        else
        {
            player.isSprinting = false;
            runSpeed = Mathf.Lerp(runSpeed, 4f, 0.2f);
            player.playerAnimationManager.UpdateAnimatorBoolParameters("isSprinting", false);
        }
    }

    public void HandleLockOn()
    {
        
        if (PlayerCamera.instance.isLockedOn)
        {
            PlayerCamera.instance.isLockedOn = false;
            PlayerCamera.instance.lockOnTarget = null;
        }
        else
        {
            PlayerCamera.instance.isLockedOn = true;
            PlayerCamera.instance.lockOnTarget = player.playerCombatManager.GetClosestTarget();
        }
    }

    public void PerformDodge()
    {
        if (player.isPerformingAction)
            return;
    
        if (player.playerStatManager.canDodge)
        {
            targetDirection = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
            Vector3 dodgePosition = PlayerCamera.instance.lockOnTarget.transform.position + targetDirection * dodgeAmount;
            Vector3 allowedDodgeDirection = dodgePosition - transform.position;
            allowedDodgeDirection.y = 0;
            allowedDodgeDirection.Normalize();
    
            player.transform.rotation = Quaternion.LookRotation(allowedDodgeDirection);
    
            player.playerAnimationManager.PlayTargetActionAnimation("Dodge", true, 0.1f, false);
    
            player.playerStatManager.UpdateStamina(-30f, 1);
            StartCoroutine(MoveCharacterSmoothly(allowedDodgeDirection * dodgeAmount, 0.7f));
        }
    }


    
    private IEnumerator MoveCharacterSmoothly(Vector3 direction, float duration = 1f)
    {
        var elapsedTime = 0f;
        var startPosition = player.transform.position;

        while (elapsedTime < duration)
        {
            var t = elapsedTime / duration;
            var newPosition = Vector3.Lerp(startPosition, startPosition + direction, t);

            player.controller.Move(newPosition - player.transform.position);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.controller.Move(direction * Time.deltaTime);
        yield return null;
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

    
    public void HandleJump()
    {

        if (player.playerStatManager.canJump)
        {
            isJumping = true;
            if(isSliding)
                player.playerAnimationManager.PlayTargetActionAnimation("SlideJump", true, 0.05f, false, true, true);
        }
    }
    public void StartSlide()
    {
        if (player.isPerformingAction)
            return;
        
        isSliding = true;
        speedChangeMultiplier = 0.4f;
        moveDirectionTimer = 0.005f;
        runSpeed = runSpeed + 1f;
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isSliding", true);
        player.playerAnimationManager.PlayTargetActionAnimation("Slide Start", true, 0.1f, false, true, true);
        print("im here start slide");
        
        
    }

    public void StopSlide()
    {
        isSliding = false;
        speedChangeMultiplier = 1f;
        moveDirectionTimer = 0.5f;
        runSpeed = runSpeed - 1f;
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isSliding", false);
        print("im here end slide");
        
    }
    
}