using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Vector2 sensitivity;
    [SerializeField] Transform camTransform, camHolder;
    [SerializeField] [Range(-90, 90)] float camRotationMin, camRotationMax;
    [SerializeField] float gravity;
    float camRotation;
    public bool canMove {get; private set;} = true;
    Vector2 movementInput, rotationInput;
    InputSystem_Actions inputActions;
    CharacterController characterController;
    [SerializeField] float camTransitionTime = 1.2f;
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        movementInput = Vector2.zero;
        rotationInput = Vector2.zero;

        LockCam(false);
        Cursor.visible = false;
    }

    void Update()
    {
        if(!canMove) return;

        if(movementInput.sqrMagnitude != 0 || characterController.isGrounded)
        {
            var moveDirection = transform.TransformDirection(new(movementInput.x, -gravity, movementInput.y));
            characterController.Move(speed * Time.deltaTime * moveDirection);
        }

        if(rotationInput.sqrMagnitude != 0)
        {
            var playerRotation = transform.localEulerAngles;
            playerRotation.y += rotationInput.x * sensitivity.x * Time.deltaTime;

            transform.localEulerAngles = playerRotation;

            camRotation = Math.Clamp(camRotation + rotationInput.y * Time.deltaTime * sensitivity.y, camRotationMin, camRotationMax);
            camHolder.localEulerAngles = new(-camRotation, 0, 0);

            camTransform.SetPositionAndRotation(camHolder.position, camHolder.rotation);
        }
    }

    void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => 
            movementInput = ctx.ReadValue<Vector2>();

        inputActions.Player.Move.canceled += ctx => 
            movementInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx =>
            rotationInput = ctx.ReadValue<Vector2>();

        inputActions.Player.Look.canceled += ctx => 
            rotationInput = Vector2.zero;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    public void LockCam(bool lockCam)
    {
        canMove = !lockCam;
        Cursor.lockState = lockCam ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public IEnumerator LerpCam(Transform target = null)
    {
        if(target == null) {
            target = camHolder;
            LockCam(false);
        }
        camTransform.GetPositionAndRotation(out Vector3 camStartPosition, out Quaternion camStartRotation);
        float elapsedTime = 0;

        while(elapsedTime < camTransitionTime)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / camTransitionTime;

            camTransform.SetPositionAndRotation(
                Vector3.Lerp(camStartPosition, target.position, percentage),
                Quaternion.Lerp(camStartRotation, target.rotation, percentage)
            );
            yield return null;
        }
        camTransform.SetPositionAndRotation(target.position, target.rotation);
        if(target != camHolder) LockCam(true);
    }
}
