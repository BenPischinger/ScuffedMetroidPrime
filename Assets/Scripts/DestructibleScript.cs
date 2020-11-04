using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleScript : MonoBehaviour
{

    public GameObject destroyedVersion;

    // Quick script for making objects destructible
    // Simply instantiates a destroyed version of the object and then destroys the object itself upon its destruction
    public void DestroyObject()
    {
        if (destroyedVersion != null)
        {
            Instantiate(destroyedVersion, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
