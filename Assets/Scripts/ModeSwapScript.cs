using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSwapScript : MonoBehaviour
{
    [Header("Swap Settings")]
    public GameObject samusGameObject;
    public GameObject uiCanvas;
    public GameObject morphballGameObject;
    public GameObject morphballCamera;
    public float swapCD;

    bool samusIsActive = true;
    bool canSwap = true;

    // Keeps samus and the morphball at the same position at all times and simply activates/deactives the character when Q is pressed
    void Update()
    {
        if (samusIsActive)
        {
            morphballGameObject.transform.position = samusGameObject.transform.position;
            morphballGameObject.transform.rotation = samusGameObject.transform.rotation;
            morphballCamera.transform.position = samusGameObject.transform.position;
            morphballCamera.transform.eulerAngles = new Vector3(0.0f, samusGameObject.transform.eulerAngles.y, 0.0f);
        }
        else
        {
            samusGameObject.transform.position = morphballGameObject.transform.position;
            samusGameObject.transform.eulerAngles = new Vector3(0.0f, morphballCamera.transform.eulerAngles.y, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.Q) && canSwap)
        {
            StartCoroutine(SwapModes());
        }
    }

    // Coroutine to add a cooldown to mode swapping
    // Dashes and UI need to be reset here since the coroutine from the first person script is stopped upon switching
    IEnumerator SwapModes()
    {   
        canSwap = false;

        if (samusIsActive)
        {
            samusIsActive = false;

            samusGameObject.SetActive(false);
            samusGameObject.GetComponent<SamusFirstPersonScript>().numberOfDashes = samusGameObject.GetComponent<SamusFirstPersonScript>().maxNumberOfDashes;
            samusGameObject.GetComponent<SamusFirstPersonScript>().canDash = true;
            samusGameObject.GetComponent<SamusFirstPersonScript>().ResetUI();
            morphballCamera.SetActive(true);
            morphballGameObject.SetActive(true);
            morphballGameObject.GetComponent<Rigidbody>().WakeUp();
        }
        else
        {
            samusIsActive = true;

            morphballCamera.SetActive(false);
            morphballGameObject.SetActive(false);
            morphballGameObject.GetComponent<Rigidbody>().Sleep();
            samusGameObject.SetActive(true);
        }

        yield return new WaitForSeconds(swapCD);

        canSwap = true;
    }

}
