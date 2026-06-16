using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Vector2 sensitivity;
    [SerializeField] Transform camTransform;
    [SerializeField] [Range(-90, 90)] float camRotationMin, camRotationMax;
    float camRotation;
    Vector2 movementInput, rotationInput;
    InputSystem_Actions inputActions;
    CharacterController characterController;
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        movementInput = Vector2.zero;
        rotationInput = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if(movementInput.sqrMagnitude != 0)
        {
            var moveDirection = transform.TransformDirection(new(movementInput.x, 0, movementInput.y));
            characterController.Move(speed * Time.deltaTime * moveDirection);
        }

        if(rotationInput.sqrMagnitude != 0)
        {
            var playerRotation = transform.localEulerAngles;
            playerRotation.y += rotationInput.x * sensitivity.x * Time.deltaTime;

            transform.localEulerAngles = playerRotation;

            camRotation = Math.Clamp(camRotation + rotationInput.y * Time.deltaTime * sensitivity.y, camRotationMin, camRotationMax);
            camTransform.localEulerAngles = new(-camRotation, 0, 0);
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

    void OnDsable()
    {
        inputActions.Player.Disable();
    }
}
