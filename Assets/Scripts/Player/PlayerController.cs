using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
  
    [Header("Movement Settings")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 90.0f;

    [Header("References")]
    public Camera playerCamera; // assign in prefab inspector
    public GameObject weaponRoot; // assign weapon root under camera

    public CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField] private int playerSelfLayer = 7;


    public override void OnStartServer()
    {
        base.OnStartServer();

        // Register player in player manager
        PlayerManager.Instance?.RegisterPlayer(OwnerId, NetworkObject);
    }


    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        // TODO quitar este if
        if (IsOwner)
        {        
            characterController = GetComponent<CharacterController>();

            // Init camera
            playerCamera = GetComponentInChildren<Camera>(true);       
            playerCamera.enabled = true;
            if (playerCamera == null)
                Debug.LogError("PlayerController: No Camera found in children!");
            playerCamera.GetComponent<AudioListener>().enabled = true;

            // Set up layers
            gameObject.layer = playerSelfLayer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = playerSelfLayer;
            }

            // Set up cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return; // Only local player processes input

        HandleMovement();
        HandleLook();
    }


    void HandleMovement()
    {
        if (!canMove) return;

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

    public void TogglePlayer(bool state)
    {

        Debug.Log("[PlayerController.TogglePlayer] Toggling player: " + state);

        canMove = state;

        // 1. Character controller
        if (characterController != null)
            characterController.enabled = state;

        // 2. Disable all colliders
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = state;

        // 3. Disable all renderers
        foreach (var rend in GetComponentsInChildren<Renderer>())
            rend.enabled = state;

        // 4. Disable all gameplay scripts except networking
        foreach (var mb in GetComponentsInChildren<MonoBehaviour>())
        {
            if (mb == null) continue;
            if (mb == this) continue;
            if (mb is NetworkBehaviour) continue;

            mb.enabled = state;
        }

        // 5. Update cursor
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = state;
        
    }

    [ObserversRpc]
    public void ObserversTogglePlayer(bool state)
    {
        TogglePlayer(state);
    }

    public void SetPlayerPsition()
    {

    }

    [ServerRpc]
    private void ServerSetPlayerPosition()
    {

    }

    // Called by the server
    [TargetRpc]
    public void TargetSetPlayerPosition(NetworkConnection netConnection, Vector3 newPosition, Quaternion rotation)
    {
        // The client sets its own position
        transform.position = newPosition;
        transform.rotation = rotation;

        // Optional: reset velocity, re-enable movement, etc.
    }


    public override void OnStopServer()
    {
        base.OnStopClient();

        PlayerManager.Instance?.UnregisterPlayer(OwnerId);
    }
}


