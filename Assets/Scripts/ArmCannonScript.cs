using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ArmCannonScript : MonoBehaviour
{
    public Camera firstPersonCamera;

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

    [Header("Blaster Settings")]
    public Transform armCannon;

    public GameObject blasterProjectile;
    private ParticleSystem blasterParticleSystem;

    public GameObject chargeProjectile;
    private ParticleSystem chargeParticleSystem;

    public ParticleSystem muzzleVFX;

    public float blasterProjectileSpeed;
    public float weaponCooldown;

    private float chargedShotTimer = 0;
    private Vector3 projectileDestination;

    private Vector3 armCannonInitialPos;
    private Vector3 armCannonLocalPos;

    [Space]

    [Header("Rocket Settings")]
    public GameObject rocketProjectile;

    public ParticleSystem rocketMuzzleVFX;

    public float rocketProjectileSpeed;

    private int rocketAmount = 250;

    [Space]

    [Header("UI Settings")]
    public Image chargeBarImage;
    public Image chargeBarChargingImage;

    public Transform rocketAmmoImage;
    public Text rocketAmmoText;

    Quaternion tempQuaternion;

    bool canShoot = true;

    // Start is called before the first frame update
    void Start()
    {

        blasterParticleSystem = blasterProjectile.GetComponent<ParticleSystem>();
        chargeParticleSystem = chargeProjectile.GetComponent<ParticleSystem>();

        armCannonLocalPos = armCannon.localPosition;
        armCannonInitialPos = armCannon.localPosition;

    }

    // Update is called once per frame
    void Update()
    {
        tempQuaternion = firstPersonCamera.transform.rotation;

        WeaponSway();

        ShootBlaster();

        ShootRocket();
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
        if (Input.GetButtonDown("Fire2") && canShoot && rocketAmount > 0)
        {
            StartCoroutine(WeaponCoolDown(weaponCooldown));

            DoTweenPunchBack();

            RocketAmmoTracker();

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

    void RocketAmmoTracker()
    {
        --rocketAmount;

        var position = rocketAmmoImage.localPosition;
        position.y -= 0.6552f;
        rocketAmmoImage.localPosition = position;

        rocketAmmoText.text = rocketAmount.ToString();
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
