using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionScript : MonoBehaviour
{
    public GameObject projectile;
    public GameObject impactVFX;
    public ParticleSystem trail;

    private bool collided;

    // Checks if the projectile collided with anything other than the player or a projectile/itself
    // Instantiates the impact VFX if it hit something
    // Trail stopped manually and VFX destroyed after 2 seconds to give the trail time to catch up with the projectile
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Projectile" && collision.gameObject.tag != "Player" && !collided)
        {
            collided = true;

            var impact = Instantiate(impactVFX, collision.contacts[0].point, Quaternion.identity.normalized) as GameObject;

            Destroy(projectile);

            if (trail)
            {
                trail.Stop();
            }

            Destroy(impact, 2);
            Destroy(gameObject, 2);
        }
    }
}
