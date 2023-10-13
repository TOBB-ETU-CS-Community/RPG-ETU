using UnityEngine;
public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public PlayerManager player;
    public Camera cameraObject;
    [SerializeField] private float leftAndRightRotationSpeed = 2000;
    [SerializeField] private float upAndDownRotationSpeed = 2000;
    [SerializeField] private float minPivot = -30;
    [SerializeField] private float maxPivot = 60;
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private LayerMask collideWithLayers;

    public bool isLockedOn;

    [Header("Camera Settings")]
    [SerializeField] private float cameraSmoothSpeed = 0.2f;
    private Vector3 cameraObjectPosition;

    [Header("Camera Values")] 
    [SerializeField] private Vector3 cameraVelocity;
    private float cameraZPosition;
    private float targetCameraZPosition;
    
    [Header("Lock On Settings")]
    [SerializeField] public float lockOnSmoothSpeed = 7.5f;
    [SerializeField] public GameObject lockOnTarget;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        if (isLockedOn)
        {
            HandleLockOnCameraMovement();
        }
            
        else
        {
            FollowTarget();
            HandleRotations();
        }
        
        HandleCollisions();
        
    }

    private void FollowTarget()
    {
        var targetPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity,
            cameraSmoothSpeed);
        transform.position = targetPosition;
    }


    private void HandleRotations()
    {
        if (isLockedOn)
            return;

        var horizontal = PlayerInputManager.instance.cameraHorizontalInput;
        var vertical = PlayerInputManager.instance.cameraVerticalInput;

        var targetRotation = transform.rotation.eulerAngles;
        targetRotation.x -= vertical * upAndDownRotationSpeed * Time.deltaTime;
        targetRotation.y += horizontal * leftAndRightRotationSpeed * Time.deltaTime;
        targetRotation.z = 0;

        if (targetRotation.x > 180) targetRotation.x -= 360;

        targetRotation.x = Mathf.Clamp(targetRotation.x, minPivot, maxPivot);

        var targetRotationQuaternion = Quaternion.Euler(targetRotation);
        transform.rotation = targetRotationQuaternion;
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        cameraObjectPosition = cameraObject.transform.localPosition;

        RaycastHit hit;
        var direction = transform.position - cameraObject.transform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraObject.transform.position, cameraCollisionRadius, direction, out hit,
                Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            var distance = Vector3.Distance(cameraObject.transform.position, hit.point);
            targetCameraZPosition = -Mathf.Clamp(distance - cameraCollisionRadius, 0, Mathf.Abs(targetCameraZPosition));
        }

        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius) targetCameraZPosition = -cameraCollisionRadius;

        cameraObjectPosition.z =
            Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 5 * Time.deltaTime);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
    

    public void HandleLockOnCameraMovement()
    {
        FollowTarget();
        
        var lockOnTransform = lockOnTarget.transform;
        var cameraTargetPosition = lockOnTransform.position + lockOnTarget.transform.right * 0.5f + lockOnTransform.up * 1.5f;

        var cameraTargetDirection = cameraTargetPosition - transform.position ;
        cameraTargetDirection.Normalize();
        cameraTargetDirection.y = 0;

        var cameraTargetRotation = Quaternion.LookRotation(cameraTargetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraTargetRotation, lockOnSmoothSpeed * Time.deltaTime);




    }
}