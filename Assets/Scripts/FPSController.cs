using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    CharacterController samus;

    public Camera firstPersonCamera;
    float rotationX;
    float lookSensitivity = 2.0f;
    public int FPS = 144;

    private Vector3 moveDirection = Vector3.zero;
    public float movementSpeed;

    public float jumpSpeed;
    public float gravity;

    public int maxNumberOfDashes;
    public int numberOfDashes;
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;

    public Transform firePoint;
    public GameObject blasterProjectile;
    private ParticleSystem blasterParticleSystem;
    public float blasterProjectileSpeed;
    private Vector3 projectileDestination;
    public GameObject muzzle;
    private float chargedShotTimer = 0;

    public GameObject rocketProjectile;
    public float rocketProjectileSpeed;
    public GameObject rocketMuzzle;
    Quaternion tempQuaternion;

    bool canMove = true;
    bool canDoubleJump = false;
    bool canDash = true;
    bool canShoot = true;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = FPS;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        samus = GetComponent<CharacterController>();
        blasterParticleSystem = blasterProjectile.GetComponent<ParticleSystem>();

        numberOfDashes = maxNumberOfDashes;
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfDashes == 0)
        {
            canDash = false;
        }

        MovementController();

        FireBlaster();

        tempQuaternion = samus.transform.rotation;

        FireRocket();
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
        if(Input.GetKeyDown(KeyCode.LeftShift) && canMove && canDash)
        {
            // Incresaes the movement speed for a split second to simulate a dash
            StartCoroutine(DashRoutine());
            // Counts down the cooldown for the dashes
            StartCoroutine(DashCooldownRoutine());
        }

        // Move the player
        samus.Move(moveDirection * Time.deltaTime);

        // Handle camera input and limit the angle
        rotationX += -Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, -70, 70);

        firstPersonCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSensitivity, 0);

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
        
        yield return new WaitForSeconds(dashCooldown);

        if (numberOfDashes < maxNumberOfDashes)
        {
            ++numberOfDashes;
        }

        canDash = true;
    }

    // Fires the blaster. Fire1, left click, can be charged to fire a bigger shot
    // Raycast checks if the projectile hits anything and destroys it
    void FireBlaster()
    {
        if (Input.GetButton("Fire1") && canShoot)
        {
            chargedShotTimer += Time.deltaTime;
           
        }

        if(Input.GetButtonUp("Fire1") && canShoot)
        {
            
            if (chargedShotTimer > 3)
            {
                var main = blasterParticleSystem.main;
                main.startSize = 10.0f;
            }
            else
            {
                var main = blasterParticleSystem.main;
                main.startSize = chargedShotTimer * 3;
            }

            chargedShotTimer = 0;
            
            Ray ray = firstPersonCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                projectileDestination = raycastHit.point;
            }
            else
            {
                projectileDestination = ray.GetPoint(1000);
            }

            var projectileObject = Instantiate(blasterProjectile, firePoint.position, Quaternion.identity) as GameObject;
            projectileObject.GetComponent<Rigidbody>().velocity = (projectileDestination - firePoint.position).normalized * blasterProjectileSpeed;
            var muzzleObject = Instantiate(muzzle, firePoint.position, Quaternion.identity) as GameObject;

            Destroy(muzzleObject, 1);
            Destroy(projectileObject, 10);
        }
    }

    void FireRocket()
    {
        if (Input.GetButtonUp("Fire2") && canShoot)
        {
            Ray ray = firstPersonCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                projectileDestination = raycastHit.point;
            }
            else
            {
                projectileDestination = ray.GetPoint(1000);
            }

            var projectileObject = Instantiate(rocketProjectile, firePoint.position, tempQuaternion) as GameObject;
            projectileObject.GetComponent<Rigidbody>().velocity = (projectileDestination - firePoint.position).normalized * rocketProjectileSpeed;
            var muzzleObject = Instantiate(rocketMuzzle, firePoint.position, Quaternion.identity) as GameObject;

            Destroy(muzzleObject, 1);
            Destroy(projectileObject, 10);
        }
    }
}
