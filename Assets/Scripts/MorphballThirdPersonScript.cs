using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphballThirdPersonScript : MonoBehaviour
{
    Rigidbody morphballRigidBody;

    public Transform cameraPivot;
    public Camera morphballCamera;
    private Vector3 moveDirection;
    private Vector2 rotation;

    [Header("Movement Settings")]
    public float lookSensitifity;
    public float mass;
    public float speed;
    public float jumpSpeed;
    public float positivelookXLimit;
    public float negativeLookXLimit;

    bool canMove = true;
    bool isGrounded = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        morphballRigidBody = GetComponentInChildren<Rigidbody>();
        morphballRigidBody.mass = mass;

        rotation.y = transform.eulerAngles.y;
    }

    private void Update()
    {
        // Handles the mouse input to turn the camera around the morphball
        rotation.y += Input.GetAxis("Mouse X") * lookSensitifity;
        rotation.x += -Input.GetAxis("Mouse Y") * lookSensitifity;

        rotation.x = Mathf.Clamp(rotation.x, -negativeLookXLimit, positivelookXLimit);

        cameraPivot.transform.eulerAngles = new Vector2(rotation.x, rotation.y);

        // Position of the camera is set to the center of the rendered object since the morphball pivot is not at the center
        cameraPivot.transform.position = GetComponent<Renderer>().bounds.center; 

        // Pump by applying force in the Y direction
        if (Input.GetButtonDown("Jump") && canMove && isGrounded)
        {
            Vector3 tempVector = morphballRigidBody.velocity;
            tempVector.y = jumpSpeed;
            morphballRigidBody.velocity = tempVector;
        }
    }

    void FixedUpdate()
    {
        CheckIfGrounded();

        // Moving the morphball by applying foce in the look directino of the camera
        if (canMove)
        {
            Vector3 forward = cameraPivot.transform.TransformDirection(Vector3.forward);
            Vector3 right = cameraPivot.transform.TransformDirection(Vector3.right);

            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;

            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            morphballRigidBody.AddForce(moveDirection);
        }
    }

    // Ground check by raycasting downwards to see if there's distance between the ground and the morphball
    void CheckIfGrounded()
    {
        RaycastHit hit;
        float distance = 1.0f;
        Vector3 dir = new Vector3(0.0f, -1.0f, 0.0f);

        if (Physics.Raycast(GetComponent<Renderer>().bounds.center, dir, out hit, distance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
