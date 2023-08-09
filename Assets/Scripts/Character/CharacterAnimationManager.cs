using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    private readonly int hHash = Animator.StringToHash("Horizontal");
    private readonly int vHash = Animator.StringToHash("Vertical");
    private CharacterManager character;


    protected void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void UpdateAnimatorMovementParameters(float h, float v)
    {
        character.animator.SetFloat(hHash, h);
        character.animator.SetFloat(vHash, v);
    }

    public virtual void PlayTargetActionAnimation(string targetAnim, bool isPerformingAction, float transitionDuration = 0.2f, bool applyRoot = true,
        bool canRotate = false, bool canMove = false)
    {
        character.isPerformingAction = isPerformingAction;
        character.applyRootMotion = applyRoot;
        character.canRotate = canRotate;
        character.canMove = canMove;
        
        character.animator.CrossFade(targetAnim, transitionDuration);
    }
    
    public void UpdateAnimatorBoolParameters(string parameterName, bool value)
    {
        character.animator.SetBool(parameterName, value);
    }
    
    public void UpdateAnimatorTriggerParameters(string parameterName)
    {
        character.animator.SetTrigger(parameterName);
    }
    
}