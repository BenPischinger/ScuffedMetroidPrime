using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPortFollowerScript : MonoBehaviour
{
    public Transform target;
    public float lerp = .25f;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, lerp);
    }
}
