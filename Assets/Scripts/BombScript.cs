using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    //private GameObject bombObject;

    [Header("Bomb Settings")]
    public GameObject bomb;
    public GameObject explosionVFX;
    public float explosionForce;
    public float explosionUpforce;
    public float explosionRadius;
    public float explosionTimer;

    private Vector3 explosionPosition;

    private bool canShoot = true;

    // Start is called before the first frame update
    void Update()
    {
        StartCoroutine(SpawnBomb());
    }

    // Coroutine for bomb spawn to update the position of the bomb rigidbody right before the explosion to spawn the VFX at the right position
    IEnumerator SpawnBomb()
    {
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            var bombObject = Instantiate(bomb, transform.position, Quaternion.identity) as GameObject;

            Physics.IgnoreCollision(bombObject.GetComponent<Collider>(), GetComponent<Collider>());

            yield return new WaitForSeconds(explosionTimer);

            explosionPosition = bombObject.transform.position;

            Explode(); 

            Destroy(bombObject);
        }
    }

    void Explode()
    {
        Collider[] collidersToDestroy = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (Collider nearbyObjects in collidersToDestroy)
        {
            DestructibleScript dest = nearbyObjects.GetComponent<DestructibleScript>();

            if (dest != null)
            {
                dest.DestroyObject();
            }
        }

        Collider[] collidersToApplyForce = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach(Collider nearbyObjects in collidersToApplyForce)
        {
            Rigidbody rb = nearbyObjects.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, explosionUpforce, ForceMode.Impulse);
            }
        }

        var explosion = Instantiate(explosionVFX, explosionPosition, Quaternion.identity.normalized) as GameObject;

        Destroy(explosion, 2);
    }
}
