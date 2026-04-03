using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    // Public configurable variables

    // Camera
    public Camera playerCamera;

    // Freeze
    public bool freeze = false;

    // Walk & Sprint
    [Header("Walk & Sprint")]
    public float walkSpeed = 5f;
    public bool sprintingEnabled = true;
    public float sprintSpeed = 10f;
    // Jump & Gravity
    [Header("Jumping & Gravity")]
    public bool jumpingEnabled = true;
    public float jumpPower = 2f;
    public float gravity = 10f;
    // Look settings
    [Header("Look settings")]
    public float lookSensitivity = 5f;
    public float lookAngle = 90f;
    [Header("Crouching")]
    public float defaultHeight = 2f;
    public bool crouchingEnabled = true;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;
    // Head bob settings
    [Header("Bob settings")]
    public bool bobEnabled = true;
    public float amplitude = 0.015f;
    public float frequency = 10f;
    public float toggleSpeed = 1f;



    // Private variables

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private CharacterController characterController;

    // Default camera position
    private Vector3 defaultCameraPosition;

    private bool canMove = true;
    private bool canJump = true;

    private void Start()
    {
        // Check single instance and destroy if exists already
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);


        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultCameraPosition = playerCamera.transform.localPosition;
    }

    private void Update()
    {
        if (freeze) return;

        characterMovement();
        cameraMovement();
        headMovement();
        resetHeadMovement();
    }

    private void characterMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool sprinting = Input.GetKey(KeyCode.LeftShift);

        // If player can move, multiply movement speed by axis on which player is moving.
        // Ability to adjust movement speed on controllers thanks to decimal value from joytick input
        float movementX = canMove ? (sprinting && sprintingEnabled ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float movementY = canMove ? (sprinting && sprintingEnabled ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * movementX) + (right * movementY);

        // Jump
        if (Input.GetButton("Jump") && jumpingEnabled && canMove && canJump && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Crouching script
        //if (Input.GetButton("C") && ) {}

        // Gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Movement
        characterController.Move(moveDirection * Time.deltaTime);
    }

    // Camera movement function
    private void cameraMovement()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookAngle, lookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSensitivity, 0);
    }

    // Head bob when walking
    private void headMovement()
    {
        if (!bobEnabled) return;

        float speed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;

        if (speed < toggleSpeed || !characterController.isGrounded) return;

        playerCamera.transform.localPosition += footstepMotion(speed);
    }

    private Vector3 footstepMotion(float currentSpeed)
    {
        Vector3 pos = Vector3.zero;

        pos.y = Mathf.Sin(Time.time * frequency) * amplitude * currentSpeed / walkSpeed;
        pos.x = Mathf.Cos(Time.time * frequency / 2) * amplitude / 2 * currentSpeed / walkSpeed;

        return pos;
    }

    private void resetHeadMovement()
    {
        if (playerCamera.transform.localPosition == defaultCameraPosition) return;
        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, defaultCameraPosition, 1 * Time.deltaTime);
    }


}
