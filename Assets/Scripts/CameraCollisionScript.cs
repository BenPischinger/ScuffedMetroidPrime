using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionScript : MonoBehaviour
{
    public Transform referenceTransform;
    public float collisionOffset = 0.2f; //To prevent Camera from clipping through Objects

    Vector3 defaultPos;
    Vector3 directionNormalized;
    Transform parentTransform;
    float defaultDistance;

    void Start()
    {
        defaultPos = transform.localPosition;
        directionNormalized = defaultPos.normalized;
        parentTransform = transform.parent;
        defaultDistance = Vector3.Distance(defaultPos, Vector3.zero);
    }

    // Moves the camera when there's an object between the camera and the player so it doesn't clip through said objects
    void FixedUpdate()
    {
        Vector3 currentPos = defaultPos;
        RaycastHit hit;
        Vector3 dirTmp = parentTransform.TransformPoint(defaultPos) - referenceTransform.position;
        if (Physics.SphereCast(referenceTransform.position, collisionOffset, dirTmp, out hit, defaultDistance))
        {
            currentPos = (directionNormalized * (hit.distance - collisionOffset));
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, currentPos, Time.deltaTime * 15f);
    }
}
