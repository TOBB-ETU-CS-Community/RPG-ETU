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
    
    public Vector3 moveDirection;
    public Vector3 leanDirection;
    public Vector3 targetDirection;

    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 4f;
    private Transform cameraTransform;

    private PlayerManager player;
    private Rig[] rigs;

    
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
        rigs = GetComponentsInChildren<Rig>();
    }

    public void HandleAllMovement()
    {
        cameraTransform = PlayerCamera.instance.cameraObject.transform;
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
        

        vertical = inputInstance.verticalInput;
        horizontal = inputInstance.horizontalInput;
        moveAmount = inputInstance.moveAmount;
        targetDirection = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
    }

    private void HandleMovement()
    {
        if (!player.canMove) return;

        GetMovementValues();

        var targetMoveDirection = targetDirection;
        targetMoveDirection.y = 0;
        targetMoveDirection.Normalize();

        moveDirection = Vector3.Lerp(moveDirection, targetMoveDirection, 0.5f);

        var speed = moveAmount > 0.5f ? runSpeed : walkSpeed;

        leanDirection = targetMoveDirection - moveDirection;
        leanDirection.Normalize();

        var dotProduct = Vector3.Dot(leanDirection, transform.right) / 2;
        leanAmount = Mathf.Lerp(leanAmount, dotProduct, 0.05f);
        player.controller.Move(speed * Time.deltaTime * moveDirection);
        player.playerAnimationManager.UpdateAnimatorMovementParameters(leanAmount, moveAmount);
    }

    private void HandleRotation()
    {
        if (!player.canRotate) return;
        
        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        var playerRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), 10 * Time.deltaTime);

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
        if (player.isPerformingAction) 
            return;

        var lockOnInput = PlayerInputManager.instance.lockOnInput;

        if (lockOnInput)
        {
            if (PlayerCamera.instance.isLockedOn)
                PlayerCamera.instance.isLockedOn = false;
            //PlayerCamera.instance.lockOnTarget = null;
            else
                PlayerCamera.instance.isLockedOn = true;
            //PlayerCamera.instance.lockOnTarget = EnemyManager.instance.GetClosestEnemy();
        }
    }

    public void PerformDodge()
    {
        if (player.isPerformingAction)
            return;
        
        if (player.playerStatManager.canDodge)
        {
            var rollDirection = targetDirection;
            rollDirection.y = 0;
            rollDirection.Normalize();

            player.transform.rotation = Quaternion.LookRotation(rollDirection);
            player.playerAnimationManager.PlayTargetActionAnimation("Dodge", true, 0.1f, false);
            player.playerStatManager.UpdateStamina(-30f, 1);
            
            StartCoroutine(MoveCharacterSmoothly(rollDirection * 4, 0.7f));
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

  
}