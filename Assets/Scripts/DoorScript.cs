using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject door;

    private bool doorIsOpening = false;
    private bool doorIsClosed = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            doorIsOpening = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (doorIsOpening)
        {
           StartCoroutine(OpenSesame());
        }

        if(!doorIsClosed)
        {
            StartCoroutine(CloseSesame());
        }
    }

    IEnumerator OpenSesame()
    {
        var doorPosition = door.transform.position;

        if (doorPosition.y > -6)
        {

            doorPosition.y -= 2 * Time.deltaTime;

            door.transform.position = doorPosition;
        }
            
        yield return new WaitForSeconds(6.0f);

        doorIsOpening = false;
        doorIsClosed = false;
    }

    IEnumerator CloseSesame()
    {
        var doorPosition = door.transform.position;

        if(doorPosition.y < 0)
        {
            doorPosition.y += 2 * Time.deltaTime;

            door.transform.position = doorPosition;
        }

        yield return new WaitForSeconds(6.0f);

        doorIsClosed = true;
    }
}
