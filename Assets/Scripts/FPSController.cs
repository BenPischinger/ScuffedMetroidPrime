using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FPSController : MonoBehaviour
{
    CharacterController samus;

    public Camera firstPersonCamera;
    float rotationX;
    float lookSensitivity = 2.0f;
    public int FPS = 144;

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

    [Header("Weapon Sway Settings")]
    public Transform firePoint;
    public float swayAmount;
    public float maxSwayAmount;
    public float smoothSwayAmount;

    [Space]

    [Header("DoTween Settings")]
    public float punchStrenght = 0.2f;
    public int punchVibrato = 5;
    public float punchDuration = 0.3f;
    public float punchElasticity = 0.5f;

    [Space]
    [Header("Blaster Settings")]
    private float chargedShotTimer = 0;
    private Vector3 projectileDestination;
    public float weaponCooldown;

    public Transform armCannon;
    private Vector3 armCannonInitialPos;
    private Vector3 armCannonLocalPos;

    public GameObject blasterProjectile;
    private ParticleSystem blasterParticleSystem;

    public GameObject chargeProjectile;
    private ParticleSystem chargeParticleSystem;

    public ParticleSystem muzzleVFX;

    public float blasterProjectileSpeed;

    [Space]

    [Header("Rocket Settings")]
    public GameObject rocketProjectile;

    public ParticleSystem rocketMuzzleVFX;

    public float rocketProjectileSpeed;

    [Space]

    [Header("Pivots")]
    public Transform mainPivot;
    public Transform helmetPivot;

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
        chargeParticleSystem = chargeProjectile.GetComponent<ParticleSystem>();

        armCannonLocalPos = armCannon.localPosition;
        armCannonInitialPos = armCannon.localPosition;

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

        WeaponSway();

        ShootBlaster();

        tempQuaternion = samus.transform.rotation;

        ShootRocket();
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

        yield return new WaitForSeconds(dashCooldown);

        if (numberOfDashes < maxNumberOfDashes)
        {
            ++numberOfDashes;
        }

        canDash = true;
    }

    // Fires the blaster. Fire1, left click, can be charged to fire a bigger shot
    // Raycast checks if the projectile hits anything and destroys it
    void ShootBlaster()
    {
        // Handles input for the left mouse button down
        // Starts the animation for the charging of the blaster
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            chargeParticleSystem.Play();
        }

        // Handles input for when the left mouse button is held down
        // Adds up the charge timer and punches the arm cannon back 
        if (Input.GetButton("Fire1") && canShoot)
        {
            chargedShotTimer += Time.deltaTime;

            armCannon.DOLocalMoveZ(armCannonLocalPos.z - 0.22f, punchDuration);
        }

        // Handles input for when the left mouse buttin is let go
        // Clears the charge particles, plays the muzzle flash and shoots the projectile
        if (Input.GetButtonUp("Fire1") && canShoot)
        {
            StartCoroutine(WeaponCoolDown(weaponCooldown));

            chargeParticleSystem.Clear();
            chargeParticleSystem.Stop();

            muzzleVFX.Play();

            DoTweenPunchBack();

            var mainBlaster = blasterParticleSystem.main;

            if (chargedShotTimer < 2)
            {
                mainBlaster.startSize = 1.0f;
            }
            else
            {
                mainBlaster.startSize = 3;
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
        }
    }

    // Similar to the blaster fire, but for rockets
    void ShootRocket()
    {
        if (Input.GetButtonDown("Fire2") && canShoot)
        {
            StartCoroutine(WeaponCoolDown(weaponCooldown));

            DoTweenPunchBack();

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

            rocketMuzzleVFX.Play();

            var projectileObject = Instantiate(rocketProjectile, firePoint.position, tempQuaternion) as GameObject;
            projectileObject.GetComponent<Rigidbody>().velocity = (projectileDestination - firePoint.position).normalized * rocketProjectileSpeed;
        }
    }

    IEnumerator WeaponCoolDown(float weaponCooldown)
    {
        canShoot = false;

        yield return new WaitForSeconds(weaponCooldown);

        canShoot = true;
    }

    void WeaponSway()
    {
        float movementX = -Input.GetAxis("Mouse X") * swayAmount;
        float movementY = -Input.GetAxis("Mouse Y") * swayAmount;
        movementX = Mathf.Clamp(movementX, -maxSwayAmount, maxSwayAmount);
        movementY = Mathf.Clamp(movementY, -maxSwayAmount, maxSwayAmount);

        Vector3 swayPosition = new Vector3(movementX, movementY, 0);
        armCannon.localPosition = Vector3.Lerp(armCannon.localPosition, swayPosition + armCannonInitialPos, Time.deltaTime * smoothSwayAmount);
    }

    void DoTweenPunchBack()
    {
        Sequence doTweenSequence = DOTween.Sequence();
        doTweenSequence.Append(armCannon.DOPunchPosition(new Vector3(0, 0, -punchStrenght), punchDuration, punchVibrato, punchElasticity));
        doTweenSequence.Join(armCannon.DOLocalMove(armCannonLocalPos, punchDuration).SetDelay(punchDuration));
        armCannon.DOComplete();
        armCannon.DOPunchPosition(new Vector3(0, 0, -punchStrenght), punchDuration, punchVibrato, punchElasticity);
    }
}