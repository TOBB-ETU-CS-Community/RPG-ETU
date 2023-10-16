using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;
    
    [SerializeField] protected float onGroundSpeed;
    [SerializeField] protected Vector3 positionalVelocity;

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