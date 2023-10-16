using UnityEngine;

public class PlayerAnimationManager : CharacterAnimationManager
{
    private PlayerManager player;

    protected void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    private void OnAnimatorMove()
    {
        if (player.applyRootMotion)
        {
            var velocity = player.animator.deltaPosition;
            if (player.isHitStunned)
            {
                velocity *= 0.5f;
            }
               
            player.controller.Move(velocity);
            player.transform.rotation *= player.animator.deltaRotation;
        }
    }

    public void UpdateAnimatorTurnAngle(float normalizedTurnAngle)
    {
        player.animator.SetFloat("TurnAngle", normalizedTurnAngle);
    }

    public void UpdateAnimatorFloatParameters(string s, float f)
    {
        player.animator.SetFloat(s, f, 0.1f, Time.deltaTime);
    }
}