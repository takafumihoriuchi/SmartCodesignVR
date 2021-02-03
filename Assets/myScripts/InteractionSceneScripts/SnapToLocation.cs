using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToLocation : MonoBehaviour
{
    public bool grabbed;
    private bool insideSnapZone;
    public bool Snapped;
    public GameObject Object;
    public GameObject SnapRotationReference;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == Object.name)
        {
            insideSnapZone = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == Object.name)
        {
            insideSnapZone = false;

        }
    }

    void SnapObject()
    {
        if (grabbed == false && insideSnapZone == true)
        {
            Object.gameObject.transform.position = transform.position;
            Object.gameObject.transform.rotation = SnapRotationReference.transform.rotation;
            Snapped = true;
        }
        else if (grabbed == true && insideSnapZone == true)
        {
            Snapped = false;
        }
    }

    void Update()
    {
        grabbed = Object.GetComponent<OVRGrabbable>().isGrabbed;
        SnapObject();
    }

}
