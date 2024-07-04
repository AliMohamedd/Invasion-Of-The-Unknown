using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.Image;

namespace IOTU
{
    /// <summary>
    /// player controllers, Handle character functionalities and gameplay actions
    /// player can move, jump, crouch, lean, etc...
    /// also controlling invetory screen and pause screen
    /// </summary>
    public class playerController : MonoBehaviour
    {
        public bool CanMove { get; private set; } = true; // Boolean property indicating whether the player can move
        private bool isSprinting => canSprint && Input.GetKey(sprintKey); // Computed property determining if the player is sprinting based on input and settings
        private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded; // Computed property determining if the player should jump based on input and current state
        private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && characterController.isGrounded && !duringCrouchingAnimation; // Computed property determining if the player should crouch based on input and current state
        private bool isPaused = false; // Boolean indicating if the game is currently paused
        private bool pauseMouse; // Boolean controlling mouse/camera input pause state    
        private bool inventoryShown = false; // Boolean indicating if the inventory UI is currently shown
        private bool stop = false; // Boolean indicating if the player's movement should stop due to a specific game event

        // Movement and interaction options
        [Header("Functional Options")]
        [SerializeField] private bool canSprint = true;
        [SerializeField] private bool canJump = true;
        [SerializeField] private bool canCrouch = true;
        [SerializeField] private bool canUseHeadbob = true;
        [SerializeField] private bool canZoom = true;
        [SerializeField] private bool canInteract = true;
        [SerializeField] private bool canLean = true;
        [SerializeField] private bool inventoryOption = true;

        // Key bindings for various actions
        [Header("Control")]
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
        [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode interactKey = KeyCode.F;
        [SerializeField] private KeyCode leanRightKey = KeyCode.E;
        [SerializeField] private KeyCode leanLeftKey = KeyCode.Q;
        [SerializeField] private KeyCode inventoryWindowKey = KeyCode.I;
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

        // Movement parameters
        [Header("Movement Parameters")]
        [SerializeField] private float walkSpeed = 3.0f;
        [SerializeField] private float sprintSpeed = 7.0f;
        [SerializeField] private float crouchSpeed = 1.5f;
        private Vector2 currentInput;
        private Vector3 moveDirection;

        // Look parameters
        [Header("Look Parameters")]
        [SerializeField, Range(1, 10)] private float lookSpeed = 2.0f;
        [SerializeField, Range(1, 90)] private float camLimit = 90;
        private float rotationX = 0;
        private float rotationY = 0;

        // Jumping parameters
        [Header("Jumping Parameters")]
        [SerializeField] private float jumpForce = 8.0f;
        [SerializeField] private float gravity = 30.0f;

        // Crouching parameters
        [Header("Crouching Parameters")]
        [SerializeField] private float crouchHeight = 0.5f;
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float timeToCrouch = 0.25f;
        [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
        [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
        private bool isCrouching;
        private bool duringCrouchingAnimation;

        // Headbob parameters
        [Header("Headbob Parameters")]
        [SerializeField] private float walkBobSpeed = 14f;
        [SerializeField] private float walkBobAmount = 0.05f;
        [SerializeField] private float sprintBobSpeed = 18f;
        [SerializeField] private float sprintBobAmount = 0.11f;
        [SerializeField] private float crouchBobSpeed = 8f;
        [SerializeField] private float crouchBobAmount = 0.025f;
        private float defaultYPos = 0;
        private float timer;

        // Zoom parameters
        [Header("Zoom Parameters")]
        [SerializeField] private float timeToZoom = 0.3f;
        [SerializeField] private float zoomFOV = 30f;
        private float defaultFOV;
        private Coroutine zoomRoutine;

        // Interaction parameters
        [Header("Interaction")]
        [SerializeField] private Vector3 interactionRayPoint = default;
        [SerializeField] private float interactionDistance = default;
        [SerializeField] private LayerMask interactionLayer = default;
        private Interactable currentInteractable;
        // Lean parameters
        [Header("Lean Parameters")]
        [SerializeField] private LayerMask leanLayer;
        [SerializeField] private Animator camLean;

        
        private Camera playerCamera;
        private CharacterController characterController;

        public static playerController instance;

        // Awake is called when the script instance is being loaded
        void Awake()
        {
            instance = this;
            // Set properties of camera and charactercontroller
            playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            characterController = GetComponent<CharacterController>();
            defaultYPos = playerCamera.transform.localPosition.y;
            defaultFOV = playerCamera.fieldOfView;

            // Lock the cursor to the center of the screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void OnEnable()
        {
            // Subscribe to game events
            GameEvents.noteShown += GameEvents_NoteShown;
            GameEvents.GameMonesterFoundMe += GameEvents_GameMonesterFoundMe;
        }

        void OnDisable()
        {
            // Unsubscribe from game events
            GameEvents.noteShown -= GameEvents_NoteShown;
            GameEvents.GameMonesterFoundMe -= GameEvents_GameMonesterFoundMe;
        }

        void Update()
        {
            // Raycast to interact with diedbody
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward * 20, out RaycastHit hitinfo, 20))
            {
                if (hitinfo.collider.name == "Ghost Remover Collider 1" || hitinfo.collider.name == "Ghost Remover Collider 2")
                {
                    hitinfo.collider.gameObject.SetActive(false);
                }
            }

            // Stop player movement if certain conditions are met
            if (stop) enabled = false;

            // Handle player controls and actions based on game state
            if (!isPaused)
            {
                if (CanMove)
                {
                    HandleMovementInput();
                    HandleMouseLook();

                    if (canJump)
                        HandleJump();

                    if (canCrouch)
                        HandleCrouch();

                    if (canUseHeadbob)
                        HandheHeadBob();

                    if (canZoom)
                        HandleZoom();

                    if (canInteract)
                    {
                        HandleInteractionCheck();
                        HandleInteractionInput();
                    }

                    if (canLean)
                    {
                        HandleLean();
                    }

                    if (inventoryOption)
                    {
                        HandleShowInventory();
                    }

                    HandlePauseGame();
                    ApplyFinalMovement();
                }
            }
            else
            {
                // Handle inventory and pause menu visibility
                if (inventoryOption && inventoryShown)
                {
                    HandleShowInventory();
                }

                if (!inventoryShown)
                {
                    HandlePauseGame();
                }
            }
        }

        // Handle player movement based on input
        private void HandleMovementInput()
        {
            currentInput = new Vector2((isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"),
                                       (isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

            float moveDirectionY = moveDirection.y;
            moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) +
                            (transform.TransformDirection(Vector3.right) * currentInput.y);
            moveDirection.y = moveDirectionY;
        }

        // Handle mouse look (camera rotation) based on input
        private void HandleMouseLook()
        {
            if (pauseMouse) ;
            else
            {
                rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
                rotationY = Input.GetAxis("Mouse X") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -camLimit, camLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, rotationY, 0);
            }
        }

        // Handle player jump action based on input
        private void HandleJump()
        {
            if (ShouldJump && !isCrouching)
                moveDirection.y = jumpForce;
            else if (ShouldJump && isCrouching)
            {
                StartCoroutine(CrouchStand());
                moveDirection.y = 1.4f * jumpForce;
            }
        }

        // Handle player crouch action based on input
        private void HandleCrouch()
        {
            if (ShouldCrouch)
                StartCoroutine(CrouchStand());
        }

        // Handle headbob effect based on player movement
        private void HandheHeadBob()
        {
            if (!characterController.isGrounded) return;

            if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
            {
                timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount),
                    playerCamera.transform.localPosition.z);
            }
        }

        // Handle zoom action based on input
        private void HandleZoom()
        {
            if (Input.GetKeyDown(zoomKey))
            {
                if (zoomRoutine != null)
                {
                    StopCoroutine(zoomRoutine);
                    zoomRoutine = null;
                }

                zoomRoutine = StartCoroutine(ToggleZoom(true));
            }

            if (Input.GetKeyUp(zoomKey))
            {
                if (zoomRoutine != null)
                {
                    StopCoroutine(zoomRoutine);
                    zoomRoutine = null;
                }

                zoomRoutine = StartCoroutine(ToggleZoom(false));
            }
        }

        // Check for interactable objects within range
        private void HandleInteractionCheck()
        {
            if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
            {
                if (hit.collider.gameObject.layer == 9 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
                {
                    hit.collider.TryGetComponent(out currentInteractable);

                    if (currentInteractable)
                        currentInteractable.OnFocus();
                }
            }
            else if (currentInteractable)
            {
                currentInteractable.onLoseFocus();
                currentInteractable = null;
            }
        }

        // Handle player interaction with objects
        private void HandleInteractionInput()
        {
            if (Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
            {
                currentInteractable.OnInteract();
            }
        }

        // Handle player leaning action based on input
        private void HandleLean()
        {
            if (Input.GetKey(leanRightKey) && !Physics.Raycast(transform.position, transform.right, out RaycastHit hit, 1f, leanLayer))
            {
                camLean.ResetTrigger("idle");
                camLean.ResetTrigger("left");
                camLean.SetTrigger("right");
            }
            else if (Input.GetKey(leanLeftKey) && !Physics.Raycast(transform.position, -transform.right, out hit, 1f, leanLayer))
            {
                camLean.ResetTrigger("idle");
                camLean.ResetTrigger("right");
                camLean.SetTrigger("left");
            }
            else
            {
                camLean.ResetTrigger("right");
                camLean.ResetTrigger("left");
                camLean.SetTrigger("idle");
            }
        }

        // Handle showing and hiding inventory based on input
        private void HandleShowInventory()
        {
            bool once = false;
            if (Input.GetKeyDown(inventoryWindowKey))
            {
                isPaused = !isPaused;
                inventoryShown = !inventoryShown;
                Time.timeScale = isPaused ? 0 : 1;
                once = true;

                UIEvents.GamePlayScreenShown?.Invoke();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (isPaused && once)
            {
                UIEvents.InventoryScreenShown?.Invoke();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        // Handle pausing and resuming the game based on input
        private void HandlePauseGame()
        {
            bool once = false;
            if (Input.GetKeyDown(pauseKey))
            {
                isPaused = !isPaused;
                Time.timeScale = isPaused ? 0 : 1;
                once = true;
                UIEvents.GamePlayScreenShown?.Invoke();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            if (isPaused && once)
            {
                UIEvents.PauseScreenShown?.Invoke();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        // Apply final movement to the player character
        private void ApplyFinalMovement()
        {
            if (!characterController.isGrounded)
                moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);
        }

        // Coroutine to handle crouching and standing up animations
        private IEnumerator CrouchStand()
        {
            duringCrouchingAnimation = true;

            float timeElapsed = 0;
            float targetHeight = isCrouching ? standingHeight : crouchHeight;
            float currentHeight = characterController.height;
            Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
            Vector3 currentCenter = characterController.center;

            while (timeElapsed < timeToCrouch)
            {
                characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
                characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            characterController.height = targetHeight;
            characterController.center = targetCenter;

            isCrouching = !isCrouching;
            duringCrouchingAnimation = false;
        }

        // Coroutine to handle zooming in and out
        private IEnumerator ToggleZoom(bool isEnter)
        {
            float targetFOV = isEnter ? zoomFOV : defaultFOV;
            float startingFOV = playerCamera.fieldOfView;
            float timeElapsed = 0;

            while (timeElapsed < timeToZoom)
            {
                playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            playerCamera.fieldOfView = targetFOV;
            zoomRoutine = null;
        }

        // Event handlers for external game events
        private void GameEvents_NoteShown(bool val)
        {
            pauseMouse = val;
        }

        private void GameEvents_GameMonesterFoundMe(bool val)
        {
            pauseMouse = val;
            stop = val;
        }
    }
}
