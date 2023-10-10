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
    public float dodgeAmount = 4f;
    public float rotateAmount = 10f;
    public float speedChangeMultiplier = 1f;
    public bool isLockedOn;
    public bool isSliding;

    public float speed;
    public float moveDirectionTimer = 0.5f;
    
    
    
    public Vector3 moveDirection;
    public Vector3 leanDirection;
    public Vector3 targetDirection;

    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 4f;
    private Transform cameraTransform;

    private PlayerManager player;
    private Rig[] rigs;

    private Vector3 moveLockOn;

    
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
        rigs = GetComponentsInChildren<Rig>();
    }

    public void HandleAllMovement()
    {
        GetMovementValues();
        HandleMovement();
        HandleRotation();

        if (player.isSprinting)
        {
            player.playerStatManager.staminaRegenValue = -30;
        }
        else
        {
            player.playerStatManager.staminaRegenValue = 15;
        }
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
        
        moveDirection = Vector3.Lerp(moveDirection, targetDirection, moveDirectionTimer);
        speed = moveAmount > 0.5f ? runSpeed : walkSpeed;
        player.controller.Move(speed * Time.deltaTime * moveDirection);
        
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

        if (isSliding)
        {
            rotateAmount = Mathf.Lerp(rotateAmount, 1 / runSpeed, 0.1f);
        }
        
        var playerRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), rotateAmount * Time.deltaTime);
        
        playerRotation.x = playerRotation.z = 0;
        transform.rotation = playerRotation;
        
       
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
        if (player.isPerformingAction)
            return;

        if (player.playerStatManager.canJump)
        {
            player.playerAnimationManager.PlayTargetActionAnimation("Jump", true);
        }
    }

    public void StartSlide()
    {
        
        if (player.isPerformingAction)
            return;
        
        isSliding = true;
        speedChangeMultiplier = 0.4f;
        moveDirectionTimer = 0.0001f;
        runSpeed = runSpeed + 1f;
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isSliding", true);
        player.playerAnimationManager.PlayTargetActionAnimation("Slide Start", true, 0.1f, false, true, true);
        print("im here start slide");
        
        
    }

    public void StopSlide()
    {
        isSliding = false;
        rotateAmount = 10f;
        speedChangeMultiplier = 1f;
        moveDirectionTimer = 0.5f;
        runSpeed = runSpeed - 1f;
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isSliding", false);
        print("im here end slide");
        
    }
    
}