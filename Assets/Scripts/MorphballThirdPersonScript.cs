using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphballThirdPersonScript : MonoBehaviour
{
   
    Rigidbody morphballRigidBody;
    CharacterController morphballCharacterController;

    public Transform cameraPivot;
    public Camera morphballCamera;
    private Vector3 moveDirection;
    private Vector2 rotation;

    [Header("Movement Settings")]
    public float lookSensitifity;
    public float speed;
    public float jumpSpeed;
    public float lookXLimit;

    bool canMove = true;
    bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        morphballRigidBody = GetComponentInChildren<Rigidbody>();
        morphballCharacterController = GetComponent<CharacterController>();

        rotation.y = transform.eulerAngles.y;
    }

    private void Update()
    {

        rotation.y += Input.GetAxis("Mouse X") * lookSensitifity;
        rotation.x += -Input.GetAxis("Mouse Y") * lookSensitifity;

        rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);

        morphballCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);

        cameraPivot.transform.eulerAngles = new Vector2(0, rotation.y);

        cameraPivot.transform.position = morphballRigidBody.transform.position;
    }

    void FixedUpdate()
    {
        CheckIfGrounded();

        if (canMove)
        {
            Vector3 forward = cameraPivot.transform.TransformDirection(Vector3.forward);
            Vector3 right = cameraPivot.transform.TransformDirection(Vector3.right);

            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;

            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButtonDown("Jump"))
            {
                Vector3 tempVector = morphballRigidBody.velocity;
                tempVector.y = jumpSpeed;
                morphballRigidBody.velocity = tempVector;
            }

            morphballRigidBody.AddForce(moveDirection);
        }
    }

    void CheckIfGrounded()
    {
        RaycastHit2D[] hits;

        //We raycast down 1 pixel from this position to check for a collider
        Vector2 positionToCheck = transform.position;
        hits = Physics2D.RaycastAll(positionToCheck, new Vector2(0, -1), 0.01f);

        //if a collider was hit, we are grounded
        if (hits.Length > 0)
        {
            isGrounded = true;
        }
    }
}
