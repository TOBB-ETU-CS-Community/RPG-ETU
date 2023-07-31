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
            player.controller.Move(velocity);
            player.transform.rotation *= player.animator.deltaRotation;
        }
    }

    public void UpdateAnimatorTurnAngle(float normalizedTurnAngle)
    {
        player.animator.SetFloat("TurnAngle", normalizedTurnAngle);
    }
    
}