using UnityEngine;
using FishNet.Object;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    [Header("References")]
    public Camera playerCamera; // assign in prefab inspector
    public GameObject weaponRoot; // assign weapon root under camera

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;

    [HideInInspector]
    public bool canMove = true;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(base.IsOwner)
        {

            if (!base.IsOwner)
                return;

            characterController = GetComponent<CharacterController>();

            
            
            // Set up camera
            
            //playerCamera = GetComponentInChildren<Camera>(true);
            //playerCamera.gameObject.SetActive(true);

            playerCamera = GetComponentInChildren<Camera>(true);
            
            playerCamera.enabled = true;

            if (playerCamera == null)
                Debug.LogError("PlayerController: No Camera found in children!");

            playerCamera.GetComponent<AudioListener>().enabled = true;


            // Init weapons
            if (TryGetComponent(out PlayerWeaponManager playerWeapon))
            {
                playerWeapon.InitializeWeapons(playerCamera.transform);
            }
            else
            {
                Debug.LogWarning("PlayerWeapon script not assined to player.");
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }

        
        if (!base.IsOwner)
        {
            //if (playerCamera != null) playerCamera.enabled = false;
            //playerCamera.TryGetComponent(out AudioListener al)) al.enabled = false;
            //if (weaponRoot != null) weaponRoot.SetActive(false);
            //enabled = false;
            return;
        }


             
        
    }


    void Update()
    {
        if (!base.IsOwner) return; // Only local player processes input

        HandleMovement();
        HandleLook();
    }

    void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0f;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0f;

        float verticalVelocity = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (characterController.isGrounded && Input.GetButton("Jump") && canMove)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = verticalVelocity - gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleLook()
    {

        if (playerCamera == null) Debug.Log("Camera == null");
        if (!canMove || playerCamera == null) return;
            

        // Vertical rotation (X-axis)
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Horizontal rotation (Y-axis)
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        transform.Rotate(0f, mouseX, 0f);
    }
}
