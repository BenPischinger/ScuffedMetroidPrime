using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SamusFirstPersonScript : MonoBehaviour
{
    CharacterController samus;

    [Header("Camera Settings")]
    public Camera firstPersonCamera;
    float rotationX;
    float lookSensitivity = 2.0f;

    [Space]

    [Header("Movement Settings")]
    private Vector3 moveDirection = Vector3.zero;
    public float movementSpeed;
    public float jumpSpeed;
    public float gravity;

    [Space]

    [Header("Dash Settings")]
    public int maxNumberOfDashes;
    public int numberOfDashes;
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;

    [Space]

    [Header("Pivots")]
    public Transform mainPivot;
    public Transform helmetPivot;

    [Space]

    [Header("UI Settings")]   
    public Image dashOffCooldownImage;
    public Image dashOnCooldownImage;

    bool canMove = true;
    bool canDoubleJump = false;
    bool canDash = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        samus = GetComponent<CharacterController>();

        numberOfDashes = maxNumberOfDashes;
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfDashes == 0)
        {
            canDash = false;
        }

        UpdateView();

        MovementController();

        UIDashCooldown();
    }

    void UpdateView()
    {
        // Handle camera input and limit the angle
        rotationX += -Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, -60, 60);

        firstPersonCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSensitivity, 0);

        mainPivot.transform.localRotation = firstPersonCamera.transform.localRotation;
        helmetPivot.transform.localRotation = firstPersonCamera.transform.localRotation;
    }

    void MovementController()
    {
        // Player is grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? movementSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? movementSpeed * Input.GetAxis("Horizontal") : 0;

        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Player is grounded, so they can jump and double jump as long as they are in mid air
        if (Input.GetButtonDown("Jump") && canMove && samus.isGrounded)
        {

            canDoubleJump = true;

            moveDirection.y = jumpSpeed;

        }
        else if (Input.GetButtonDown("Jump") && canMove && !samus.isGrounded && canDoubleJump)
        {
            moveDirection.y = jumpSpeed;

            canDoubleJump = false;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity if player is not grounded
        if (!samus.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Dash into movement direction
        if (Input.GetKeyDown(KeyCode.LeftShift) && canMove && canDash)
        {
            // Incresaes the movement speed for a split second to simulate a dash
            StartCoroutine(DashRoutine());
            // Counts down the cooldown for the dashes
            StartCoroutine(DashCooldownRoutine());
        }

        // Move the player
        samus.Move(moveDirection * Time.deltaTime);

    }

    // Coroutine to simulate the dash and count down the available dashes
    IEnumerator DashRoutine()
    {
        --numberOfDashes;

        float currentMovementSpeed = movementSpeed;
        movementSpeed = dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        movementSpeed = currentMovementSpeed;
    }

    // Coroutine to keep track of dash cooldown
    IEnumerator DashCooldownRoutine()
    {
        dashOffCooldownImage.fillAmount = 0.0f;
        dashOnCooldownImage.fillAmount = 1.0f;

        yield return new WaitForSeconds(dashCooldown);

        if (numberOfDashes < maxNumberOfDashes)
        {
            ++numberOfDashes;
        }

        canDash = true;
    }

    void UIDashCooldown()
    {
        if(numberOfDashes < maxNumberOfDashes)
        {
            dashOffCooldownImage.fillAmount += 1 / dashCooldown * Time.deltaTime;
            dashOnCooldownImage.fillAmount -= 1 / dashCooldown * Time.deltaTime;
        }
    }

}