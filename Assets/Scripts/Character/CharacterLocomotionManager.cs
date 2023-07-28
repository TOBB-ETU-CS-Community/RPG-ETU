using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;
    
    [SerializeField] protected Vector3 yVelocity;
    [SerializeField] protected Vector3 groundedVelocity;
    [SerializeField] protected Vector3 fallingVelocity;

    protected virtual void Awake()
    {
    }

    protected virtual void Update()
    {
    }

    protected void HandleGroundCheck()
    {
        //character.isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer, QueryTriggerInteraction.Ignore);
    }
}