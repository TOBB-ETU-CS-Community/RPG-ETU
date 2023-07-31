using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Animator animator;

    [Header("Flags")] 
    public bool isPerformingAction;
    public bool applyRootMotion;
    public bool canRotate = true;
    public bool canMove = true;
    public bool isGrounded = true;
    public bool isJumping = false;
    public bool isFalling = false;
    public bool isSprinting = false;
    public bool isLightAttacking = false;


    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
    }

    protected virtual void LateUpdate()
    {
    }

    public virtual void ResetActionFlags()
    {
        isPerformingAction = false;
        applyRootMotion = false;
        canRotate = true;
        canMove = true;

    }
}