using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
//run during jump ?
// #####################################################################
// ***************^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^***************
// ***************^                                     ^***************
// ***************^  Published by The indie club        ^***************
// ***************^                                     ^***************
// ***************^  Copyright © 2023 by Ali Mohamed    ^***************
// ***************^                                     ^***************
// ***************^   All rights reserved.              ^***************
// ***************^                                     ^***************
// ***************^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^***************
// #####################################################################
public class playerController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool isSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && characterController.isGrounded && !duringCrouchingAnimation;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool canZoom = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool canLean = true;

    [Header("Control")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private KeyCode leanRightKey = KeyCode.E;
    [SerializeField] private KeyCode leanLeftKey = KeyCode.Q;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f; 
    [SerializeField] private float sprintSpeed = 7.0f;
    [SerializeField] private float crouchtSpeed = 1.5f;
    private Vector2 currentInput;
    private Vector3 moveDirection;

    // We can use " lookSpeed " instead of " lookSpeedX " & " lookSpeedY ".
    // We can use " camLimit " instead of " upperCamLimit " & " lowerCamLimit ".
    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeed = 2.0f;
    //[SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    //[SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 90)] private float camLimit = 90;
    //[SerializeField, Range(1, 90)] private float upperCamLimit = 80.0f;
    //[SerializeField, Range(1, 90)] private float lowerCamLimit = 80.0f;
    private float rotationX = 0;
    private float rotationY = 0;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouching Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchingAnimation;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.11f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float ZoomFOV = 30f;
    private float defaultFOV;
    private Coroutine zoomRoutine;

    // add any type of colliders to the object that want to interact with for raycast detection.
    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance = default;
    [SerializeField] private LayerMask interactionLayer = default;
    private Interactable currentInteractable;

    [Header("Lean Parameters")]
    [SerializeField] private LayerMask Layr;
    [SerializeField] private Animator camLean; 
    
    private Camera playerCamera;
    private CharacterController characterController;

    void Awake()
    {
        //playerCamera = GetComponentInChildren<Camera>();
        playerCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        characterController = GetComponent<CharacterController>();

        defaultYPos = playerCamera.transform.localPosition.y;

        defaultFOV = playerCamera.fieldOfView;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        /*if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward * 5))
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 5, Color.green);
        }
        else
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * 5, Color.red);
        }*/
        
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
               

            ApplyFinalMovement(); 
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? crouchtSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchtSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationY = Input.GetAxis("Mouse X") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -camLimit, camLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, rotationY, 0);
    }

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

    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    private void HandheHeadBob()
    {
        if (!characterController.isGrounded) return;

        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    private void HandleZoom()
    {
        if (Input.GetKeyDown(zoomKey))
        {
            if(zoomRoutine != null)
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

    private void HandleInteractionCheck()
    {
        if(Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if(hit.collider.gameObject.layer == 9 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID() ))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if(currentInteractable)
                    currentInteractable.OnFocus();
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.onLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer)) 
        {
            currentInteractable.OnInteract();
        }
    }

    private void HandleLean()
    {
        if (Input.GetKey(leanRightKey) && !Physics.Raycast(transform.position, transform.right, out RaycastHit hit, 1f, Layr))
        {
            camLean.ResetTrigger("idle");
            camLean.ResetTrigger("left");
            camLean.SetTrigger("right");
        }
        else if (Input.GetKey(leanLeftKey) && !Physics.Raycast(transform.position, -transform.right, out hit, 1f, Layr))
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
    private void ApplyFinalMovement()
    {
        if(!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

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
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;
        

        duringCrouchingAnimation = false;
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? ZoomFOV : defaultFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0;

        while (timeElapsed < timeToZoom) 
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed/timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        playerCamera.fieldOfView = targetFOV;   
        zoomRoutine = null;
    }
}
