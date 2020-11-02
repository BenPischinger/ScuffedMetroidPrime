﻿using System.Collections;
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
    public float mass;
    public float speed;
    public float jumpSpeed;
    public float lookXLimit;

    bool canMove = true;
    public bool isGrounded = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        morphballRigidBody = GetComponentInChildren<Rigidbody>();
        morphballCharacterController = GetComponent<CharacterController>();

        morphballRigidBody.mass = mass;

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

    void CheckIfGrounded()
    {
        RaycastHit hit;
        float distance = 1f;
        Vector3 dir = new Vector3(0, -1);

        if (Physics.Raycast(transform.position, dir, out hit, distance))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
