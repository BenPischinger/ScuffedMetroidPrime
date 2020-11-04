using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmetLerpScript : MonoBehaviour
{
    public Vector2 amount;
    public float lerp;

    // Short helmet swaying while looking around by lerping between two positions
    void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, y * amount.y, lerp), Mathf.LerpAngle(transform.localEulerAngles.y, x * amount.x, lerp), 0);
    }
}
