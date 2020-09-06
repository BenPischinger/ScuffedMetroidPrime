using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class FirstPersonController : MonoBehaviour
{
    public float movementSpeed = 7.5f;

    public float jumpSpeed = 10.0f;
    public float jumpStrength = 0.0f;
    public float maxJumpStrength = 5.0f;
    public float gravity = 13.0f;

    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 70.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public Transform playerCameraParent;
    Vector2 rotation = Vector2.zero;

    public GameObject samus;
    public GameObject ballSamus;
    public GameObject armCannon;

    [HideInInspector]
    public bool canMove = true;
    public bool jumpIsCharging = false;

    public bool canDash = true;
    public int numberOfDashes = 2;
    public float dashTime;
    public float dashSpeed;
    public float dashCooldown;

    public bool thirdPersonIsActive = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        rotation.y = transform.eulerAngles.y;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // FirstPersonMode
        if (!thirdPersonIsActive)
        {
            FirstPersonControls();
        }
        // ThirdPersonMode
        else if (thirdPersonIsActive)
        {
            ThirdPersonControls();
        }

        // Dash ability
        Dash();

        // Switch modes
        SwitchCameraMode();
    }

    private void FixedUpdate()
    {
       
    }

    private void FirstPersonControls()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float curSpeedX = canMove ? movementSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? movementSpeed * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Charged jump
        if (Input.GetKeyDown(KeyCode.Space) && canMove && characterController.isGrounded)
        {
            jumpIsCharging = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space) && canMove && characterController.isGrounded)
        {
            if (jumpStrength > maxJumpStrength)
            {
                jumpStrength = maxJumpStrength;
            }

            jumpIsCharging = false;

            moveDirection.y = jumpSpeed + jumpStrength;

            jumpStrength = 0.0f;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (jumpIsCharging && jumpStrength < maxJumpStrength + 1)
        {
            jumpStrength += Time.deltaTime * 4;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove && !thirdPersonIsActive)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            firstPersonCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void ThirdPersonControls()
    {
        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float curSpeedX = canMove ? movementSpeed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? movementSpeed * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }
    }

    private void Dash()
    {
        if(canMove && canDash && !thirdPersonIsActive)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                while (numberOfDashes > 0)
                {
                    StartCoroutine(DashRoutine());
                    break;
                }
                
                StartCoroutine(DashCooldownRoutine());
            }
        }
    }

    IEnumerator DashRoutine()
    {
        --numberOfDashes;

        movementSpeed = dashSpeed;

        yield return new WaitForSeconds(dashTime);

        movementSpeed = 7.5f;
    }

    IEnumerator DashCooldownRoutine()
    {
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;

        numberOfDashes = 2;
    }

    private void SwitchCameraMode()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!thirdPersonIsActive)
            {
                thirdPersonIsActive = true;
                thirdPersonCamera.enabled = true;
                ballSamus.SetActive(true);

                firstPersonCamera.enabled = false;
                samus.SetActive(false);
                armCannon.SetActive(false);
            }
            else if (thirdPersonIsActive)
            {
                thirdPersonIsActive = false;
                thirdPersonCamera.enabled = false;
                ballSamus.SetActive(false);

                firstPersonCamera.enabled = true;
                samus.SetActive(true);
                armCannon.SetActive(true);
            }

            StartCoroutine(SwitchModes());
        }
    }

    IEnumerator SwitchModes()
    {
        yield return new WaitForSeconds(0.5f);
    }
}