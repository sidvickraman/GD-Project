using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class AnimationAndMovementController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera aimVirtualCamera;

    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;
    Vector2 currentMovementInput;
    float currentRotationInput = 0.0f;
    float rotationSpeed = 60.0f;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;
    bool isAimingPressed;
    bool isShootingPressed;

    [SerializeField]
    private Transform bulletPrefab;
    [SerializeField]
    private Transform bulletPosition;
    [SerializeField]
    private Transform bulletDirection;

    float walkMultiplier = 5.0f;
    float runMultiplier = 15.0f;

    int isWalkingHash;
    int isRunningHash;
    int isAimingHash;

    Transform cameraTransform;

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isAimingHash = Animator.StringToHash("isAiming");
        aimVirtualCamera.gameObject.SetActive(false);

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;

        playerInput.CharacterControls.Rotate.started += onRotate;
        playerInput.CharacterControls.Rotate.canceled += onRotate;

        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;

        playerInput.CharacterControls.Aim.started += onAim;
        playerInput.CharacterControls.Aim.canceled += onAim;

        playerInput.CharacterControls.Shoot.started += onShoot;
        playerInput.CharacterControls.Shoot.canceled += onShoot;
    }

    void onShoot(InputAction.CallbackContext context){
        isShootingPressed = context.ReadValueAsButton();
    }

    void onAim(InputAction.CallbackContext context)
    {
        isAimingPressed = context.ReadValueAsButton();
        if (isAimingPressed){
            aimVirtualCamera.gameObject.SetActive(true);
        }
        else{
            aimVirtualCamera.gameObject.SetActive(false);
        }
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void onRotate(InputAction.CallbackContext context)
    {
        currentRotationInput = context.ReadValue<float>();
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x * walkMultiplier;
        currentMovement.z = currentMovementInput.y * walkMultiplier;
        currentMovement = transform.TransformDirection(currentMovement);
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        currentRunMovement = transform.TransformDirection(currentRunMovement);
        isMovementPressed = (currentMovementInput.x != 0 || currentMovementInput.y != 0);
    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isAiming = animator.GetBool(isAimingHash);

        if (!isWalking && isMovementPressed){
            animator.SetBool(isWalkingHash, true);
        }
        else if (isWalking && !isMovementPressed){
            animator.SetBool(isWalkingHash, false);
        }

        if (!isRunning && (isMovementPressed && isRunPressed)){
            animator.SetBool(isRunningHash, true);
        }
        else if (isRunning && (!isMovementPressed || !isRunPressed)){
            animator.SetBool(isRunningHash, false);
        }

        if (!isRunning){
            if (!isAiming && isAimingPressed){
                animator.SetBool(isAimingHash, true);
            }
            else if (isAiming && !isAimingPressed){
                animator.SetBool(isAimingHash, false);
            }
        }
    }

    void handleGravity()
    {
        if (characterController.isGrounded){
            float groundedGravity = -0.05f;
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else{
            float gravity = -9.8f;
            currentMovement.y = gravity;
            currentRunMovement.y = gravity;
        }
    }

    void Update()
    {
        handleGravity();
        handleAnimation();
        if (isRunPressed){
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else{
            characterController.Move(currentMovement * Time.deltaTime);
        }

        if (currentRotationInput != 0){
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed * currentRotationInput);
        }

        if (isAimingPressed && isShootingPressed){
            Vector3 bulletDirectionRay = bulletDirection.position - bulletPosition.position;
            Instantiate(bulletPrefab, bulletPosition.position, Quaternion.LookRotation(bulletDirectionRay, Vector3.up));
            isShootingPressed = false;
        }
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
